import { useState, useRef, useEffect } from 'react';
import { ChevronDown, Asterisk, Smile, Angry, Hash, Eye, EyeOff, Volume2, Square, Strikethrough, Minus, Check } from 'lucide-react';

interface StrategyOption {
  name: string;
  description?: string;
}

interface StrategyDropdownProps {
  value: string;
  options: StrategyOption[];
  onChange: (value: string) => void;
}

const getStrategyIcon = (strategyName: string): React.ReactNode => {
  const icons: Record<string, React.ReactNode> = {
    'Asterisk': <Asterisk className="w-5 h-5 text-amber-400" />,
    'Emoji': <Smile className="w-5 h-5 text-yellow-400" />,
    'AngerEmoji': <Angry className="w-5 h-5 text-red-400" />,
    'MiddleSwearEmoji': <span className="text-base">🤬</span>,
    'RandomAsterisk': <Hash className="w-5 h-5 text-amber-400" />,
    'MiddleAsterisk': <Eye className="w-5 h-5 text-amber-400" />,
    'FirstLetterThenAsterisk': <EyeOff className="w-5 h-5 text-amber-400" />,
    'VowelAsterisk': <span className="text-base font-mono text-amber-400">a*</span>,
    'Bleep': <Volume2 className="w-5 h-5 text-blue-400" />,
    'RedactedRectangle': <Square className="w-5 h-5 text-zinc-400" />,
    'StrikeThrough': <Strikethrough className="w-5 h-5 text-zinc-400" />,
    'Underscores': <Minus className="w-5 h-5 text-zinc-400" />,
    'Grawlix': <span className="text-base font-bold text-pink-400">#$@!</span>,
    'BoldGrawlix': <span className="text-base font-black text-pink-400">#$@!</span>,
  };
  return icons[strategyName] || <Asterisk className="w-5 h-5 text-amber-400" />;
};

export default function StrategyDropdown({ value, options, onChange }: StrategyDropdownProps) {
  const [isOpen, setIsOpen] = useState(false);
  const dropdownRef = useRef<HTMLDivElement>(null);

  // Close dropdown when clicking outside
  useEffect(() => {
    const handleClickOutside = (event: MouseEvent) => {
      if (dropdownRef.current && !dropdownRef.current.contains(event.target as Node)) {
        setIsOpen(false);
      }
    };

    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, []);

  // Close dropdown on escape key
  useEffect(() => {
    const handleEscape = (event: KeyboardEvent) => {
      if (event.key === 'Escape') {
        setIsOpen(false);
      }
    };

    document.addEventListener('keydown', handleEscape);
    return () => document.removeEventListener('keydown', handleEscape);
  }, []);

  const handleSelect = (optionName: string) => {
    onChange(optionName);
    setIsOpen(false);
  };

  const selectedOption = options.find(o => o.name === value);

  return (
    <div ref={dropdownRef} className="relative">
      {/* Trigger Button */}
      <button
        type="button"
        onClick={() => setIsOpen(!isOpen)}
        className="dropdown-trigger flex items-center gap-3 rounded-lg pl-4 pr-3 py-2.5 transition-colors cursor-pointer min-w-[200px]"
      >
        <span className="flex items-center justify-center w-6">
          {getStrategyIcon(value)}
        </span>
        <span className="dropdown-trigger-text text-base flex-1 text-left">
          {selectedOption?.name || value}
        </span>
        <ChevronDown className={`w-5 h-5 text-zinc-500 transition-transform ${isOpen ? 'rotate-180' : ''}`} />
      </button>

      {/* Dropdown Menu */}
      {isOpen && (
        <div className="dropdown-menu absolute z-50 mt-1 w-full min-w-[240px] py-2 rounded-lg shadow-xl max-h-[300px] overflow-y-auto">
          {options.map((option) => (
            <button
              key={option.name}
              type="button"
              onClick={() => handleSelect(option.name)}
              className={`dropdown-option w-full flex items-center gap-4 px-4 py-2.5 text-left transition-colors cursor-pointer ${
                option.name === value ? 'selected' : ''
              }`}
            >
              <span className="flex items-center justify-center w-6">
                {getStrategyIcon(option.name)}
              </span>
              <span className="dropdown-option-text text-base flex-1">
                {option.name}
              </span>
              {option.name === value && (
                <Check className="dropdown-check w-5 h-5" />
              )}
            </button>
          ))}
        </div>
      )}
    </div>
  );
}

export { getStrategyIcon };
