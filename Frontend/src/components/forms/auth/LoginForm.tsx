"use client"
import { GoogleOutlined, GithubOutlined, LinkedinOutlined, EyeOutlined, EyeInvisibleOutlined, CheckCircleOutlined, CloseCircleOutlined } from "@ant-design/icons"
import type React from "react"
import { useState, useEffect } from "react"
import { useNavigate } from "react-router-dom"
import type { LoginReq } from "../../../models"
import authService from "../../../services/auth/authService"

const LoginForm: React.FC = () => {
  const [email, setEmail] = useState("")
  const [password, setPassword] = useState("")
  const [rememberMe, setRememberMe] = useState(false)
  const [showPassword, setShowPassword] = useState(false)
  const [showSuccessNotification, setShowSuccessNotification] = useState(false)
  const [errorMessage, setErrorMessage] = useState("")
  const [fieldError, setFieldError] = useState<{ email?: string; password?: string }>({})
  const navigate = useNavigate()

  useEffect(() => {
    const remembered = localStorage.getItem("rememberMe") === "true"
    const savedEmail = remembered ? localStorage.getItem("email") || "" : ""
    setEmail(savedEmail)
    setRememberMe(remembered)
  }, [])

  const validateEmail = (email: string) => {
    return /\S+@\S+\.\S+/.test(email)
  }

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    setErrorMessage("")
    setFieldError({})

    let hasError = false
    const errors: { email?: string; password?: string } = {}

    if (!email.trim()) {
      errors.email = "Please enter your email"
      hasError = true
    } else if (!validateEmail(email)) {
      errors.email = "Email must be in a correct format"
      hasError = true
    }

    if (!password.trim()) {
      errors.password = "Please enter your password"
      hasError = true
    }

    setFieldError(errors)
    if (hasError) return

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

      setShowSuccessNotification(true)
      setTimeout(() => {
        setShowSuccessNotification(false)
        navigate("/")
      }, 1000)
    } catch (err) {
      console.error("Login failed:", err)
      setErrorMessage("Email or password is not correct")
      setTimeout(() => {
        setErrorMessage("")
      }, 1000)
    }
  }

  const togglePasswordVisibility = () => {
    setShowPassword(!showPassword)
  }

  return (
    <>
      {showSuccessNotification && (
        <div className="fixed top-4 right-4 bg-green-100 border-l-4 border-green-500 text-green-700 p-4 rounded shadow-md transition-all duration-500 transform animate-fade-in flex items-center max-w-sm">
          <CheckCircleOutlined className="text-green-500 text-xl mr-2" />
          <div>
            <p className="font-bold">Sign in successfully!</p>
          </div>
        </div>
      )}

      {errorMessage && (
        <div className="fixed top-6 left-1/2 transform -translate-x-1/2 z-50">
          <div className="bg-red-100 border-l-4 border-red-500 text-red-700 p-4 rounded shadow-md transition-all duration-500 transform animate-fade-in flex items-center max-w-sm">
            <CloseCircleOutlined className="text-red-500 text-xl mr-2" />
            <div>
              <p className="font-bold">Error!</p>
              <p>{errorMessage}</p>
            </div>
          </div>
        </div>
      )}


      <form onSubmit={handleSubmit} className="w-full max-w-md space-y-6 bg-white dark:bg-gray-800 p-6 rounded shadow">
        <h2 className="text-2xl font-bold text-center text-gray-800 dark:text-white">Sign in to your account</h2>

        <div>
          <label htmlFor="email" className="block text-sm font-medium text-gray-700 dark:text-white">Email</label>
          <input
            id="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            placeholder="Enter your email"
            className="mt-1 w-full px-3 py-2 border border-gray-300 rounded dark:bg-gray-700 dark:text-white"
          />
          {fieldError.email && (
            <p className="text-red-500 text-sm mt-1">{fieldError.email}</p>
          )}
        </div>

        <div>
          <label htmlFor="password" className="block text-sm font-medium text-gray-700 dark:text-white">Password</label>
          <div className="relative">
            <input
              id="password"
              type={showPassword ? "text" : "password"}
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              placeholder="Enter your password"
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
          {fieldError.password && (
            <p className="text-red-500 text-sm mt-1">{fieldError.password}</p>
          )}
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
            <button type="button" className="flex items-center gap-2 px-4 py-2 border border-gray-300 rounded hover:bg-gray-100 dark:border-gray-600 dark:hover:bg-gray-700">
              <GoogleOutlined /> Google
            </button>
            <button type="button" className="flex items-center gap-2 px-4 py-2 border border-gray-300 rounded hover:bg-gray-100 dark:border-gray-600 dark:hover:bg-gray-700">
              <GithubOutlined /> GitHub
            </button>
            <button type="button" className="flex items-center gap-2 px-4 py-2 border border-gray-300 rounded hover:bg-gray-100 dark:border-gray-600 dark:hover:bg-gray-700">
              <LinkedinOutlined /> LinkedIn
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
