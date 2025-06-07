import React, { useEffect, useState } from "react";
import { Form, Input, Upload, Button, App, Progress } from "antd";
import type { RcFile, UploadFile } from "antd/es/upload/interface";
import { userService } from "../../../services/user/userService";
import type { NotificationProps } from "../../../types/Notification";
import type {
  MentorApplicationType,
  MentorApplicationDetailItemProp,
} from "../../../types/MentorApplicationType";
import { useAuth } from "../../../hooks";
import { normalizeServerFiles } from "../../../utils/InputNormalizer";
import { mentorApplicationService } from "../../../services/mentorAppplications/mentorApplicationService";
import { useNavigate } from "react-router-dom";

const { TextArea } = Input;

interface MentorApplicationFormProps {
  isEditMode?: boolean;
  application?: MentorApplicationDetailItemProp;
}

const MentorApplicationForm: React.FC<MentorApplicationFormProps> = ({
  isEditMode = false,
  application,
}) => {
  const [form] = Form.useForm<MentorApplicationType>();
  const [notify, setNotify] = useState<NotificationProps | null>(null);
  const { user } = useAuth();
  const { notification } = App.useApp();
  const [uploadedFileNames, setUploadedFileNames] = useState<string[]>([]);
  const [fileCount, setFileCount] = useState(0);

  const [mentorInfo, setMentorInfo] = useState<{
    fullName?: string;
    email?: string;
    phoneNumber?: string;
    profilePhotoUrl?: string;
    experiences?: string;
  }>({});
  const [progress, setProgress] = useState(0);
  const navigate = useNavigate();

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

        if (userProfile.experiences) {
          form.setFieldsValue({ workExperience: userProfile.experiences });
          setProgress(25);
        }
      } catch {
        setNotify({
          type: "error",
          message: "Error",
          description: "Failed to fetch availabilities",
        });
      }
    };

    fetchMentorInfo();
  }, [user, notification, form]);

  useEffect(() => {
    if (isEditMode && application) {
      form.setFieldsValue({
        education: application.education || "",
        workExperience: application.experiences || "",
        certifications: application.certifications || "",
        statement: application.statement || "",
        documents:
          application.documents?.map((doc) => ({
            uid: doc.documentId,
            name: normalizeServerFiles(doc.documentUrl),
            status: "done",
            url: doc.documentUrl,
          })) || [],
      });

      handleFieldChange();
    } else if (mentorInfo.experiences !== undefined) {
      form.setFieldsValue({ workExperience: mentorInfo.experiences });
    }
  }, [mentorInfo, application, isEditMode, form]);

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

    try {
      if (isEditMode && application?.mentorApplicationId) {
        await mentorApplicationService.editMentorApplication(
          application.mentorApplicationId,
          formData,
        );
        setNotify({
          type: "success",
          message: "Application Updated",
          description: "Your application has been successfully updated.",
        });
      } else {
        await mentorApplicationService.postMentorSubmission(formData);
        setNotify({
          type: "success",
          message: "Application Submitted",
          description: "Thank you! We will review your application soon.",
        });
      }
      form.resetFields();
      setProgress(0);
      navigate(-1);
    } catch (error: any) {
      setNotify({
        type: "error",
        message: "Error",
        description:
          error.response?.data?.error ||
          (isEditMode
            ? "Failed to update application."
            : "Failed to submit application."),
      });
    }
  };

  const beforeUpload = (file: RcFile) => {
    if (uploadedFileNames.includes(file.name)) {
      setNotify({
        type: "error",
        message: "Error",
        description: "File name already exists. Please rename your file.",
      });
      return Upload.LIST_IGNORE;
    }
    const isLt1M = file.size / 1024 / 1024 < 1;
    if (!isLt1M) {
      setNotify({
        type: "error",
        message: "Error",
        description: "File size must smaller than 1MB!",
      });
    }

    return isLt1M ? false : Upload.LIST_IGNORE;
  };

  const handleFieldChange = async () => {
    let currentProgress = 0;
    const values = form.getFieldsValue();

    if (values.workExperience) {
      currentProgress += 25;
    }

    if (values.education) {
      currentProgress += 25;
    }

    if (values.certifications) {
      currentProgress += 25;
    }

    if (values.statement) {
      currentProgress += 25;
    }

    setProgress(currentProgress);
  };

  const handleChange = ({ fileList }: { fileList: UploadFile[] }) => {
    setFileCount(fileList.length);
    setUploadedFileNames(fileList.map((f) => f.name));
  };

  return (
    <div className="text-white p-6 rounded-xl max-w-3xl my-6 mx-auto shadow-lg bg-gray-800">
      <h1 className="text-2xl font-semibold mb-6">
        {isEditMode ? "Edit Mentor Application" : "Mentor Application"}
      </h1>

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
          <Progress
            percent={progress}
            strokeColor={"#f97316"}
            className="mb-4"
          />
        </div>
      </div>

      <Form<MentorApplicationType>
        form={form}
        layout="vertical"
        name="mentor_application_form"
        onFinish={handleSubmit}
        requiredMark={false}
        onValuesChange={handleFieldChange}
      >
        <Form.Item
          name="education"
          label="Education"
          rules={[
            {
              max: 300,
              message: "Education must be less than 300 characters.",
            },
          ]}
        >
          <TextArea rows={4} placeholder="Your education..." />
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
          <TextArea rows={4} placeholder="Describe your work experience..." />
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
            rows={4}
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
            onChange={handleChange}
          >
            {fileCount < 5 && (
              <button style={{ border: 0, background: "none" }} type="button">
                Upload
              </button>
            )}
          </Upload>
        </Form.Item>

        <div className="flex gap-4">
          <Button
            className="bg-gray-500"
            size="large"
            onClick={() => navigate(-1)}
          >
            Back
          </Button>
          <div className="flex-1">
            <Form.Item>
              <Button
                type="primary"
                htmlType="submit"
                size="large"
                className="w-full bg-orange-500"
              >
                {isEditMode ? "Update" : "Submit"}
              </Button>
            </Form.Item>
          </div>
        </div>
      </Form>
    </div>
  );
};

export default MentorApplicationForm;
