﻿// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Aspire.Hosting;
#pragma warning restore IDE0130 // Namespace does not match folder structure

internal static class ContainerResourceBuilderExtensions
{
    private const int AspNetCoreDefaultHttpsPort = 8081;

    private const string AspNetCoreHttpsPortsEnv = "ASPNETCORE_HTTPS_PORTS";
    private const string KestrelCertPathEnv = "ASPNETCORE_Kestrel__Certificates__Default__Path";
    private const string KestrelCertPasswordEnv = "ASPNETCORE_Kestrel__Certificates__Default__Password";

    internal static IResourceBuilder<TResource> RunWithHttpsDevCert<TResource>(
        this IResourceBuilder<TResource> builder,
        IResourceBuilder<ParameterResource>? devCertPassword = null,
        Action<string>? onSuccessfulExport = null) where TResource : ContainerResource
    {
        if (!builder.ApplicationBuilder.ExecutionContext.IsRunMode ||
            !builder.ApplicationBuilder.Environment.IsDevelopment())
        {
            return builder;
        }

        builder.ApplicationBuilder.Eventing.Subscribe<BeforeStartEvent>(async (e, ct) =>
        {
            var logger = e.Services.GetRequiredService<ResourceLoggerService>().GetLogger(builder.Resource);

            // Export the ASP.NET Core HTTPS development certificate & private key to files and configure the resource to use them via
            // the specified environment variables.
            var passwordParameter = devCertPassword?.Resource
                ?? ParameterResourceBuilderExtensions.CreateDefaultPasswordParameter(builder.ApplicationBuilder, "cert-password");

            var (exported, certPath) = await TryExportDevCertificateAsync(builder.ApplicationBuilder, logger, passwordParameter);

            if (!exported)
            {
                // The export failed for some reason, don't configure the resource to use the certificate.
                return;
            }

            // Bind-mount the certificate files into the container.
            const string DEV_CERT_BIND_MOUNT_DEST_DIR = "/etc/ssl/aspnetcert/";

            var certFileName = Path.GetFileName(certPath);
            var bindSource = Path.GetDirectoryName(certPath) ?? throw new UnreachableException();

            var certFileDest = $"{DEV_CERT_BIND_MOUNT_DEST_DIR}/{certFileName}";

            builder.WithHttpsEndpoint(targetPort: AspNetCoreDefaultHttpsPort, env: AspNetCoreHttpsPortsEnv)
                   .WithBindMount(bindSource, DEV_CERT_BIND_MOUNT_DEST_DIR, isReadOnly: true)
                   .WithEnvironment(context =>
                   {
                       context.EnvironmentVariables[KestrelCertPathEnv] = certFileDest;
                       context.EnvironmentVariables[KestrelCertPasswordEnv] = passwordParameter;
                   });

            onSuccessfulExport?.Invoke(certPath);
        });

        return builder;
    }

    private static async Task<(bool, string CertFilePath)> TryExportDevCertificateAsync(
        IDistributedApplicationBuilder builder,
        ILogger logger,
        ParameterResource password)
    {
        var tempDir = builder.GetAspireTempPath();
        var certExportPath = Path.Combine(tempDir, "dev-cert.pfx");

        if (File.Exists(certExportPath))
        {
            // Certificate already exported, return the path.
            logger.UsingPreviouslyExportedDevCertFiles(certExportPath);

            return (true, certExportPath);
        }

        if (!Directory.Exists(tempDir))
        {
            logger.CreatingDirectoryToExportDevCert(tempDir);

            Directory.CreateDirectory(tempDir);
        }

        string[] args =
        [
            "dev-certs",
            "https",
            "--export-path",
            $"\"{certExportPath}\"",
            "--password",
            $"\"{password.Value}\""
        ];
        var argsString = string.Join(' ', args);

        logger.RunningCommandToExportDevCert(argsString);

        var exportStartInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = argsString,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            WindowStyle = ProcessWindowStyle.Hidden,
        };

        var exportProcess = new Process { StartInfo = exportStartInfo };

        Task? stdOutTask = null;
        Task? stdErrTask = null;

