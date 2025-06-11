"use client";

import type React from "react";
import { useState } from "react";
import {
  EyeOutlined,
  EyeInvisibleOutlined,
  CheckCircleOutlined,
} from "@ant-design/icons";
import { Form, Input, Button } from "antd";
import authService from "../../../services/auth/authService";
import type { ResetPasswordReq } from "../../../models";
import { useNavigate } from "react-router-dom";

const ResetPasswordForm: React.FC = () => {
  const [form] = Form.useForm();
  const [showPassword, setShowPassword] = useState(false);
  const [submitted, setSubmitted] = useState(false);
  const [showNotification, setShowNotification] = useState(false);
  const [isLoading, setIsLoading] = useState(false);

  const navigate = useNavigate();

  const handleSubmit = async (values: {
    email: string;
    oldPassword: string;
    newPassword: string;
  }) => {
    const trimmedEmail = values.email.trim();

    setIsLoading(true);

    const data: ResetPasswordReq = {
      email: trimmedEmail,
      oldPassword: values.oldPassword,
      newPassword: values.newPassword,
    };

    try {
      await authService.resetPassword(data);
      console.log("Reset password successful for:", trimmedEmail);
      setShowNotification(true);
      setTimeout(() => {
        setShowNotification(false);
        navigate("/login", { replace: true });
        setSubmitted(true);
      }, 3000);
    } catch (err: any) {
      console.error("Reset password failed:", err);
      if (err?.response?.status === 400) {
        form.setFields([{
          name: 'oldPassword',
          errors: ['Current password is incorrect.']
        }]);
      } else if (err?.response?.status === 404) {
        form.setFields([{
          name: 'email',
          errors: ['Email not found.']
        }]);
      } else {
        alert("Reset password failed: an unexpected error occurred.");
      }    } finally {
      setIsLoading(false);
    }
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
        </h2>        {submitted ? (
          <div className="mt-6 text-green-600 dark:text-green-400 text-center">
            Your password has been reset successfully.
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

            <Form.Item
              name="oldPassword"
              label={<span className="text-sm font-medium text-white">Current Password</span>}
              rules={[
                {
                  required: true,
                  message: "Please enter your current password",
                },
              ]}
            >
              <Input.Password
                placeholder="Enter your current password"
                className="mt-1 w-full px-3 py-2 border rounded bg-gray-700 text-white"
                visibilityToggle={{
                  visible: showPassword,
                  onVisibleChange: setShowPassword,
                }}
                iconRender={(visible) =>
                  visible ? <EyeInvisibleOutlined /> : <EyeOutlined />
                }
                style={{
                  backgroundColor: "#374151",
                  borderColor: "#6B7280",
                  color: "white",
                }}
              />
            </Form.Item>

            <Form.Item
              name="newPassword"
              label={<span className="text-sm font-medium text-white">New Password</span>}
              rules={[
                {
                  required: true,
                  message: "Please enter your new password",
                },
                {
                  min: 8,
                  max: 32,
                  message: "Password must be between 8 and 32 characters",
                },
                {
                  pattern: /(?=.*[a-zA-Z])(?=.*\d)(?=.*[!@#$%^&*()_+{}[\]:;<>,.?~\\/-])/,
                  message: "Password must include letters, numbers, and special characters",
                },
              ]}
            >
              <Input.Password
                placeholder="Enter your new password"
                className="mt-1 w-full px-3 py-2 border rounded bg-gray-700 text-white"
                visibilityToggle={{
                  visible: showPassword,
                  onVisibleChange: setShowPassword,
                }}
                iconRender={(visible) =>
                  visible ? <EyeInvisibleOutlined /> : <EyeOutlined />
                }
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
                {isLoading ? "Processing..." : "Reset Password"}
              </Button>
            </Form.Item>

            <div className="text-center">
              <a
                href="/login"
                className="text-sm text-blue-600 hover:text-blue-800 dark:text-blue-400"
              >
                Back to Sign In
              </a>
            </div>
          </Form>
        )}
      </div>
    </>
  );
};

export default ResetPasswordForm;
