"use client"
import { GoogleOutlined, GithubOutlined, LinkedinOutlined, EyeOutlined, EyeInvisibleOutlined, CheckCircleOutlined } from "@ant-design/icons"
import type React from "react"
import { useState, useEffect } from "react"
import { useNavigate } from "react-router-dom";
import type { LoginReq } from "../../../models"
import authService from "../../../services/auth/authService"

const LoginForm: React.FC = () => {
  const [email, setEmail] = useState("")
  const [password, setPassword] = useState("")
  const [rememberMe, setRememberMe] = useState(false)
  const [showPassword, setShowPassword] = useState(false)
  const [showNotification, setShowNotification] = useState(false)
  const navigate = useNavigate();

  useEffect(() => {
    const remembered = localStorage.getItem("rememberMe") === "true"
    const savedEmail = remembered ? localStorage.getItem("email") || "" : ""
    setEmail(savedEmail)
    setRememberMe(remembered)
  }, [])

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    
    const loginData: LoginReq = { email, password }
    try {
      const res = await authService.login(loginData)
      console.log("Login successful:", res)
      if (rememberMe) {
        localStorage.setItem("rememberMe", "true")
        localStorage.setItem("email", email)
      } else {
        localStorage.removeItem("rememberMe")
        localStorage.removeItem("email")
      }
      
      setShowNotification(true)
      
      setTimeout(() => {
        setShowNotification(false)
        navigate("/");
      }, 1000)
    } catch (err) {
      console.error("Login failed:", err)
      alert("Account does not exist.");
    }
  }

  const togglePasswordVisibility = () => {
    setShowPassword(!showPassword);
  }

  return (
    <>
      {showNotification && (
        <div className="fixed top-4 right-4 bg-green-100 border-l-4 border-green-500 text-green-700 p-4 rounded shadow-md transition-all duration-500 transform animate-fade-in flex items-center max-w-sm">
          <CheckCircleOutlined className="text-green-500 text-xl mr-2" />
          <div>
            <p className="font-bold">Success!</p>
            <p>You have successfully logged in.</p>
          </div>
        </div>
      )}
      
      <form onSubmit={handleSubmit} className="w-full max-w-md space-y-6 bg-white dark:bg-gray-800 p-6 rounded shadow">
        <h2 className="text-2xl font-bold text-center text-gray-800 dark:text-white">Sign in to your account</h2>

        <div>
          <label htmlFor="email" className="block text-sm font-medium text-gray-700 dark:text-white">
            Email
          </label>
          <input
            id="email"
            type="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            required
            className="mt-1 w-full px-3 py-2 border border-gray-300 rounded dark:bg-gray-700 dark:text-white"
          />
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
              required
              minLength={8}
              onInvalid={(e) => (e.currentTarget.setCustomValidity("Password must be at least 8 characters."))}
              onInput={(e) => e.currentTarget.setCustomValidity("")}
              className="mt-1 w-full px-3 py-2 border border-gray-300 rounded dark:bg-gray-700 dark:text-white"
            />
            <button
              type="button"
              onClick={togglePasswordVisibility}
              className="absolute inset-y-0 right-0 pr-3 flex items-center text-gray-600 dark:text-gray-300"
            >
              {showPassword ? <EyeInvisibleOutlined /> : <EyeOutlined />}
            </button>
          </div>
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
          <a href="/reset-password" className="text-sm text-blue-600 hover:text-blue-800 dark:text-blue-400">
            Forgot password?
          </a>
        </div>

        <button type="submit" className="w-full bg-green-600 hover:bg-green-700 text-white font-semibold py-2 rounded">
          Sign In
        </button>

        <div className="text-center">
          <p className="text-sm text-gray-600 dark:text-gray-300">or continue with</p>
          <div className="mt-3 flex justify-center gap-4">
            <button
              type="button"
              className="flex items-center gap-2 px-4 py-2 border border-gray-300 rounded hover:bg-gray-100 dark:border-gray-600 dark:hover:bg-gray-700"
            >
              <GoogleOutlined />
              Google
            </button>
            <button
              type="button"
              className="flex items-center gap-2 px-4 py-2 border border-gray-300 rounded hover:bg-gray-100 dark:border-gray-600 dark:hover:bg-gray-700"
            >
              <GithubOutlined />
              GitHub
            </button>
            <button
              type="button"
              className="flex items-center gap-2 px-4 py-2 border border-gray-300 rounded hover:bg-gray-100 dark:border-gray-600 dark:hover:bg-gray-700"
            >
              <LinkedinOutlined />
              LinkedIn
            </button>
          </div>
        </div>

        <div className="text-center">
          <a href="/signup" className="text-sm text-blue-600 hover:text-blue-800 dark:text-blue-400">
            Don't have an account? Sign up
          </a>
        </div>
      </form>
      
    </>
  )
}

export default LoginForm