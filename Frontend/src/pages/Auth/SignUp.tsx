import type React from "react"
import SignUpForm from "../../components/forms/auth/SignUpForm"

export const SignUp: React.FC = () => {
  return (
    <div className="flex h-screen flex-col items-center justify-center">
      <div className="mb-8 flex flex-col items-center justify-center gap-2 md:mb-24">
        <img src="/favicon.ico" alt="Logo" className="h-20 w-20 sm:h-32 sm:w-32" />
        <p className="text-center text-xl font-bold text-red-600 sm:text-2xl">Rookies - Group 4</p>
      </div>
      <div className="mb-24 flex w-full justify-center">
        <SignUpForm />
      </div>
    </div>
  );
};

