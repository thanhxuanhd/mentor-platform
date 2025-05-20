"use client";
import {
  GoogleOutlined,
  GithubOutlined,
  CloseCircleOutlined,
  EyeOutlined,
  EyeInvisibleOutlined,
  CheckCircleOutlined,
} from "@ant-design/icons";
import type React from "react";
import { useState, useEffect } from "react";
import { useNavigate, Link } from "react-router-dom";
import type { LoginReq } from "../../../models";
import { redirectOAuthHandler } from "../../../utils/oAuth";
import { authService } from "../../../services/auth/authService";
import { useAuth } from "../../../hooks";

const encodePassword = (password: string): string => {
  const salt = "SECURITY_SALT";
  const encodedString = btoa(password + salt);
  return encodedString;
};

const decodePassword = (encodedPassword: string): string => {
  const salt = "SECURITY_SALT";
  const decodedString = atob(encodedPassword);
  return decodedString.replace(salt, "");
};

const LoginForm: React.FC = () => {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [rememberMe, setRememberMe] = useState(false);
  const [showPassword, setShowPassword] = useState(false);
  const [showSuccessNotification, setShowSuccessNotification] = useState(false);
  const [errorMessage, setErrorMessage] = useState("");
  const [fieldError, setFieldError] = useState<{ email?: string; password?: string }>({});
  const navigate = useNavigate();
  const { setToken } = useAuth();

  useEffect(() => {
    try {
      const remembermeInfoString = localStorage.getItem("RemembermeInfo");
      if (remembermeInfoString) {
        const remembermeInfo = JSON.parse(remembermeInfoString);
        const remembered = remembermeInfo.enabled || false;
        const savedEmail = remembered ? remembermeInfo.email || "" : "";
        const savedEncodedPassword = remembered ? remembermeInfo.password || "" : "";

        setEmail(savedEmail);
        setPassword(savedEncodedPassword ? decodePassword(savedEncodedPassword) : "");
        setRememberMe(remembered);
      }
    } catch (error) {
      console.error("Error retrieving remembered info:", error);
      localStorage.removeItem("RemembermeInfo");
    }
  }, []);

  const validateEmail = (email: string) => /\S+@\S+\.\S+/.test(email);

  const handleGoogleLogin = () => redirectOAuthHandler("google");
  const handleGithubLogin = () => redirectOAuthHandler("github");

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setErrorMessage("");
    setFieldError({});

    let hasError = false;
    const cleanedEmail = email.trim();
    const errors: { email?: string; password?: string } = {};

    if (!cleanedEmail) {
      errors.email = "Please enter your email";
      hasError = true;
    } else if (!validateEmail(cleanedEmail)) {
      errors.email = "Email must be in a correct format";
      hasError = true;
    } else if (cleanedEmail.length > 50) {
      errors.email = "Email must not exceed 50 characters";
      hasError = true;
    }
    if (!password.trim()) {
      errors.password = "Please enter your password";
      hasError = true;
    }
    setFieldError(errors);
    if (hasError) return;

    const loginData: LoginReq = { email: cleanedEmail, password };
    try {
      const res = await authService.login(loginData);
      console.log("Login successful:", res);

      if (rememberMe) {
        const remembermeInfo = {
          enabled: true,
          email: cleanedEmail,
          password: encodePassword(password),
        };
        localStorage.setItem("RemembermeInfo", JSON.stringify(remembermeInfo));
      } else {
        localStorage.removeItem("RemembermeInfo");
      }
      
      setShowSuccessNotification(true);
      setTimeout(() => {
        setShowSuccessNotification(false);
        setToken(res.value);
        navigate("/");
      }, 1000);
    } catch (err) {
      console.error("Login failed:", err);
      setErrorMessage("Email or password is not correct");
      setTimeout(() => {
        setErrorMessage("");
      }, 1000);
    }
  };

  const togglePasswordVisibility = () => {
    setShowPassword(!showPassword);
  };

  return (
    <>
      {showSuccessNotification && (
        <div
          role="alert"
          aria-live="assertive"
          className="fixed top-4 right-4 bg-green-100 border-l-4 border-green-500 text-green-700 p-4 rounded shadow-md transition-all duration-500 transform animate-fade-in flex items-center max-w-sm"
        >
          <CheckCircleOutlined className="text-green-500 text-xl mr-2" />
          <div>
            <p className="font-bold">Sign in successfully!</p>
          </div>
        </div>
      )}

      {errorMessage && (
        <div
          role="alert"
          aria-live="assertive"
          className="fixed top-6 left-1/2 transform -translate-x-1/2 z-50"
        >
          <div className="bg-red-100 border-l-4 border-red-500 text-red-700 p-4 rounded shadow-md transition-all duration-500 transform animate-fade-in flex items-center max-w-sm">
            <CloseCircleOutlined className="text-red-500 text-xl mr-2" />
            <div>
              <p className="font-bold">Error!</p>
              <p>{errorMessage}</p>
            </div>
          </div>
        </div>
      )}

      <form
        onSubmit={handleSubmit}
        noValidate
        className="w-full max-w-md space-y-6 bg-white dark:bg-gray-800 p-6 rounded shadow mx-auto"
      >
        <h2 className="text-2xl font-bold text-center text-gray-800 dark:text-white">
          Sign in to your account
        </h2>

        <div>
          <label htmlFor="email" className="block text-sm font-medium text-gray-700 dark:text-white">
            Email
          </label>
          <input
            id="email"
            type="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            placeholder="Enter your email"
            className="mt-1 w-full px-3 py-2 border border-gray-300 rounded dark:bg-gray-700 dark:text-white"
            required
          />
          {fieldError.email && <p className="text-red-500 text-sm mt-1">{fieldError.email}</p>}
        </div>

        <div>
          <label htmlFor="password" className="block text-sm font-medium text-gray-700 dark:text-white">
            Password
          </label>
          <div className="relative">
            <input
              id="password"
              type={showPassword ? "text" : "password"}
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              placeholder="Enter your password"
              className="mt-1 w-full px-3 py-2 border border-gray-300 rounded dark:bg-gray-700 dark:text-white"
              required
              onInput={(e) => e.currentTarget.setCustomValidity("")}
            />
            <button
              type="button"
              onClick={togglePasswordVisibility}
              aria-label={showPassword ? "Hide password" : "Show password"}
              className="absolute inset-y-0 right-0 pr-3 flex items-center text-gray-600 dark:text-gray-300"
            >
              {showPassword ? <EyeInvisibleOutlined /> : <EyeOutlined />}
            </button>
          </div>
          {fieldError.password && <p className="text-red-500 text-sm mt-1">{fieldError.password}</p>}
        </div>

        <div className="flex items-center justify-between">
          <label className="flex items-center text-sm text-gray-700 dark:text-white">
            <input
              type="checkbox"
              checked={rememberMe}
              onChange={(e) => setRememberMe(e.target.checked)}
              className="mr-2"
            />
            Remember me
          </label>
          <Link to="/forgot-password" className="text-sm text-blue-600 hover:text-blue-800 dark:text-blue-400">
            Forgot password?
          </Link>
        </div>
        <button
          type="submit"
          className="w-full bg-orange-500 hover:bg-orange-600 text-white font-semibold py-2 rounded"
        >
          Sign In
        </button>
        <div className="text-center">
          <p className="text-sm text-gray-600 dark:text-gray-300">or continue with</p>
          <div className="mt-3 flex justify-center gap-4">
            <button
              type="button"
              onClick={handleGoogleLogin}
              className="flex items-center gap-2 px-4 py-2 border border-gray-300 rounded hover:bg-gray-100 dark:border-gray-600 dark:hover:bg-gray-700"
            >
              <GoogleOutlined />
              Google
            </button>
            <button
              type="button"
              onClick={handleGithubLogin}
              className="flex items-center gap-2 px-4 py-2 border border-gray-300 rounded hover:bg-gray-100 dark:border-gray-600 dark:hover:bg-gray-700"
            >
              <GithubOutlined />
              GitHub
            </button>
          </div>
        </div>

        <div className="text-center text-sm">
            <p>
              Don't have an account? <a href="/signup" className="text-orange-500 hover:underline">Sign up</a>
            </p>
          </div>

        <div className="text-center mt-4">
          <a
            href="/terms"
            target="_blank"
            rel="noopener noreferrer"
            className="text-xs text-gray-500 hover:text-blue-600 underline"
          >
            Terms of Service and Privacy Policy
          </a>
        </div>
      </form>
    </>
  );
};

export default LoginForm;