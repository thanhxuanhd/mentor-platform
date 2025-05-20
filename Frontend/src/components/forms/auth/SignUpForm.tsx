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
  const [errors, setErrors] = useState<{ email?: string; password?: string; confirmPassword?: string; terms?: string }>({});
  const [showNotification, setShowNotification] = useState(false);
  const [agreeToTerms, setAgreeToTerms] = useState(false);

  const navigate = useNavigate();

  const validateEmail = (email: string) => /\S+@\S+\.\S+/.test(email);

  const validatePassword = (password: string) => {
    const lengthValid = password.length >= 8 && password.length <= 32;
    const containsMix = /[A-Za-z]/.test(password) && /\d/.test(password) && /[^A-Za-z0-9]/.test(password);
    const noSpaces = !/\s/.test(password);
    return lengthValid && containsMix && noSpaces;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    const newErrors: typeof errors = {};

    const trimmedEmail = email.trim();

    if (!trimmedEmail) {
      newErrors.email = "Please enter your email";
    } else if (!validateEmail(trimmedEmail)) {
      newErrors.email = "Email must be in a correct format";
    } else if (trimmedEmail.length > 50) {
      newErrors.email = "Email should not exceed 50 characters";
    }

    if (!password) {
      newErrors.password = "Please enter your password";
    } else {
      if (password.length < 8 || password.length > 32) {
        newErrors.password = "Password should be in 8-32 characters";
      } else if (!validatePassword(password)) {
        if (/\s/.test(password)) {
          newErrors.password = "Password should not include space characters";
        } else {
          newErrors.password = "Password must include a mix of letters, numbers, symbols";
        }
      }
    }

    if (!confirmPassword) {
      newErrors.confirmPassword = "Please enter your confirm password";
    } else if (password !== confirmPassword) {
      newErrors.confirmPassword = "Confirm password is incorrect";
    }

    if (!agreeToTerms) {
      newErrors.terms = "Please agree with Terms of Service and Privacy Policy";
    }

    setErrors(newErrors);

    if (Object.keys(newErrors).length > 0) return;

    const signUpData: SignUpReq = {
      email: trimmedEmail,
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
        navigate("/signup/step2", { replace: true });
      }, 1000);
    } catch (err) {
      console.error("Signup failed:", err);
      setErrors({ email: "Already have this email" });
    }
  };

  return (
    <div className="min-h-[60vh] w-full max-w-md space-y-6 bg-[#1A1F2B] dark:bg-gray-800 p-6 rounded shadow mx-auto text-sm text-gray-300">
      {showNotification && (
        <div className="fixed top-4 right-4 bg-green-100 border-l-4 border-green-500 text-green-700 p-3 rounded shadow-md transition-all duration-500 transform animate-fade-in flex items-center max-w-sm text-sm">
          <CheckCircleOutlined className="text-green-500 text-lg mr-2" />
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
            <label htmlFor="email" className="block text-base">Email Address</label>
            <input
              id="email"
              type="text"
              value={email}
              placeholder="Enter your email"
              onChange={(e) => setEmail(e.target.value)}
              className="mt-1 w-full px-3 py-2 border border-gray-300 rounded dark:bg-gray-700 dark:text-white focus:outline-none focus:border-blue-500"
            />
            {errors.email && <p className="text-red-500 text-xs mt-1">{errors.email}</p>}
          </div>

          <div className="space-y-1">
            <label htmlFor="password" className="block text-base">Password</label>
            <input
              id="password"
              type="text"
              placeholder="Enter your password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              className="mt-1 w-full px-3 py-2 border border-gray-300 rounded dark:bg-gray-700 dark:text-white focus:outline-none focus:border-blue-500"
            />
            {errors.password && <p className="text-red-500 text-xs mt-1">{errors.password}</p>}
          </div>

          <div className="space-y-1">
            <label htmlFor="confirmPassword" className="block text-base">Confirm Password</label>
            <input
              id="confirmPassword"
              type="password"
              placeholder="Enter your confirm password"
              value={confirmPassword}
              onChange={(e) => setConfirmPassword(e.target.value)}
              className="mt-1 w-full px-3 py-2 border border-gray-300 rounded dark:bg-gray-700 dark:text-white focus:outline-none focus:border-blue-500"
            />
            {errors.confirmPassword && <p className="text-red-500 text-xs mt-1">{errors.confirmPassword}</p>}
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
              I agree to the <a href="/terms" className="text-orange-500 hover:underline">Terms of Service</a> and <a href="/privacy" className="text-orange-500 hover:underline">Privacy Policy</a>
            </label>
          </div>
          {errors.terms && <p className="text-red-500 text-xs mt-1">{errors.terms}</p>}

          <button
            type="submit"
            className="w-full bg-orange-500 hover:bg-orange-600 text-white font-bold py-2 px-4 rounded transition-colors text-sm"
          >
            Continue to Profile Setup
          </button>

          <div className="text-center text-sm">
            <p>
              Already have an account? <a href="/login" className="text-orange-500 hover:underline">Sign in</a>
            </p>
          </div>
        </form>
      </div>
    </div>
  );
};

export default SignUpForm;
