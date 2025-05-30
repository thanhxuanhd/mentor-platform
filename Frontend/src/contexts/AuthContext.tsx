import { jwtDecode } from "jwt-decode";
import React, { createContext, useEffect, useMemo, useState } from "react";
import type { ReactNode } from "react";

interface User {
  id: string;
  fullName: string;
  email: string;
  role: string;
}

export interface AuthContextProps {
  user: User | null;
  isAuthenticated: boolean;
  setIsAuthenticated: (value: boolean) => void;
  setUser: (value: User) => void;
  setToken: (value: string) => void;
  removeToken: () => void;
  loading: boolean;
}

export const AuthContext = createContext<AuthContextProps>({
  user: null,
  isAuthenticated: false,
  setIsAuthenticated: () => {},
  setUser: () => {},
  setToken: () => {},
  removeToken: () => {},
  loading: true,
});

interface AuthProviderProps {
  children: ReactNode;
}

interface Token {
  sub: string;
  exp: number;
  userId: string;
  email: string;
  "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name": string;
  "http://schemas.microsoft.com/ws/2008/06/identity/claims/role": string;
}

export const AuthProvider: React.FC<AuthProviderProps> = ({ children }) => {
  const [user, setUser] = useState<User | null>(null);
  const [loading, setLoading] = useState<boolean>(true);
  const [isAuthenticated, setIsAuthenticated] = useState<boolean>(false);

  const setToken = (token: string) => {
    window.localStorage.setItem("token", token);
    fetchUserFromToken();
  };

  const removeToken = () => {
    window.localStorage.removeItem("token");
    setUser(null);
    setIsAuthenticated(false);
    setLoading(false);
  };

  const getToken = () => {
    return window.localStorage.getItem("token");
  };

  const fetchUserFromToken = () => {
    const storedToken = getToken();
    if (!storedToken) {
      setUser(null);
      setIsAuthenticated(false);
      setLoading(false);
      return;
    }

    const decodedToken = jwtDecode<Token>(storedToken);
    // Check token expiration
    if (decodedToken.exp && decodedToken.exp * 1000 < Date.now()) {
      removeToken();
      setUser(null);
      setIsAuthenticated(false);
      setLoading(false);
      return;
    }

    setUser({
      id: decodedToken.sub,
      email: decodedToken.email,
      fullName:
        decodedToken[
          "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"
        ],
      role: decodedToken[
        "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
      ],
    });
    setIsAuthenticated(true);
    setLoading(false);
  };

  useEffect(() => {
    fetchUserFromToken();
  }, []);

  const authContextValue: AuthContextProps = useMemo(
    () => ({
      user,
      isAuthenticated,
      setIsAuthenticated,
      setUser,
      setToken,
      removeToken,
      loading,
    }),
    [user, isAuthenticated, loading],
  );

  return (
    <AuthContext.Provider value={authContextValue}>
      {children}
    </AuthContext.Provider>
  );
};