        try
        {
            try
            {
                if (exportProcess.Start())
                {
                    stdOutTask = ConsumeOutput(exportProcess.StandardOutput, msg => logger.LogStandardOutput(msg));
                    stdErrTask = ConsumeOutput(exportProcess.StandardError, msg => logger.LogErrorOutput(msg));
                }
            }
            catch (Exception ex)
            {
                logger.FailedToStartHttpsDevCertificateExportProcess(ex);

                return default;
            }

            var timeout = TimeSpan.FromSeconds(5);
            var exited = exportProcess.WaitForExit(timeout);

            if (exited && File.Exists(certExportPath))
            {
                logger.DevCertExported(certExportPath);

                return (true, certExportPath);
            }

            if (exportProcess is { HasExited: true, ExitCode: not 0 })
            {
                logger.HttpsDevCertExportFailed(exportProcess.ExitCode);

            }
            else if (!exportProcess.HasExited)
            {
                exportProcess.Kill(true);

                logger.HttpsDevCertExportTimedOut(timeout.TotalSeconds);
            }
            else
            {
                logger.HttpsDevCertExportFailedUnknownReason();
            }

            return default;
        }
        finally
        {
            await Task.WhenAll(stdOutTask ?? Task.CompletedTask, stdErrTask ?? Task.CompletedTask);
        }

        static async Task ConsumeOutput(TextReader reader, Action<string> callback)
        {
            var buffer = new char[256];
            int charsRead;

            while ((charsRead = await reader.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                callback(new string(buffer, 0, charsRead));
            }
        }
    }
}

internal static partial class Log
{
    [LoggerMessage(Message = "Running command to export dev cert: dotnet {ExportCmd}")]
    public static partial void RunningCommandToExportDevCert(
        this ILogger logger,
        string exportCmd,
        LogLevel logLevel = LogLevel.Trace);

    [LoggerMessage(Message = "> {StandardOutput}")]
    public static partial void LogStandardOutput(
        this ILogger logger,
        string standardOutput,
        LogLevel logLevel = LogLevel.Information);

    [LoggerMessage(Message = "! {ErrorOutput}")]
    public static partial void LogErrorOutput(
        this ILogger logger,
        string errorOutput,
        LogLevel logLevel = LogLevel.Error);

    [LoggerMessage(Message = "Failed to start HTTPS dev certificate export process")]
    public static partial void FailedToStartHttpsDevCertificateExportProcess(
        this ILogger logger,
        Exception exception,
        LogLevel logLevel = LogLevel.Error);

    [LoggerMessage(Message = "Using previously exported dev cert files '{CertPath}'")]
    public static partial void UsingPreviouslyExportedDevCertFiles(
        this ILogger logger,
        string certPath,
        LogLevel logLevel = LogLevel.Debug);

    [LoggerMessage(Message = "Creating directory to export dev cert to '{ExportDir}'")]
    public static partial void CreatingDirectoryToExportDevCert(
        this ILogger logger,
        string exportDir,
        LogLevel logLevel = LogLevel.Trace);

    [LoggerMessage(Message = "Dev cert exported to '{CertPath}'")]
    public static partial void DevCertExported(
        this ILogger logger,
        string certPath,
        LogLevel logLevel = LogLevel.Debug);

    [LoggerMessage(Message = "HTTPS dev certificate export failed with exit code {ExitCode}")]
    public static partial void HttpsDevCertExportFailed(
        this ILogger logger,
        int exitCode,
        LogLevel logLevel = LogLevel.Error);

    [LoggerMessage(Message = "HTTPS dev certificate export timed out after {TimeoutSeconds} seconds")]
    public static partial void HttpsDevCertExportTimedOut(
        this ILogger logger,
        double timeoutSeconds,
        LogLevel logLevel = LogLevel.Error);

    [LoggerMessage(Message = "HTTPS dev certificate export failed for an unknown reason")]
    public static partial void HttpsDevCertExportFailedUnknownReason(
        this ILogger logger,
        LogLevel logLevel = LogLevel.Error);
}