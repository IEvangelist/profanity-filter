import { useState, useEffect, useCallback, useRef } from 'react';
import { Sparkles, Copy, Check, AlertTriangle, Zap, Github, RefreshCw, ChevronDown, Asterisk, Smile, Angry, Hash, Eye, EyeOff, Volume2, Square, Strikethrough, Minus } from 'lucide-react';
import ReactMarkdown from 'react-markdown';
import { fetchStrategies, createSignalRConnection, LiveStreamClient } from './api';
import type { StrategyResponse, ProfanityFilterResponse } from './types';
import type { HubConnection } from '@microsoft/signalr';

export default function App() {
  const [text, setText] = useState('');
  const [strategy, setStrategy] = useState('Asterisk');
  const [strategies, setStrategies] = useState<StrategyResponse[]>([]);
  const [result, setResult] = useState<ProfanityFilterResponse | null>(null);
  const [isLiveMode, setIsLiveMode] = useState(true);
  const [isConnected, setIsConnected] = useState(false);
  const [isProcessing, setIsProcessing] = useState(false);
  const [copied, setCopied] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const connectionRef = useRef<HubConnection | null>(null);
  const liveClientRef = useRef<LiveStreamClient | null>(null);
  const debounceRef = useRef<ReturnType<typeof setTimeout> | null>(null);

  // Load strategies on mount
  useEffect(() => {
    const loadData = async () => {
      try {
        const strategiesData = await fetchStrategies();
        setStrategies(strategiesData);
        if (strategiesData.length > 0) {
          setStrategy(strategiesData[0].name);
        }
      } catch (err) {
        setError('Failed to load configuration. Make sure the API is running.');
        console.error(err);
      }
    };
    loadData();
  }, []);

  // SignalR connection management
  useEffect(() => {
    if (!isLiveMode) return;

    const connection = createSignalRConnection();
    connectionRef.current = connection;

    const startConnection = async () => {
      try {
        await connection.start();
        setIsConnected(true);
        setError(null);
        
        const client = new LiveStreamClient(connection);
        client.setHandlers(
          (response) => {
            setResult(response);
            setIsProcessing(false);
          },
          (err) => {
            console.error('Stream error:', err);
            setError('Live stream error occurred.');
          }
        );
        liveClientRef.current = client;
      } catch (err) {
        console.error('Connection failed:', err);
        setIsConnected(false);
        setError('Failed to connect to live stream. Retrying...');
        setTimeout(startConnection, 3000);
      }
    };

    connection.onclose(() => {
      setIsConnected(false);
      liveClientRef.current = null;
    });

    connection.onreconnecting(() => {
      setIsConnected(false);
    });

    connection.onreconnected(() => {
      setIsConnected(true);
      // Recreate client on reconnect
      const client = new LiveStreamClient(connection);
      client.setHandlers(
        (response) => {
          setResult(response);
          setIsProcessing(false);
        },
        (err) => {
          console.error('Stream error:', err);
          setError('Live stream error occurred.');
        }
      );
      liveClientRef.current = client;
    });

    startConnection();

    return () => {
      liveClientRef.current?.complete();
      connection.stop();
    };
  }, [isLiveMode]);

  // Handle text changes with debouncing for live mode
  const handleTextChange = useCallback((newText: string) => {
    setText(newText);

    if (isLiveMode && liveClientRef.current && newText.trim()) {
      if (debounceRef.current) {
        clearTimeout(debounceRef.current);
      }

      setIsProcessing(true);
      debounceRef.current = setTimeout(() => {
        liveClientRef.current?.sendRequest({
          text: newText,
          strategy,
          target: 'Body',
        });
      }, 300);
    } else if (!newText.trim()) {
      setResult(null);
    }
  }, [isLiveMode, strategy]);

  // Re-filter when strategy changes
  useEffect(() => {
    if (text.trim() && isLiveMode && liveClientRef.current) {
      setIsProcessing(true);
      liveClientRef.current.sendRequest({
        text,
        strategy,
        target: 'Body',
      });
    }
  }, [strategy]);

  const handleCopy = async () => {
    if (result?.filteredText) {
      await navigator.clipboard.writeText(result.filteredText);
      setCopied(true);
      setTimeout(() => setCopied(false), 2000);
    }
  };

  const handleReset = () => {
    setText('');
    setResult(null);
  };

  // Get icon for strategy
  const getStrategyIcon = (strategyName: string) => {
    const icons: Record<string, React.ReactNode> = {
      'Asterisk': <Asterisk className="w-4 h-4 text-amber-400" />,
      'Emoji': <Smile className="w-4 h-4 text-yellow-400" />,
      'AngerEmoji': <Angry className="w-4 h-4 text-red-400" />,
      'MiddleSwearEmoji': <span className="text-sm">🤬</span>,
      'RandomAsterisk': <Hash className="w-4 h-4 text-amber-400" />,
      'MiddleAsterisk': <Eye className="w-4 h-4 text-amber-400" />,
      'FirstLetterThenAsterisk': <EyeOff className="w-4 h-4 text-amber-400" />,
      'VowelAsterisk': <span className="text-sm font-mono text-amber-400">a*</span>,
      'Bleep': <Volume2 className="w-4 h-4 text-blue-400" />,
      'RedactedRectangle': <Square className="w-4 h-4 text-zinc-400" />,
      'StrikeThrough': <Strikethrough className="w-4 h-4 text-zinc-400" />,
      'Underscores': <Minus className="w-4 h-4 text-zinc-400" />,
      'Grawlix': <span className="text-sm font-bold text-pink-400">#$@!</span>,
      'BoldGrawlix': <span className="text-sm font-black text-pink-400">#$@!</span>,
    };
    return icons[strategyName] || <Asterisk className="w-4 h-4 text-amber-400" />;
  };

  return (
    <div className="h-full flex flex-col">
      {/* Header */}
      <header className="glass border-b border-white/10 px-4 sm:px-6 py-4">
        <div className="max-w-7xl mx-auto flex items-center justify-between">
          <div className="flex items-center gap-3">
            <div className="p-2.5 rounded-2xl bg-gradient-to-br from-amber-400 via-orange-500 to-red-500 shadow-lg shadow-orange-500/40 glow-amber animate-pulse-soft">
              <span className="text-2xl" role="img" aria-label="swear">🤬</span>
            </div>
            <div>
              <h1 className="text-xl font-bold bg-gradient-to-r from-amber-300 via-orange-400 to-pink-500 bg-clip-text text-transparent">
                Profanity Filter
              </h1>
              <p className="text-xs text-zinc-400 hidden sm:block">✨ Real-time potty mouth detector ✨</p>
            </div>
          </div>

          <div className="flex items-center gap-4">
            {/* Connection Status */}
            {isLiveMode && (
              <div className="flex items-center gap-2 text-sm">
                <span className={`w-2 h-2 rounded-full ${isConnected ? 'bg-green-400 animate-pulse' : 'bg-red-400'}`} />
                <span className="text-zinc-400 hidden sm:inline">
                  {isConnected ? 'Live' : 'Connecting...'}
                </span>
              </div>
            )}

            <a
              href="https://github.com/IEvangelist/profanity-filter"
              target="_blank"
              rel="noopener noreferrer"
              className="p-2 rounded-lg hover:bg-white/10 transition-colors"
            >
              <Github className="w-5 h-5 text-zinc-400 hover:text-white" />
            </a>
          </div>
        </div>
      </header>

      {/* Error Banner */}
      {error && (
        <div className="bg-red-500/10 border-b border-red-500/20 px-4 py-3">
          <div className="max-w-7xl mx-auto flex items-center gap-2 text-red-400 text-sm">
            <AlertTriangle className="w-4 h-4 shrink-0" />
            <span>{error}</span>
          </div>
        </div>
      )}

      {/* Main Content */}
      <main className="flex-1 overflow-auto p-4 sm:p-6">
        <div className="max-w-7xl mx-auto h-full flex flex-col lg:flex-row gap-6">
          {/* Input Panel */}
          <div className="flex-1 flex flex-col min-h-0">
            <div className="glass rounded-2xl flex flex-col h-full overflow-hidden">
              {/* Controls */}
              <div className="p-4 border-b border-zinc-700/50 flex flex-wrap items-center gap-3">
                <div className="relative">
                  <div className="flex items-center gap-2 bg-zinc-800 border border-zinc-700 rounded-lg pl-3 pr-2 py-2 hover:border-zinc-600 transition-colors">
                    {getStrategyIcon(strategy)}
                    <select
                      value={strategy}
                      onChange={(e) => setStrategy(e.target.value)}
                      className="bg-transparent text-sm text-zinc-200 focus:outline-none appearance-none cursor-pointer pr-6"
                    >
                      {strategies.map((s) => (
                        <option key={s.name} value={s.name} className="bg-zinc-900 text-zinc-200">
                          {s.name}
                        </option>
                      ))}
                    </select>
                    <ChevronDown className="w-4 h-4 text-zinc-500 absolute right-2 pointer-events-none" />
                  </div>
                </div>

                <div className="ml-auto flex items-center gap-2">
                  <button
                    onClick={handleReset}
                    className="flex items-center gap-1.5 px-3 py-1.5 rounded-lg text-sm text-zinc-400 hover:text-white hover:bg-white/10 transition-colors"
                  >
                    <RefreshCw className="w-4 h-4" />
                    <span className="hidden sm:inline">Reset</span>
                  </button>

                  <button
                    onClick={() => setIsLiveMode(!isLiveMode)}
                    className={`flex items-center gap-1.5 px-3 py-1.5 rounded-lg text-sm transition-colors ${
                      isLiveMode
                        ? 'bg-blue-500/20 text-blue-400 border border-blue-500/30'
                        : 'text-zinc-400 hover:text-white hover:bg-white/10'
                    }`}
                  >
                    <Zap className="w-4 h-4" />
                    <span className="hidden sm:inline">Live</span>
                  </button>
                </div>
              </div>

              {/* Text Input */}
              <div className="flex-1 p-4 min-h-0">
                <textarea
                  value={text}
                  onChange={(e) => handleTextChange(e.target.value)}
                  placeholder="Enter text to filter for profanity..."
                  autoComplete="off"
                  autoCorrect="off"
                  autoCapitalize="off"
                  spellCheck={false}
                  data-gramm="false"
                  data-gramm_editor="false"
                  data-enable-grammarly="false"
                  className="w-full h-full resize-none bg-transparent text-lg leading-relaxed placeholder:text-zinc-600 focus:outline-none"
                />
              </div>

              {/* Input Footer */}
              <div className="px-4 py-3 border-t border-white/10 flex items-center justify-between text-sm text-zinc-400">
                <span>{text.length} characters</span>
                {isProcessing && (
                  <span className="flex items-center gap-2 text-blue-400">
                    <div className="w-4 h-4 border-2 border-blue-400 border-t-transparent rounded-full animate-spin" />
                    Processing...
                  </span>
                )}
              </div>
            </div>
          </div>

          {/* Output Panel */}
          <div className="flex-1 flex flex-col min-h-0">
            <div className="glass rounded-2xl flex flex-col h-full overflow-hidden">
              {/* Output Header */}
              <div className="p-4 border-b border-white/10 flex items-center justify-between">
                <div className="flex items-center gap-2">
                  <Sparkles className="w-5 h-5 text-purple-400" />
                  <h2 className="font-semibold">Filtered Output</h2>
                </div>

                {result && (
                  <button
                    onClick={handleCopy}
                    className="flex items-center gap-1.5 px-3 py-1.5 rounded-lg text-sm text-zinc-400 hover:text-white hover:bg-white/10 transition-colors"
                  >
                    {copied ? (
                      <>
                        <Check className="w-4 h-4 text-green-400" />
                        <span className="text-green-400">Copied!</span>
                      </>
                    ) : (
                      <>
                        <Copy className="w-4 h-4" />
                        <span>Copy</span>
                      </>
                    )}
                  </button>
                )}
              </div>

              {/* Output Content */}
              <div className="flex-1 p-4 overflow-auto min-h-0">
                {result ? (
                  <div className="markdown-output">
                    <ReactMarkdown>{result.filteredText}</ReactMarkdown>
                  </div>
                ) : (
                  <div className="h-full flex items-center justify-center text-zinc-500">
                    <p>Filtered text will appear here...</p>
                  </div>
                )}
              </div>

              {/* Stats Footer */}
              {result && (
                <div className="px-4 py-3 border-t border-white/10">
                  <div className="flex flex-wrap items-center gap-4 text-sm">
                    <div className="flex items-center gap-2">
                      <span className={`w-2 h-2 rounded-full ${result.containsProfanity ? 'bg-amber-400' : 'bg-green-400'}`} />
                      <span className="text-zinc-400">
                        {result.containsProfanity ? 'Profanity detected' : 'Clean text'}
                      </span>
                    </div>
                    {result.containsProfanity && (
                      <>
                        <span className="text-zinc-500">|</span>
                        <span className="text-zinc-400">
                          <span className="text-white font-medium">{result.matchCount}</span> match{result.matchCount !== 1 ? 'es' : ''}
                        </span>
                        <span className="text-zinc-500">|</span>
                        <span className="text-zinc-400">
                          Strategy: <span className="text-purple-400">{result.replacementStrategy}</span>
                        </span>
                      </>
                    )}
                  </div>

                  {result.matches && result.matches.length > 0 && (
                    <div className="mt-3 flex flex-wrap gap-2">
                      {result.matches.map((match, i) => (
                        <span
                          key={i}
                          className="px-2 py-1 bg-amber-500/10 border border-amber-500/20 rounded text-xs text-amber-400"
                        >
                          {match}
                        </span>
                      ))}
                    </div>
                  )}
                </div>
              )}
            </div>
          </div>
        </div>
      </main>

      {/* Footer */}
      <footer className="glass border-t border-white/10 px-4 py-3">
        <div className="max-w-7xl mx-auto flex items-center justify-between text-xs text-zinc-500">
          <div className="flex items-center gap-2">
            <span>Built with</span>
            <a href="https://aspire.dev" target="_blank" rel="noopener noreferrer" className="text-purple-400 hover:text-purple-300 font-medium transition-colors">Aspire</a>
            <span>&</span>
            <span className="text-blue-400">React</span>
            <span className="ml-1">🚀</span>
          </div>
          <div className="flex items-center gap-1">
            <span>© 2026</span>
            <a href="https://davidpine.dev" target="_blank" rel="noopener noreferrer" className="text-zinc-400 hover:text-white transition-colors">David Pine</a>
          </div>
        </div>
      </footer>
    </div>
  );
}
