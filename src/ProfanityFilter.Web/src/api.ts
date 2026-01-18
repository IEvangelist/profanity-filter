import * as signalR from '@microsoft/signalr';
import type { ProfanityFilterRequest, ProfanityFilterResponse, StrategyResponse, FilterTargetResponse } from './types';

const getBaseUrl = (): string => {
  // In development, use relative URLs (empty string) so requests go through Vite proxy
  // In production, use the API_HTTPS or API_HTTP environment variable
  if (import.meta.env.DEV) {
    return ''; // Use Vite proxy
  }
  return import.meta.env.API_HTTPS || import.meta.env.API_HTTP || '';
};

export async function fetchStrategies(): Promise<StrategyResponse[]> {
  const response = await fetch(`${getBaseUrl()}/profanity/strategies`);
  if (!response.ok) {
    throw new Error('Failed to fetch strategies');
  }
  return response.json();
}

export async function fetchTargets(): Promise<FilterTargetResponse[]> {
  const response = await fetch(`${getBaseUrl()}/profanity/targets`);
  if (!response.ok) {
    throw new Error('Failed to fetch targets');
  }
  return response.json();
}

export async function filterText(request: ProfanityFilterRequest): Promise<ProfanityFilterResponse> {
  const response = await fetch(`${getBaseUrl()}/profanity/filter`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(request),
  });
  if (!response.ok) {
    throw new Error('Failed to filter text');
  }
  return response.json();
}

export function createSignalRConnection(): signalR.HubConnection {
  return new signalR.HubConnectionBuilder()
    .withUrl(`${getBaseUrl()}/profanity/hub`)
    .withAutomaticReconnect()
    .configureLogging(signalR.LogLevel.Information)
    .build();
}

export class LiveStreamClient {
  private connection: signalR.HubConnection;
  private subject: signalR.Subject<Record<string, string>> | null = null;
  private onResult: ((response: ProfanityFilterResponse) => void) | null = null;
  private onError: ((error: Error) => void) | null = null;
  private isStreaming = false;

  constructor(connection: signalR.HubConnection) {
    this.connection = connection;
  }

  setHandlers(
    onResult: (response: ProfanityFilterResponse) => void,
    onError: (error: Error) => void
  ): void {
    this.onResult = onResult;
    this.onError = onError;
  }

  private startStream(): void {
    if (this.isStreaming) {
      console.log('Already streaming, skipping startStream');
      return;
    }
    
    if (this.connection.state !== signalR.HubConnectionState.Connected) {
      console.log('Connection not ready, state:', this.connection.state);
      return;
    }

    console.log('Starting new stream...');
    this.subject = new signalR.Subject<Record<string, string>>();
    this.isStreaming = true;
    
    this.connection.stream<ProfanityFilterResponse>('live', this.subject)
      .subscribe({
        next: (response) => {
          console.log('Received response:', response);
          this.onResult?.(response);
        },
        error: (err) => {
          console.error('Stream error:', err);
          this.isStreaming = false;
          this.subject = null;
          this.onError?.(err);
        },
        complete: () => {
          console.log('Stream completed, will restart on next request');
          this.isStreaming = false;
          this.subject = null;
        },
      });
  }

  sendRequest(request: ProfanityFilterRequest): void {
    console.log('sendRequest called, isStreaming:', this.isStreaming, 'connectionState:', this.connection.state);
    
    // Start stream on demand if not already streaming
    if (!this.isStreaming) {
      this.startStream();
    }

    if (this.subject) {
      console.log('Sending request:', { Text: request.text, Strategy: request.strategy, Target: request.target });
      // Send with PascalCase keys to match server-side C# record
      this.subject.next({
        Text: request.text,
        Strategy: request.strategy,
        Target: request.target,
      });
    } else {
      console.warn('No subject available to send request');
    }
  }

  complete(): void {
    if (this.subject) {
      this.subject.complete();
      this.subject = null;
      this.isStreaming = false;
    }
  }
}
