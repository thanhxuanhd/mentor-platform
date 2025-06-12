import { createContext, useMemo, useState, type ReactNode } from "react";

interface UserContextProps {
  isProfileUpdated: boolean;
  setProfileUpdated: (updated: boolean) => void;
}

export const UserContext = createContext<UserContextProps | undefined>(undefined);

interface UserProviderProps {
  children: ReactNode;
  initialProfileUpdated?: boolean;
}

export const UserProvider = ({ children, initialProfileUpdated = false }: UserProviderProps) => {
  const [isProfileUpdated, setProfileUpdated] = useState(initialProfileUpdated);

  const userContextValue: UserContextProps = useMemo(() => ({
    isProfileUpdated,
    setProfileUpdated,
  }), [isProfileUpdated])

  return (
    <UserContext.Provider value={userContextValue}>
      {children}
    </UserContext.Provider>
  );
};
