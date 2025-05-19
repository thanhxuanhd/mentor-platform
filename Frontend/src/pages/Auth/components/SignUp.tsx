import type React from "react";
import SignUpForm from "../../../components/forms/auth/SignUpForm";

export const SignUp: React.FC = () => {
  return (
    <div className="flex flex-col h-full items-center justify-center bg-gradient-to-br from-blue-100 to-red-100 dark:from-gray-800 dark:to-gray-900">
      <div className="mb-24 flex w-full justify-center">
        <SignUpForm />
      </div>
    </div>
  );
};
