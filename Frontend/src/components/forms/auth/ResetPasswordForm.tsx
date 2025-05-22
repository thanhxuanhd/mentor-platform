"use client";

import type React from "react";
import { useState } from "react";
import {
  EyeOutlined,
  EyeInvisibleOutlined,
  CheckCircleOutlined,
} from "@ant-design/icons";
import authService from "../../../services/auth/authService";
import type { ResetPasswordReq } from "../../../models";
import { useNavigate } from "react-router-dom";

const ResetPasswordForm: React.FC = () => {
  const [email, setEmail] = useState("");
  const [oldPassword, setOldPassword] = useState("");
  const [newPassword, setNewPassword] = useState("");
  const [showPassword, setShowPassword] = useState(false);
  const [submitted, setSubmitted] = useState(false);
  const [showNotification, setShowNotification] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const [fieldError, setFieldError] = useState<{
    email?: string;
    oldPassword?: string;
    newPassword?: string;
  }>({});

  const navigate = useNavigate();
  const validateEmail = (email: string) => /\S+@\S+\.\S+/.test(email);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    const trimmedEmail = email.trim();
    const errors: {
      email?: string;
      oldPassword?: string;
      newPassword?: string;
    } = {};

    if (!trimmedEmail) {
      errors.email = "Please enter your email";
    } else if (!validateEmail(trimmedEmail)) {
      errors.email = "Email must be in a correct format";
    }

    if (!oldPassword.trim()) {
      errors.oldPassword = "Please enter your current password";
    }

    if (!newPassword.trim()) {
      errors.newPassword = "Please enter your new password";
    }
    if (!newPassword.trim()) {
      errors.newPassword = "Please enter your new password";
    } else if (newPassword.length < 8 || newPassword.length > 32) {
      errors.newPassword = "Password must be between 8 and 32 characters";
    } else if (
      !/(?=.*[a-zA-Z])/.test(newPassword) || // letters
      !/(?=.*\d)/.test(newPassword) || // digits
      !/(?=.*[!@#$%^&*()_+{}[\]:;<>,.?~\\/-])/.test(newPassword) // special chars
    ) {
      errors.newPassword =
        "Password must include letters, numbers, and special characters";
    }
    setFieldError(errors);

    if (Object.keys(errors).length > 0) {
      setIsLoading(false);
      return;
    }

    setIsLoading(true);

    const data: ResetPasswordReq = {
      email: trimmedEmail,
      oldPassword,
      newPassword,
    };

    try {
      await authService.resetPassword(data);
      console.log("Reset password successful for:", email);
      setShowNotification(true);
      setTimeout(() => {
        setShowNotification(false);
        navigate("/login", { replace: true });
        setSubmitted(true);
      }, 3000);
    } catch (err: any) {
      console.error("Reset password failed:", err);
      const newErrors: typeof fieldError = {};
      if (err?.response?.status === 400) {
        newErrors.oldPassword = "Current password is incorrect.";
      } else if (err?.response?.status === 404) {
        newErrors.email = "Email not found.";
      } else {
        alert("Reset password failed: an unexpected error occurred.");
      }

      setFieldError(newErrors);
    } finally {
      setIsLoading(false);
    }
  };

  const togglePasswordVisibility = () => {
    setShowPassword(!showPassword);
  };

  return (
    <>
      {showNotification && (
        <div className="fixed top-4 right-4 bg-green-100 border-l-4 border-green-500 text-green-700 p-4 rounded shadow-md transition-all duration-500 transform animate-fade-in flex items-center max-w-sm">
          <CheckCircleOutlined className="text-green-500 text-xl mr-2" />
          <div>
            <p className="font-bold">Success!</p>
            <p>Reset password sucessfully, please sign in again!</p>
          </div>
        </div>
      )}

      <div className="w-full max-w-md mx-auto mt-10 p-6 rounded shadow bg-gray-800">
        <h2 className="text-2xl font-bold text-center text-white">
          Reset your password
        </h2>

        {submitted ? (
          <div className="mt-6 text-green-600 dark:text-green-400 text-center">
            Your password has been reset successfully.
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
                className={`mt-1 w-full px-3 py-2 border rounded dark:bg-gray-700 dark:text-white ${
                  fieldError.email ? "border-red-500" : "border-gray-300"
                }`}
                placeholder="you@example.com"
              />
              {fieldError.email && (
                <p className="text-red-500 text-sm mt-1">{fieldError.email}</p>
              )}
            </div>

            <div>
              <label
                htmlFor="oldPassword"
                className="block text-sm font-medium text-white"
              >
                Current Password
              </label>
              <div className="relative">
                <input
                  id="oldPassword"
                  type={showPassword ? "text" : "password"}
                  value={oldPassword}
                  onChange={(e) => setOldPassword(e.target.value)}
                  className={`mt-1 w-full px-3 py-2 border rounded dark:bg-gray-700 dark:text-white ${
                    fieldError.oldPassword
                      ? "border-red-500"
                      : "border-gray-300"
                  }`}
                  placeholder="Enter your current password"
                />
              </div>
              {fieldError.oldPassword && (
                <p className="text-red-500 text-sm mt-1">
                  {fieldError.oldPassword}
                </p>
              )}
            </div>

            <div>
              <label
                htmlFor="newPassword"
                className="block text-sm font-medium text-white"
              >
                New Password
              </label>
              <div className="relative">
                <input
                  id="newPassword"
                  type={showPassword ? "text" : "password"}
                  value={newPassword}
                  onChange={(e) => setNewPassword(e.target.value)}
                  className={`mt-1 w-full px-3 py-2 border rounded dark:bg-gray-700 dark:text-white ${
                    fieldError.newPassword
                      ? "border-red-500"
                      : "border-gray-300"
                  }`}
                  placeholder="Enter your new password"
                />
                <button
                  type="button"
                  onClick={togglePasswordVisibility}
                  className="absolute inset-y-0 right-0 pr-3 flex items-center text-gray-300"
                >
                  {showPassword ? <EyeInvisibleOutlined /> : <EyeOutlined />}
                </button>
              </div>
              {fieldError.newPassword && (
                <p className="text-red-500 text-sm mt-1">
                  {fieldError.newPassword}
                </p>
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
              {isLoading ? "Processing..." : "Reset Password"}
            </button>

            <div className="text-center">
              <a
                href="/login"
                className="text-sm text-blue-600 hover:text-blue-800 dark:text-blue-400"
              >
                Back to Sign In
              </a>
            </div>
          </form>
        )}
      </div>
    </>
  );
};

export default ResetPasswordForm;
