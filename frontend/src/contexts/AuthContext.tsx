"use client";

import React, { createContext, useContext, useEffect, useState } from "react";
import { useRouter } from "next/navigation";

interface AuthUser {
  name: string;
  email: string;
  token: string;
}

interface AuthContextValue {
  user: AuthUser | null;
  login: (token: string, name: string, email: string) => void;
  logout: () => void;
  isLoading: boolean;
}

const AuthContext = createContext<AuthContextValue>({} as AuthContextValue);

export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [user, setUser] = useState<AuthUser | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const router = useRouter();

  useEffect(() => {
    const token = localStorage.getItem("token");
    const stored = localStorage.getItem("user");
    if (token && stored) {
      try {
        const parsed = JSON.parse(stored);
        setUser({ ...parsed, token });
      } catch {
        localStorage.clear();
      }
    }
    setIsLoading(false);
  }, []);

  function login(token: string, name: string, email: string) {
    localStorage.setItem("token", token);
    localStorage.setItem("user", JSON.stringify({ name, email }));
    setUser({ token, name, email });
    router.push("/");
  }

  function logout() {
    localStorage.removeItem("token");
    localStorage.removeItem("user");
    setUser(null);
    router.push("/login");
  }

  return (
    <AuthContext.Provider value={{ user, login, logout, isLoading }}>
      {children}
    </AuthContext.Provider>
  );
}

export const useAuth = () => useContext(AuthContext);
