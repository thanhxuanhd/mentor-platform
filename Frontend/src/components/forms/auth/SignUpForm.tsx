"use client";
import React, { useState } from "react";
import { CheckCircleOutlined } from "@ant-design/icons";
import { useNavigate } from "react-router-dom";
import type { SignUpReq } from "../../../models";
import authService from "../../../services/auth/authService";

const SignUpForm: React.FC = () => {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
  const [error, setError] = useState<string | null>(null);
  const [showNotification, setShowNotification] = useState(false);
  const [agreeToTerms, setAgreeToTerms] = useState(false);

  const navigate = useNavigate();

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);

    if (password.length < 8) {
      setError("Password must be at least 8 characters long.");
      return;
    }

    if (password !== confirmPassword) {
      alert("Passwords do not match.");
      return;
    }

    if (!agreeToTerms) {
      setError("You must agree to the Terms of Service and Privacy Policy.");
      return;
    }

    const signUpData: SignUpReq = {
      email,
      password,
      confirmpassword: confirmPassword,
      roleId: 3,
    };

    try {
      const res = await authService.signUp(signUpData);
      console.log("Signup successful:", res);

      setShowNotification(true);

      setTimeout(() => {
        setShowNotification(false);
        navigate("/step2", { state: { ...res } });
      }, 1000);
    } catch (err) {
      console.error("Signup failed:", err);
      setError("Signup failed. Please try again.");
    }
  };

  return (
    <div className="w-full max-w-md space-y-6 p-6 bg-gray-800 rounded shadow mx-auto text-sm text-gray-300">
      {showNotification && (
        <div className="fixed top-4 right-4 bg-green-100 border-l-4 border-green-500 text-green-700 p-3 rounded shadow-md transition-all duration-500 transform animate-fade-in flex items-center max-w-sm text-sm">
          <CheckCircleOutlined className="text-green-500 text-lg mr-2" />
          <div>
            <p className="font-bold">Success!</p>
            <p>Your account has been created successfully.</p>
          </div>
        </div>
      )}

      <div className="max-w-xl mx-auto">
        <div className="flex justify-between items-center mb-6">
          <h1 className="text-2xl font-bold text-white">Create Your Account</h1>
          <div className="text-orange-500 text-sm">
            <span>Step 1</span> of 3
          </div>
        </div>

        <form onSubmit={handleSubmit} className="space-y-6">
          <div className="space-y-1">
            <label htmlFor="email" className="block text-base">
              Email Address
            </label>
            <input
              id="email"
              type="email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              required
              className="mt-1 w-full px-3 py-2 border border-gray-300 rounded dark:bg-gray-700 dark:text-white focus:outline-none focus:border-blue-500"
            />
          </div>

          <div className="space-y-1">
            <label htmlFor="password" className="block text-base">
              Password
            </label>
            <input
              id="password"
              type="password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              required
              className="mt-1 w-full px-3 py-2 border border-gray-300 rounded dark:bg-gray-700 dark:text-white focus:outline-none focus:border-blue-500"
            />
          </div>

          <div className="space-y-1">
            <label htmlFor="confirmPassword" className="block text-base">
              Confirm Password
            </label>
            <input
              id="confirmPassword"
              type="password"
              value={confirmPassword}
              onChange={(e) => setConfirmPassword(e.target.value)}
              required
              className="mt-1 w-full px-3 py-2 border border-gray-300 rounded dark:bg-gray-700 dark:text-white focus:outline-none focus:border-blue-500"
            />
          </div>

          <div className="flex items-center">
            <input
              id="terms"
              type="checkbox"
              checked={agreeToTerms}
              onChange={(e) => setAgreeToTerms(e.target.checked)}
              className="h-4 w-4 text-blue-600 focus:ring-blue-500 border-gray-500 rounded"
            />
            <label htmlFor="terms" className="ml-2 block text-xs">
              I agree to the{" "}
              <a href="/terms" className="text-orange-500 hover:underline">
                Terms of Service
              </a>{" "}
              and{" "}
              <a href="/privacy" className="text-orange-500 hover:underline">
                Privacy Policy
              </a>
            </label>
          </div>

          {error && <p className="text-red-500 text-xs">{error}</p>}

          <button
            type="submit"
            className="w-full bg-orange-500 hover:bg-orange-600 text-white font-bold py-2 px-4 rounded transition-colors text-sm"
          >
            Continue to Profile Setup
          </button>

          <div className="text-center text-sm">
            <p>
              Already have an account?{" "}
              <a href="/login" className="text-orange-500 hover:underline">
                Sign in
              </a>
            </p>
          </div>
        </form>
      </div>
    </div>
  );
};

export default SignUpForm;
