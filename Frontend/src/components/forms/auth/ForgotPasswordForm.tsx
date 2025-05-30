"use client";

import type React from "react";
import { useState } from "react";
import { userService } from "../../../services/user/userService";
import { HiExclamationCircle } from "react-icons/hi";
import { useNavigate } from "react-router-dom";

const ForgotPasswordForm: React.FC = () => {
  const [email, setEmail] = useState("");
  const [submitted, setSubmitted] = useState(false);
  const [error, setError] = useState("");
  const [isLoading, setIsLoading] = useState(false);
  const [fieldError, setFieldError] = useState<{ email?: string }>({});
  const navigate = useNavigate();

  const validateEmail = (email: string) => /\S+@\S+\.\S+/.test(email);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    const trimmedEmail = email.trim();
    const errors: { email?: string } = {};

    if (!trimmedEmail) {
      errors.email = "Please enter your email";
    } else if (!validateEmail(trimmedEmail)) {
      errors.email = "Email must be in a correct format";
    }

    setFieldError(errors);
    if (Object.keys(errors).length > 0) {
      setIsLoading(false);
      return;
    }

    setIsLoading(true);
    setError("");

    try {
      await userService.forgotPassword(trimmedEmail);
      setSubmitted(true);
      setTimeout(() => {
        navigate("/reset-password", { replace: true });
      }, 1000);
    } catch (err) {
      console.error("Forgot password error:", err);
      setError("Account does not exist. Please check your email.");
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="w-full max-w-md mx-auto mt-10 bg-gray-800 p-6 rounded shadow">
      <h2 className="text-2xl font-bold text-center text-white">
        Forgot Password
      </h2>

      {error && (
        <div
          className="mt-6 bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded flex items-center justify-center"
          role="alert"
        >
          <HiExclamationCircle className="w-5 h-5 mr-2" />
          <span>{error}</span>
        </div>
      )}

      {submitted ? (
        <div className="mt-6 text-green-600 dark:text-green-400 text-center">
          Password reset successful. Please check your email.
        </div>
      ) : (
        <form onSubmit={handleSubmit} className="space-y-6 mt-6" noValidate>
          <div>
            <label
              htmlFor="email"
              className="block text-sm font-medium text-white"
            >
              Email
            </label>
            <input
              id="email"
              type="text"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              className={`mt-1 w-full px-3 py-2 border rounded bg-gray-700 text-white ${
                fieldError.email ? "border-red-500" : "border-gray-300"
              }`}
              placeholder="you@example.com"
            />
            {fieldError.email && (
              <p className="text-red-500 text-sm mt-1">{fieldError.email}</p>
            )}
          </div>

          <button
            type="submit"
            disabled={isLoading}
            className={`w-full text-white font-semibold py-2 rounded ${
              isLoading
                ? "bg-orange-400 cursor-not-allowed"
                : "bg-orange-600 hover:bg-orange-700"
            }`}
          >
            {isLoading ? "Sending..." : "Send New Password"}
          </button>
        </form>
      )}

      <div className="text-center mt-4">
        <a
          href="/login"
          className="text-sm text-blue-600 hover:text-blue-800 dark:text-blue-400"
        >
          Back to Sign In
        </a>
      </div>
    </div>
  );
};

export default ForgotPasswordForm;
