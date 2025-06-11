"use client";

import type React from "react";
import { useState } from "react";
import { Form, Input, Button } from "antd";
import { userService } from "../../../services/user/userService";
import { HiExclamationCircle } from "react-icons/hi";
import { useNavigate } from "react-router-dom";

const ForgotPasswordForm: React.FC = () => {
  const [form] = Form.useForm();
  const [submitted, setSubmitted] = useState(false);
  const [error, setError] = useState("");
  const [isLoading, setIsLoading] = useState(false);
  const navigate = useNavigate();

  const handleSubmit = async (values: { email: string }) => {
    const trimmedEmail = values.email.trim();

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
      </h2>      {error && (
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
        <Form
          form={form}
          onFinish={handleSubmit}
          layout="vertical"
          requiredMark={true}
          className="space-y-6 mt-6"
        >
          <Form.Item
            name="email"
            label={<span className="text-sm font-medium text-white">Email</span>}
            rules={[
              {
                required: true,
                message: "Please enter your email",
              },
              {
                type: "email",
                message: "Email must be in a correct format",
              },
            ]}
          >
            <Input
              placeholder="you@example.com"
              className="mt-1 w-full px-3 py-2 border rounded bg-gray-700 text-white"
              style={{
                backgroundColor: "#374151",
                borderColor: "#6B7280",
                color: "white",
              }}
            />
          </Form.Item>

          <Form.Item>
            <Button
              type="primary"
              htmlType="submit"
              loading={isLoading}
              className={`w-full text-white font-semibold py-2 rounded ${
                isLoading
                  ? "bg-orange-400 cursor-not-allowed"
                  : "bg-orange-600 hover:bg-orange-700"
              }`}
              style={{
                backgroundColor: isLoading ? "#FB923C" : "#EA580C",
                borderColor: isLoading ? "#FB923C" : "#EA580C",
                height: "40px",
              }}
            >
              {isLoading ? "Sending..." : "Send New Password"}
            </Button>
          </Form.Item>
        </Form>
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
