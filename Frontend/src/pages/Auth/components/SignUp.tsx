import type React from "react";
import SignUpForm from "../../../components/forms/auth/SignUpForm";

export const SignUp: React.FC = () => {
  return (
    <div className="min-h-screen flex flex-col h-full items-center justify-center">
      <SignUpForm />
    </div>
  );
};
