import type React from "react";
import SignUpForm from "../../components/forms/auth/SignUpForm";

export const SignUp: React.FC = () => {
  return (
    <div className="h-screen flex flex-col items-center justify-center">
      <SignUpForm />
    </div>
  );
};
