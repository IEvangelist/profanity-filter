export interface Strategy {
  name: string;
  description: string;
}

export interface FilterTarget {
  name: string;
  description: string;
}

export interface ProfanityFilterRequest {
  text: string;
  strategy: string;
  target: string;
}

export interface ProfanityFilterResponse {
  containsProfanity: boolean;
  inputText: string;
  filteredText: string;
  replacementStrategy: string;
  matchCount: number;
  matches: string[] | null;
}

export interface StrategyResponse {
  name: string;
  description: string;
}

export interface FilterTargetResponse {
  name: string;
  description: string;
}
