import React, { useEffect, useContext, useState } from "react";
import { Form, Input, Upload, Button, App, Progress } from "antd";
import type { RcFile } from "antd/es/upload/interface";

import { userService } from "../../../services/user/userService";
import { AuthContext } from "../../../contexts/AuthContext";
import type { NotificationProps } from "../../../types/Notification";
import type { MentorApplicationType } from "../../../types/MentorApplicationType";
import { postMentorSubmission } from "../../../services/mentor/mentorServices";

const { TextArea } = Input;

const MentorApplicationForm: React.FC = () => {
  const [form] = Form.useForm<MentorApplicationType>();
  const [notify, setNotify] = useState<NotificationProps | null>(null);
  const { user } = useContext(AuthContext);
  const { notification } = App.useApp();

  const [mentorInfo, setMentorInfo] = useState<{
    fullName?: string;
    email?: string;
    phoneNumber?: string;
    profilePhotoUrl?: string;
    experiences?: string;
  }>({});

  const [progress, setProgress] = useState(0);

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
            profilePhotoUrl: userProfile.profilePhotoUrl ?? "",
            experiences: userProfile.experiences ?? "",
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
  }, [user, notification]);

  useEffect(() => {
    if (mentorInfo.experiences !== undefined) {
      form.setFieldsValue({ workExperience: mentorInfo.experiences });
    }
  }, [mentorInfo.experiences, form]);

  useEffect(() => {
    if (notify) {
      notification[notify.type]({
        message: notify.message,
        description: notify.description,
        placement: "topRight",
        showProgress: true,
        duration: 3,
        pauseOnHover: true,
      });
      setNotify(null);
    }
  }, [notify, notification]);

  const handleSubmit = async (values: MentorApplicationType) => {
    console.log("Submitted values:", values);
    const formData = new FormData();
    formData.append("education", values.education || "");
    formData.append("workExperience", values.workExperience || "");
    formData.append("certifications", values.certifications || "");
    formData.append("statement", values.statement || "");

    if (values.documents && Array.isArray(values.documents)) {
      values.documents.forEach((fileObj: any) => {
        if (fileObj.originFileObj) {
          formData.append("documents", fileObj.originFileObj);
        }
      });
    }

    await postMentorSubmission(formData);
    setNotify({
      type: "success",
      message: "Application Submitted",
      description: "Thank you! We'll review your application soon.",
    });
    form.resetFields();
    setProgress(0); // Reset progress after submission
  };

  const beforeUpload = (file: RcFile) => {
    const isLt1M = file.size / 1024 / 1024 < 1;
    if (!isLt1M) {
      setNotify({
        type: "error",
        message: "Error",
        description: "Image must smaller than 1MB!",
      });
    }

    return isLt1M ? false : Upload.LIST_IGNORE;
  };

  const handleFieldChange = async () => {
    let currentProgress = mentorInfo.experiences !== "" ? 25 : 0;
    const values = form.getFieldsValue();

    try {
      await form.validateFields(["education"]);
      if (values.education && values.education !== "") {
        currentProgress += 25;
      }
    } catch (errorInfo) {
      console.log("Invalid:", errorInfo);
    }

    try {
      await form.validateFields(["workExperience"]);
      if (values.workExperience && values.workExperience !== "") {
        currentProgress += 25;
      }
    } catch (errorInfo) {
      console.log("Invalid:", errorInfo);
    }

    try {
      await form.validateFields(["certifications"]);
      if (values.certifications && values.certifications !== "") {
        currentProgress += 25;
      }
    } catch (errorInfo) {
      console.log("Invalid:", errorInfo);
    }

    try {
      await form.validateFields(["statement"]);
      if (values.statement && values.statement !== "") {
        currentProgress += 25;
      }
    } catch (errorInfo) {
      console.log("Invalid:", errorInfo);
    }

    setProgress(currentProgress);
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
            <h2 className="text-2xl font-semibold">
              {mentorInfo.fullName || "Loading..."}
            </h2>
          </div>
          <p>
            <strong>Phone:</strong> {mentorInfo.phoneNumber || "N/A"}
          </p>
        </div>
      </div>

      <Progress percent={progress} status="active" className="mb-4" />

      <Form<MentorApplicationType>
        form={form}
        layout="vertical"
        name="mentor_application_form"
        onFinish={handleSubmit}
        requiredMark={false}
        onValuesChange={handleFieldChange}
      >
        <Form.Item label="Education">
          <Form.Item
            name="education"
            noStyle
            rules={[
              {
                max: 300,
                message: "Education must be less than 300 characters.",
              },
            ]}
          >
            <Input style={{ width: "100%" }} placeholder="Education" />
          </Form.Item>
        </Form.Item>

        <Form.Item
          name="workExperience"
          label="Work Experience"
          rules={[
            {
              max: 300,
              message: "Work Experience must be less than 300 characters.",
            },
          ]}
        >
          <TextArea rows={3} placeholder="Describe your work experience..." />
        </Form.Item>

        <Form.Item
          name="certifications"
          label="Certifications"
          rules={[
            {
              max: 300,
              message: "Certifications must be less than 300 characters.",
            },
          ]}
        >
          <TextArea
            rows={2}
            placeholder="List your certifications (optional)"
          />
        </Form.Item>

        <Form.Item
          name="statement"
          label="Motivation Statement"
          rules={[
            {
              max: 300,
              message: "Motivation Statement must be less than 300 characters.",
            },
          ]}
        >
          <TextArea
            rows={4}
            placeholder="Tell us why you want to be a mentor and what you can offer."
          />
        </Form.Item>

        <Form.Item
          name="documents"
          label="Upload Supporting Documents (max 5)"
          valuePropName="fileList"
          getValueFromEvent={(e) => {
            if (Array.isArray(e)) {
              return e;
            }
            return e && e.fileList;
          }}
        >
          <Upload
            name="file"
            listType="picture-card"
            className="document-uploader"
            multiple
            maxCount={5}
            beforeUpload={beforeUpload}
            accept=".pdf,.png,.jpg,.jpeg,.mp4,.avi,.mpeg,.mp3,.wav,.aac"
          >
            <button style={{ border: 0, background: "none" }} type="button">
              Upload
            </button>
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
