import { useState, useEffect, useCallback, useRef } from 'react';
import { Sparkles, Copy, Check, AlertTriangle, Zap, Github, RefreshCw, Sun, Moon, Monitor, FileJson, Send } from 'lucide-react';
import ReactMarkdown from 'react-markdown';
import rehypeRaw from 'rehype-raw';
import remarkGfm from 'remark-gfm';
import { fetchStrategies, createSignalRConnection, LiveStreamClient, filterText } from './api';
import type { StrategyResponse, ProfanityFilterResponse } from './types';
import type { HubConnection } from '@microsoft/signalr';
import StrategyDropdown from './components/StrategyDropdown';

export default function App() {
  const [text, setText] = useState('');
  const [strategy, setStrategy] = useState(() => {
    return localStorage.getItem('strategy') || 'Asterisk';
  });
  const [strategies, setStrategies] = useState<StrategyResponse[]>([]);
  const [result, setResult] = useState<ProfanityFilterResponse | null>(null);
  const [isLiveMode, setIsLiveMode] = useState(true);
  const [isConnected, setIsConnected] = useState(false);
  const [showConnected, setShowConnected] = useState(false);
  const [isProcessing, setIsProcessing] = useState(false);
  const [copied, setCopied] = useState(false);
  const [isResetting, setIsResetting] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [theme, setTheme] = useState<'auto' | 'dark' | 'light'>(() => {
    const stored = localStorage.getItem('theme');
    return (stored as 'auto' | 'dark' | 'light') || 'auto';
  });

  const connectionRef = useRef<HubConnection | null>(null);
  const liveClientRef = useRef<LiveStreamClient | null>(null);
  const debounceRef = useRef<ReturnType<typeof setTimeout> | null>(null);

  // Strategy persistence
  useEffect(() => {
    localStorage.setItem('strategy', strategy);
  }, [strategy]);

  // Delay showing connected status by 500ms
  useEffect(() => {
    if (isConnected) {
      const timer = setTimeout(() => setShowConnected(true), 500);
      return () => clearTimeout(timer);
    } else {
      setShowConnected(false);
    }
  }, [isConnected]);

  // Theme management
  useEffect(() => {
    localStorage.setItem('theme', theme);
    const root = document.documentElement;
    
    if (theme === 'auto') {
      const prefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches;
      root.classList.toggle('dark', prefersDark);
      root.classList.toggle('light', !prefersDark);
    } else {
      root.classList.toggle('dark', theme === 'dark');
      root.classList.toggle('light', theme === 'light');
    }
  }, [theme]);

  // Listen for system theme changes when in auto mode
  useEffect(() => {
    if (theme !== 'auto') return;
    
    const mediaQuery = window.matchMedia('(prefers-color-scheme: dark)');
    const handler = (e: MediaQueryListEvent) => {
      const root = document.documentElement;
      root.classList.toggle('dark', e.matches);
      root.classList.toggle('light', !e.matches);
    };
    
    mediaQuery.addEventListener('change', handler);
    return () => mediaQuery.removeEventListener('change', handler);
  }, [theme]);

  const cycleTheme = () => {
    setTheme(current => {
      if (current === 'auto') return 'dark';
      if (current === 'dark') return 'light';
      return 'auto';
    });
  };

  const getThemeIcon = () => {
    if (theme === 'auto') return <Monitor className="w-5 h-5" />;
    if (theme === 'dark') return <Moon className="w-5 h-5" />;
    return <Sun className="w-5 h-5 text-orange-500" />;
  };

  // Load strategies on mount
  useEffect(() => {
    const loadData = async () => {
      try {
        const strategiesData = await fetchStrategies();
        setStrategies(strategiesData);
        // Use stored strategy if valid, otherwise use first available
        const storedStrategy = localStorage.getItem('strategy');
        if (storedStrategy && strategiesData.some(s => s.name === storedStrategy)) {
          setStrategy(storedStrategy);
        } else if (strategiesData.length > 0) {
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

  const handleManualFilter = async () => {
    if (!text.trim()) return;
    
    setIsProcessing(true);
    try {
      const response = await filterText({
        text,
        strategy,
        target: 'Body',
      });
      setResult(response);
    } catch (err) {
      console.error('Filter error:', err);
      setError('Failed to filter text. Please try again.');
    } finally {
      setIsProcessing(false);
    }
  };

  const handleReset = () => {
    setIsResetting(true);
    setText('');
    setResult(null);
    setTimeout(() => setIsResetting(false), 500);
  };

  return (
    <div className="h-full flex flex-col">
      {/* Header */}
      <header className="glass border-b border-white/10 px-4 sm:px-6 py-4">
        <div className="max-w-7xl mx-auto flex items-center justify-between">
          <div className="flex items-center gap-3">
            <div className="p-2.5 rounded-2xl bg-gradient-to-br from-amber-400 via-orange-500 to-red-500 shadow-lg shadow-orange-500/50 ring-4 ring-orange-500/20 glow-amber animate-pulse-soft">
              <span className="text-2xl" role="img" aria-label="swear">🤬</span>
            </div>
            <div>
              <h1 className="text-2xl font-bold bg-gradient-to-r from-amber-500 via-orange-500 to-pink-500 bg-clip-text text-transparent">
                Profanity Filter
              </h1>
              <p className="text-sm hidden sm:block">✨ Real-time potty mouth detector ✨</p>
            </div>
          </div>

          <div className="flex items-center gap-4">
            {/* Connection Status */}
            <div className="flex items-center gap-2 text-base transition-all duration-500">
              {isLiveMode ? (
                <>
                  <span className={`w-2 h-2 rounded-full transition-colors duration-500 ${showConnected ? 'bg-green-500 animate-pulse' : 'bg-red-500'}`} />
                  <span className="hidden sm:inline transition-opacity duration-500">
                    {showConnected ? 'Live' : 'Connecting...'}
                  </span>
                </>
              ) : (
                <div className="flex items-center gap-2 text-zinc-500">
                  <span className="w-2 h-2 rounded-full bg-zinc-500 transition-colors duration-500" />
                  <span className="hidden sm:inline">Disconnected</span>
                </div>
              )}
            </div>

            <a
              href="/scalar/v1"
              target="_blank"
              rel="noopener noreferrer"
              className="p-2 rounded-lg hover:bg-black/10 dark:hover:bg-white/10 transition-colors"
              title="OpenAPI Reference"
            >
              <FileJson className="w-5 h-5" />
            </a>

            <a
              href="https://github.com/IEvangelist/profanity-filter"
              target="_blank"
              rel="noopener noreferrer"
              className="p-2 rounded-lg hover:bg-black/10 dark:hover:bg-white/10 transition-colors"
            >
              <Github className="w-5 h-5" />
            </a>

            <button
              onClick={cycleTheme}
              className="p-2 rounded-lg hover:bg-white/10 dark:hover:bg-white/10 light:hover:bg-black/10 transition-colors cursor-pointer"
              title={`Theme: ${theme.charAt(0).toUpperCase() + theme.slice(1)}`}
            >
              {getThemeIcon()}
            </button>
          </div>
        </div>
      </header>

      {/* Error Overlay */}
      {error && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/60 backdrop-blur-sm">
          <div className="glass rounded-2xl p-6 max-w-md mx-4 shadow-2xl border border-red-500/30">
            <div className="flex items-start gap-4">
              <div className="p-3 rounded-full bg-red-500/20">
                <AlertTriangle className="w-6 h-6 text-red-400" />
              </div>
              <div className="flex-1">
                <h3 className="text-lg font-semibold text-red-400 mb-2">Connection Error</h3>
                <p className="text-base opacity-80">{error}</p>
              </div>
            </div>
            <button
              onClick={() => setError(null)}
              className="mt-4 w-full py-2 px-4 rounded-lg bg-red-500/20 hover:bg-red-500/30 text-red-400 transition-colors cursor-pointer"
            >
              Dismiss
            </button>
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
                <StrategyDropdown
                  value={strategy}
                  options={strategies}
                  onChange={setStrategy}
                />

                <div className="ml-auto flex items-center gap-2">
                  <button
                    onClick={handleReset}
                    className="flex items-center gap-1.5 px-3 py-1.5 rounded-lg text-base hover:bg-black/10 dark:hover:bg-white/10 transition-colors cursor-pointer"
                  >
                    <RefreshCw className={`w-5 h-5 transition-transform ${isResetting ? 'animate-spin' : ''}`} />
                    <span className="hidden sm:inline">Reset</span>
                  </button>

                  {!isLiveMode && (
                    <button
                      onClick={handleManualFilter}
                      disabled={!text.trim() || isProcessing}
                      className="flex items-center gap-1.5 px-3 py-1.5 rounded-lg text-base bg-amber-500/20 text-amber-500 border border-amber-500/50 hover:bg-amber-500/30 transition-colors cursor-pointer disabled:opacity-50 disabled:cursor-not-allowed"
                    >
                      <Send className="w-5 h-5" />
                      <span className="hidden sm:inline">Filter</span>
                    </button>
                  )}

                  <button
                    onClick={() => setIsLiveMode(!isLiveMode)}
                    className={`flex items-center gap-1.5 px-3 py-1.5 rounded-lg text-base transition-colors cursor-pointer ${
                      isLiveMode
                        ? 'bg-blue-500/20 text-blue-600 border border-blue-500/50'
                        : 'hover:bg-black/10 dark:hover:bg-white/10'
                    }`}
                  >
                    <Zap className="w-5 h-5" fill={isLiveMode ? 'currentColor' : 'none'} />
                    <span className="hidden sm:inline">Live</span>
                  </button>
                </div>
              </div>

              {/* Text Input */}
              <div className="flex-1 p-4 min-h-0">
                <textarea
                  value={text}
                  onChange={(e) => handleTextChange(e.target.value)}
                  placeholder={isLiveMode ? "Start typing to filter profanity in real-time..." : "Enter text and click Filter to check for profanity..."}
                  autoComplete="off"
                  autoCorrect="off"
                  autoCapitalize="off"
                  spellCheck={false}
                  data-gramm="false"
                  data-gramm_editor="false"
                  data-enable-grammarly="false"
                  className="w-full h-full resize-none bg-transparent text-xl leading-relaxed placeholder:text-zinc-500 focus:outline-none caret-amber-400 cursor-text"
                />
              </div>

              {/* Input Footer */}
              <div className="px-4 py-3 border-t border-black/10 dark:border-white/10 flex items-center justify-between text-base">
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
                    className="flex items-center gap-1.5 px-3 py-1.5 rounded-lg text-base hover:bg-black/10 dark:hover:bg-white/10 transition-colors cursor-pointer"
                  >
                    {copied ? (
                      <>
                        <Check className="w-5 h-5 text-green-400" />
                        <span className="text-green-400">Copied!</span>
                      </>
                    ) : (
                      <>
                        <Copy className="w-5 h-5" />
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
                    <ReactMarkdown remarkPlugins={[remarkGfm]} rehypePlugins={[rehypeRaw]}>{result.filteredText}</ReactMarkdown>
                  </div>
                ) : (
                  <div className="h-full flex items-center justify-center opacity-60">
                    <p>Filtered text will appear here...</p>
                  </div>
                )}
              </div>

              {/* Stats Footer */}
              {result && (
                <div className="px-4 py-3 border-t border-white/10">
                  <div className="flex flex-wrap items-center gap-4 text-base">
                    <div className="flex items-center gap-2">
                      <span className={`w-2 h-2 rounded-full ${result.containsProfanity ? 'bg-amber-500' : 'bg-green-500'}`} />
                      <span>
                        {result.containsProfanity ? 'Profanity detected' : 'Clean text'}
                      </span>
                    </div>
                    {result.containsProfanity && (
                      <>
                        <span className="opacity-50">|</span>
                        <span>
                          <span className="font-medium">{result.matchCount}</span> match{result.matchCount !== 1 ? 'es' : ''}
                        </span>
                        <span className="opacity-50">|</span>
                        <span>
                          Strategy: <span className="text-purple-600">{result.replacementStrategy}</span>
                        </span>
                      </>
                    )}
                  </div>

                  {result.matches && result.matches.length > 0 && (
                    <div className="mt-3 flex flex-wrap gap-2">
                      {result.matches.map((match, i) => (
                        <span
                          key={i}
                          className="px-2 py-1 bg-amber-500/20 border border-amber-500/30 rounded text-sm text-amber-600"
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
      <footer className="glass border-t border-black/10 dark:border-white/10 px-4 py-3">
        <div className="max-w-7xl mx-auto flex items-center justify-between text-sm">
          <div className="flex items-center gap-2">
            <span>Built with</span>
            <a href="https://aspire.dev" target="_blank" rel="noopener noreferrer" className="text-purple-600 hover:text-purple-500 transition-colors">Aspire</a>
            <span>&</span>
            <span className="text-blue-600">React</span>
            <span className="ml-1">🚀</span>
          </div>
          <div className="flex items-center gap-1">
            <span>© {new Date().getFullYear()}</span>
            <a href="https://davidpine.dev" target="_blank" rel="noopener noreferrer" className="hover:text-orange-500 transition-colors">David Pine</a>
          </div>
        </div>
      </footer>
    </div>
  );
}
