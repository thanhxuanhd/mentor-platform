import LoginForm from "../../../components/forms/auth/LoginForm";
import { useAuth } from "../../../hooks";
import { Navigate } from "react-router-dom";

export const Login: React.FC = () => {
  const { isAuthenticated } = useAuth();

  if (isAuthenticated) return <Navigate to="/" />;

  return (
    <div className="flex h-full flex-col items-center justify-center bg-gradient-to-br from-blue-100 to-red-100 dark:from-gray-800 dark:to-gray-900">
      <div className="mb-24 flex w-full justify-center ">
        <LoginForm />
      </div>
    </div>
  );
};
