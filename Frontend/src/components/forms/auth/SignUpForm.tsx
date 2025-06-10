"use client";
import React, { useState } from "react";
import { CheckCircleOutlined } from "@ant-design/icons";
import { Form, Input, Button, Checkbox } from "antd";
import { useNavigate } from "react-router-dom";
import type { SignUpReq } from "../../../models";
import authService from "../../../services/auth/authService";

const SignUpForm: React.FC = () => {
  const [form] = Form.useForm();
  const [showNotification, setShowNotification] = useState(false);
  const navigate = useNavigate();

  const validatePassword = (password: string) => {
    const lengthValid = password.length >= 8 && password.length <= 32;
    const containsMix =
      /[A-Za-z]/.test(password) &&
      /\d/.test(password) &&
      /[^A-Za-z0-9]/.test(password);
    const noSpaces = !/\s/.test(password);
    return lengthValid && containsMix && noSpaces;
  };

  const handleSubmit = async (values: {
    email: string;
    password: string;
    confirmPassword: string;
    agreeToTerms: boolean;
  }) => {
    const trimmedEmail = values.email.trim();

    const signUpData: SignUpReq = {
      email: trimmedEmail,
      password: values.password,
      confirmpassword: values.confirmPassword,
      roleId: 3,
    };

    try {
      const res = await authService.signUp(signUpData);
      console.log("Signup successful:", res);

      setShowNotification(true);

      setTimeout(() => {
        setShowNotification(false);
        navigate("/profile-setup", { state: { ...res } });
      }, 1000);
    } catch (err) {
      console.error("Signup failed:", err);
      form.setFields([{
        name: 'email',
        errors: ['Already have this email']
      }]);
    }
  };

  return (
    <div className="w-full max-w-md space-y-6 p-6 bg-gray-800 rounded shadow mx-auto text-sm text-gray-300">
      {showNotification && (
        <div className="fixed top-4 right-4 bg-green-100 border-l-4 border-green-500 text-green-700 p-4 rounded shadow-md transition-all duration-500 transform animate-fade-in flex items-center max-w-sm">
          <CheckCircleOutlined className="text-green-500 text-xl mr-2" />
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
        </div>        <Form
          form={form}
          onFinish={handleSubmit}
          layout="vertical"
          requiredMark={true}
          className="space-y-6"
        >
          <Form.Item
            name="email"
            label={<span className="text-base text-white">Email Address</span>}
            rules={[
              {
                required: true,
                message: "Please enter your email",
              },
              {
                type: "email",
                message: "Email must be in a correct format",
              },
              {
                max: 50,
                message: "Email should not exceed 50 characters",
              },
            ]}
          >
            <Input
              placeholder="Enter your email"
              className="mt-1 w-full px-3 py-2 border border-gray-300 rounded dark:bg-gray-700 dark:text-white focus:outline-none focus:border-blue-500"
              style={{
                backgroundColor: "#374151",
                borderColor: "#D1D5DB",
                color: "white",
              }}
            />
          </Form.Item>

          <Form.Item
            name="password"
            label={<span className="text-base text-white">Password</span>}
            rules={[
              {
                required: true,
                message: "Please enter your password",
              },
              {
                min: 8,
                max: 32,
                message: "Password should be in 8-32 characters",
              },
              {
                validator: (_, value) => {
                  if (value && !validatePassword(value)) {
                    if (/\s/.test(value)) {
                      return Promise.reject(new Error("Password should not include space characters"));
                    } else {
                      return Promise.reject(new Error("Password must include a mix of letters, numbers, symbols"));
                    }
                  }
                  return Promise.resolve();
                },
              },
            ]}
          >
            <Input.Password
              placeholder="Enter your password"
              className="mt-1 w-full px-3 py-2 border border-gray-300 rounded dark:bg-gray-700 dark:text-white focus:outline-none focus:border-blue-500"
              style={{
                backgroundColor: "#374151",
                borderColor: "#D1D5DB",
                color: "white",
              }}
            />
          </Form.Item>

          <Form.Item
            name="confirmPassword"
            label={<span className="text-base text-white">Confirm Password</span>}
            dependencies={['password']}
            rules={[
              {
                required: true,
                message: "Please enter your confirm password",
              },
              ({ getFieldValue }) => ({
                validator(_, value) {
                  if (!value || getFieldValue('password') === value) {
                    return Promise.resolve();
                  }
                  return Promise.reject(new Error('Confirm password is incorrect'));
                },
              }),
            ]}
          >
            <Input.Password
              placeholder="Enter your confirm password"
              className="mt-1 w-full px-3 py-2 border border-gray-300 rounded dark:bg-gray-700 dark:text-white focus:outline-none focus:border-blue-500"
              style={{
                backgroundColor: "#374151",
                borderColor: "#D1D5DB",
                color: "white",
              }}
            />
          </Form.Item>

          <Form.Item
            name="agreeToTerms"
            valuePropName="checked"
            rules={[
              {
                validator: (_, value) =>
                  value ? Promise.resolve() : Promise.reject(new Error('Please agree with Terms of Service and Privacy Policy')),
              },
            ]}
          >
            <Checkbox className="text-white">
              <span className="text-xs text-gray-300">
                I agree to the{" "}
                <a href="/terms" className="text-orange-500 hover:underline">
                  Terms of Service
                </a>{" "}
                and{" "}
                <a href="/privacy" className="text-orange-500 hover:underline">
                  Privacy Policy
                </a>
              </span>
            </Checkbox>
          </Form.Item>

          <Form.Item>
            <Button
              type="primary"
              htmlType="submit"
              className="w-full bg-orange-500 hover:bg-orange-600 text-white font-bold py-2 px-4 rounded transition-colors text-sm"
              style={{
                backgroundColor: "#F97316",
                borderColor: "#F97316",
                height: "40px",
              }}
            >
              Continue to Profile Setup
            </Button>
          </Form.Item>

          <div className="text-center text-sm">
            <p className="text-gray-300">
              Already have an account?{" "}
              <a href="/login" className="text-orange-500 hover:underline">
                Sign in
              </a>
            </p>
          </div>
        </Form>
      </div>
    </div>
  );
};

export default SignUpForm;
