// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ProfanityFilter.Client.Tests;

[TestClass]
public class RealtimeClientSecurityTests
{
    [TestMethod]
    [DataRow("false")]
    [DataRow("true")]
    public async Task StartAsyncRejectsUntrustedCertificateAsync(string runningInContainer)
    {
        using var key = RSA.Create(2048);
        var request = new CertificateRequest(
            "CN=localhost", key, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        var names = new SubjectAlternativeNameBuilder();
        names.AddIpAddress(IPAddress.Loopback);
        request.CertificateExtensions.Add(names.Build());
        using var certificate = request.CreateSelfSigned(
            DateTimeOffset.UtcNow.AddMinutes(-1), DateTimeOffset.UtcNow.AddMinutes(5));

        using var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        var endpoint = (IPEndPoint)listener.LocalEndpoint;
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        var server = RespondWithUntrustedCertificateAsync(listener, certificate, timeout.Token);

        var builder = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
        {
            DisableDefaults = true
        });
        builder.Configuration["DOTNET_RUNNING_IN_CONTAINER"] = runningInContainer;
        builder.AddProfanityFilterClient("test", options =>
            options.ApiBaseAddress = new Uri($"https://127.0.0.1:{endpoint.Port}"));
        using var host = builder.Build();
        using var scope = host.Services.CreateScope();
        var client = scope.ServiceProvider.GetRequiredService<IRealtimeClient>();

        try
        {
            var exception = await Assert.ThrowsExactlyAsync<HttpRequestException>(
                () => client.StartAsync(timeout.Token).AsTask());

            Assert.AreEqual(HttpRequestError.SecureConnectionError, exception.HttpRequestError);
            Assert.IsInstanceOfType<AuthenticationException>(exception.InnerException);
        }
        finally
        {
            await timeout.CancelAsync();
            await server;
            await client.StopAsync(CancellationToken.None);
        }
    }

    private static async Task RespondWithUntrustedCertificateAsync(
        TcpListener listener, X509Certificate2 certificate, CancellationToken cancellationToken)
    {
        try
        {
            using var connection = await listener.AcceptTcpClientAsync(cancellationToken);
            await using var stream = new SslStream(connection.GetStream());
            await stream.AuthenticateAsServerAsync(new SslServerAuthenticationOptions
            {
                ServerCertificate = certificate
            }, cancellationToken);

            await stream.WriteAsync(
                "HTTP/1.1 503 Service Unavailable\r\nContent-Length: 0\r\nConnection: close\r\n\r\n"u8.ToArray(),
                cancellationToken);
        }
        catch (Exception exception) when (
            exception is AuthenticationException or IOException or OperationCanceledException)
        {
            // The client may abort the handshake when it rejects the certificate.
        }
    }
}
