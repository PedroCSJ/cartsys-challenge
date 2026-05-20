export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
}

export interface User {
  id: string;
  name: string;
  email: string;
  createdAt: string;
}

export interface State {
  id: string;
  name: string;
  uf: string;
  createdAt: string;
}

export interface City {
  id: string;
  name: string;
  stateId: string;
  stateName: string;
  stateUF: string;
  createdAt: string;
}

export type LanguageType = 1 | 2 | 3 | 4 | 5;
export const LanguageTypeLabels: Record<LanguageType, string> = {
  1: "Frontend",
  2: "Backend",
  3: "Mobile",
  4: "Database",
  5: "DevOps",
};

export interface Language {
  id: string;
  name: string;
  type: LanguageType;
  typeLabel: string;
  createdAt: string;
}

export type Seniority = 1 | 2 | 3;
export const SeniorityLabels: Record<Seniority, string> = {
  1: "Júnior",
  2: "Pleno",
  3: "Sênior",
};

export interface Developer {
  id: string;
  name: string;
  email: string;
  seniority: Seniority;
  seniorityLabel: string;
  cityId: string;
  cityName: string;
  stateName: string;
  stateUF: string;
  notes: string | null;
  languages: Language[];
  createdAt: string;
}

export interface LoginResponse {
  token: string;
  name: string;
  email: string;
  expiresAt: string;
}
