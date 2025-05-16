import { jwtDecode } from "jwt-decode";
import React, {
  createContext,
  useEffect,
  useMemo,
  useState,
} from "react";
import type {ReactNode} from "react";
interface User {
  id: string;
  username: string;
  dateOfBirth: string;
  isFirstTimeLogin: boolean;
  staffCode: string;
  role: string;
  location: number;
}

export interface AuthContextProps {
  token: string | null;
  isAuthenticated: boolean;
  setIsAuthenticated: (value: boolean) => void;
  user: User;
  setUser: (value: User) => void;
  loading: boolean;
}

export const AuthContext = createContext<AuthContextProps>({
  token: null,
  isAuthenticated: false,
  setIsAuthenticated: () => {},
  user: {
    id: "",
    username: "",
    dateOfBirth: "",
    isFirstTimeLogin: false,
    staffCode: "",
    role: "Admin",
    location: 1,
  },
  setUser: () => {},
  loading: true,
});

interface AuthProviderProps {
  children: ReactNode;
}

interface Token {
  UserId: string;
  "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name": string;
  StaffCode: string;
  "http://schemas.microsoft.com/ws/2008/06/identity/claims/role": string;
  IsFirstTimeLogin: string;
  DateOfBirth: string;
  Location: string;
}

export const AuthProvider: React.FC<AuthProviderProps> = ({ children }) => {
  const [token, setToken] = useState<string | null>(
    localStorage.getItem("token"),
  );
  const [user, setUser] = useState<User>({
    id: "",
    username: "",
    dateOfBirth: "",
    isFirstTimeLogin: false,
    staffCode: "",
    role: "Staff",
    location: 1,
  });

  const [loading, setLoading] = useState<boolean>(true);
  const [isAuthenticated, setIsAuthenticated] = useState<boolean>(!!token);

  const fetchUserFromToken = () => {
    const storedToken = localStorage.getItem("token");

    if (storedToken) {
      setIsAuthenticated(true);
      setToken(storedToken);
      const decodedToken = jwtDecode<Token>(storedToken);
      setUser({
        id: decodedToken.UserId,
        username:
          decodedToken[
            "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"
          ],
        dateOfBirth: decodedToken.DateOfBirth,
        isFirstTimeLogin: decodedToken.IsFirstTimeLogin === "true",
        staffCode: decodedToken.StaffCode,
        role: decodedToken[
          "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
        ],
        location: parseInt(decodedToken.Location),
      });
    } else {
      setUser({
        id: "",
        username: "",
        dateOfBirth: "",
        isFirstTimeLogin: false,
        staffCode: "",
        role: "Staff",
        location: 1,
      });
      setIsAuthenticated(false);
    }
    setLoading(false);
  };

  useEffect(() => {
    fetchUserFromToken();
  }, [token, isAuthenticated]);

  const authContextValue: AuthContextProps = useMemo(
    () => ({
      token,
      isAuthenticated,
      setIsAuthenticated,
      user,
      setUser,
      loading,
    }),
    [token, user, isAuthenticated, loading],
  );

  return (
    <AuthContext.Provider value={authContextValue}>
      {children}
    </AuthContext.Provider>
  );
};
