import { AuthContext } from "../contexts/AuthContext";
import type { AuthContextProps } from "../contexts/AuthContext";
import { useContext } from "react";

export const useAuth = (): AuthContextProps => {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error("useAuth must be used within a AuthProvider");
  }
  return context;
};
