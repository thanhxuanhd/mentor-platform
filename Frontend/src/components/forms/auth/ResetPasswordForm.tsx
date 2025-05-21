"use client"

import type React from "react"
import { useState } from "react"
import { EyeOutlined, EyeInvisibleOutlined, CheckCircleOutlined } from "@ant-design/icons"
import authService from "../../../services/auth/authService"
import type { ResetPasswordReq } from "../../../models"
import { useNavigate } from "react-router-dom";

const ResetPasswordForm: React.FC = () => {
  const [email, setEmail] = useState("")
  const [oldPassword, setOldPassword] = useState("") 
  const [newPassword, setNewPassword] = useState("")
  const [showPassword, setShowPassword] = useState(false)
  const [submitted, setSubmitted] = useState(false)
  const [showNotification, setShowNotification] = useState(false)
  const [isLoading, setIsLoading] = useState(false)
  const navigate = useNavigate();

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    if (isLoading) return
    setIsLoading(true)

    const data: ResetPasswordReq = { email, oldPassword, newPassword } 
    try {
      await authService.resetPassword(data)
      console.log("Reset password successful for:", email)

      setShowNotification(true)
      setTimeout(() => {
        setShowNotification(false)
        navigate("/login", { replace: true });
        setSubmitted(true)
      }, 3000)
    } catch (err) {
      console.error("Reset password failed:", err)
      alert("Reset password failed: incorrect email or old password.")
    }
  }

  const togglePasswordVisibility = () => {
    setShowPassword(!showPassword)
  }

  return (
    <>
      {showNotification && (
        <div className="fixed top-4 right-4 bg-green-100 border-l-4 border-green-500 text-green-700 p-4 rounded shadow-md transition-all duration-500 transform animate-fade-in flex items-center max-w-sm">
          <CheckCircleOutlined className="text-green-500 text-xl mr-2" />
          <div>
            <p className="font-bold">Success!</p>
            <p>Reset Password successfully!</p>
          </div>
        </div>
      )}

      <div className="w-full max-w-md mx-auto mt-10 bg-white dark:bg-gray-800 p-6 rounded shadow">
        <h2 className="text-2xl font-bold text-center text-gray-800 dark:text-white">
          Reset your password
        </h2>

        {submitted ? (
          <div className="mt-6 text-green-600 dark:text-green-400 text-center">
            Your password has been reset successfully.
          </div>
        ) : (
          <form onSubmit={handleSubmit} className="space-y-6 mt-6">
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
                placeholder="you@example.com"
              />
            </div>

            <div>
              <label htmlFor="oldPassword" className="block text-sm font-medium text-gray-700 dark:text-white">
                Current Password
              </label>
              <div className="relative">
                <input
                  id="oldPassword"
                  type={showPassword ? "text" : "password"}
                  value={oldPassword}
                  onChange={(e) => setOldPassword(e.target.value)}
                  required
                  className="mt-1 w-full px-3 py-2 border border-gray-300 rounded dark:bg-gray-700 dark:text-white"
                  placeholder="Enter your current password"
                />
              </div>
            </div>

            <div>
              <label htmlFor="newPassword" className="block text-sm font-medium text-gray-700 dark:text-white">
                New Password
              </label>
              <div className="relative">
                <input
                  id="newPassword"
                  type={showPassword ? "text" : "password"}
                  value={newPassword}
                  onChange={(e) => setNewPassword(e.target.value)}
                  required
                  className="mt-1 w-full px-3 py-2 border border-gray-300 rounded dark:bg-gray-700 dark:text-white"
                  placeholder="Enter your new password"
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

            <button
              type="submit"
              disabled={isLoading}
              className={`w-full text-white font-semibold py-2 rounded ${
                isLoading ? "bg-orange-400 cursor-not-allowed" : "bg-orange-600 hover:bg-orange-700"
              }`}
            >
              {isLoading ? "Processing..." : "Reset Password"}
            </button>

            <div className="text-center">
              <a href="/login" className="text-sm text-blue-600 hover:text-blue-800 dark:text-blue-400">
                Back to Sign In
              </a>
            </div>
          </form>
        )}
      </div>
    </>
  )
}

export default ResetPasswordForm
