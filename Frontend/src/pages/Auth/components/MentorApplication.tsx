import React, { useEffect, useContext, useState } from "react";
import {
  Form,
  Input,
  Upload,
  Button,
  notification,
} from "antd";
import { UploadOutlined } from "@ant-design/icons";
import type { UploadFile } from "antd/es/upload/interface";

import { userService } from "../../../services/user/userService";
import { AuthContext } from "../../../contexts/AuthContext";

const { TextArea } = Input;

interface MentorApplicationFormData {
  degree: string;
  university: string;
  year: string;
  experience: string;
  certifications?: string;
  motivation: string;
  documents?: UploadFile[];
}

interface UploadChangeParam {
  fileList: UploadFile[];
  file: UploadFile;
}

const MentorApplicationForm: React.FC = () => {
  const [form] = Form.useForm<MentorApplicationFormData>();
  const { user } = useContext(AuthContext);

  const [mentorInfo, setMentorInfo] = useState<{
    fullName?: string;
    email?: string;
    phoneNumber?: string;
    bio?: string;
    profilePhotoUrl?: string;
    roleId?: number;
  }>({});

  useEffect(() => {
    const fetchMentorInfo = async () => {
      try {
        const token = localStorage.getItem("token");
        if (!token || !user?.id) return;

        const userProfile = await userService.getUserProfile(user.id);
        if (userProfile) {
          setMentorInfo({
            fullName: userProfile.fullName,
            phoneNumber: userProfile.phoneNumber,
            bio: userProfile.bio ?? undefined,
            profilePhotoUrl: userProfile.profilePhotoUrl ?? undefined,
            roleId: userProfile.roleId ?? undefined,
          });
        }
      } catch (error) {
        console.error("Error fetching mentor profile:", error);
        notification.error({
          message: "Error",
          description: "Failed to load mentor information.",
        });
      }
    };

    fetchMentorInfo();
  }, [user]);

  const handleSubmit = (values: MentorApplicationFormData) => {
    console.log("Submitted values:", values);
    notification.success({
      message: "Application Submitted",
      description: "Thank you! We'll review your application soon.",
    });
    form.resetFields();
  };

  const normFile = (e: UploadChangeParam | UploadFile[]): UploadFile[] => {
    return Array.isArray(e) ? e : e?.fileList || [];
  };

  return (
    <div className="text-white p-6 rounded-xl max-w-3xl my-6 mx-auto shadow-lg bg-gray-800">
      <h1 className="text-2xl font-semibold mb-6">Mentor Application</h1>

<div className="mb-8 p-4 rounded-lg bg-gray-700 shadow flex flex-col sm:flex-row items-start sm:items-center justify-between gap-4">
  <div className="flex-shrink-0 w-24 h-24 rounded-full bg-gray-600 overflow-hidden border border-gray-500 shadow">
    {mentorInfo.profilePhotoUrl ? (
      <img
        src={mentorInfo.profilePhotoUrl}
        alt="Avatar"
        className="w-full h-full object-cover"
      />
    ) : (
      <div className="flex items-center justify-center h-full text-white">
        <span>Profile</span>
      </div>
    )}
  </div>

  <div className="flex-1 sm:ml-6 space-y-2 text-white">
    <div className="flex flex-col sm:flex-row sm:items-center sm:gap-3">
      <h2 className="text-2xl font-semibold">{mentorInfo.fullName || "Loading..."}</h2>
      {mentorInfo.roleId === 2 && (
        <span className="bg-orange-700 text-orange-200 text-sm px-3 py-1 rounded-md border border-orange-400">
          Mentor
        </span>
      )}
    </div>
    <p><strong>Phone:</strong> {mentorInfo.phoneNumber || "N/A"}</p>
    <p><strong>Bio:</strong> {mentorInfo.bio || "N/A"}</p>
  </div>
</div>


      <Form<MentorApplicationFormData>
        form={form}
        layout="vertical"
        name="mentor_application_form"
        onFinish={handleSubmit}
        requiredMark={false}
      >
        <Form.Item label="Education">
            <Form.Item
              name="education"
              noStyle
              rules={[{ required: true, message: "Education is required" }]}
            >
              <Input style={{ width: "100%" }} placeholder="Education" />
            </Form.Item>
        </Form.Item>

        <Form.Item
          name="experience"
          label="Work Experience"
          rules={[{ required: true, message: "Please enter your experience" }]}
        >
          <TextArea rows={3} placeholder="Describe your work experience..." />
        </Form.Item>

        <Form.Item
          name="certifications"
          label="Certifications"
        >
          <TextArea rows={2} placeholder="List your certifications (optional)" />
        </Form.Item>

        <Form.Item
          name="motivation"
          label="Motivation Statement"
          rules={[{ required: true, message: "Please write your motivation" }]}
        >
          <TextArea
            rows={4}
            placeholder="Tell us why you want to be a mentor and what you can offer."
          />
        </Form.Item>

        <Form.Item
          name="documents"
          label="Upload Supporting Documents"
          valuePropName="fileList"
          getValueFromEvent={normFile}
        >
          <Upload
            name="file"
            multiple
            maxCount={5}
            beforeUpload={() => false}
            accept=".pdf,.doc,.docx,.png,.jpg"
          >
            <Button icon={<UploadOutlined />}>Click to Upload</Button>
          </Upload>
        </Form.Item>

        <Form.Item>
          <Button type="primary" htmlType="submit" className="bg-orange-500">
            Submit
          </Button>
        </Form.Item>
      </Form>
    </div>
  );
};

export default MentorApplicationForm;
