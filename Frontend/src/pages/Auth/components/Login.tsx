import LoginForm from "../../../components/forms/auth/LoginForm";
import { useAuth } from "../../../hooks";
import { Navigate } from "react-router-dom";

export const Login: React.FC = () => {
  const { isAuthenticated } = useAuth();

  if (isAuthenticated) return <Navigate to="/" />;

  return (
    <div className="h-screen flex flex-col items-center justify-center">
      <LoginForm />
    </div>
  );
};
