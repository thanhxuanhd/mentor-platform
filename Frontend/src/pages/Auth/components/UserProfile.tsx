import {
  AudioOutlined,
  CloseOutlined,
  CommentOutlined,
  PlaySquareOutlined,
  PlusOutlined,
  UserOutlined,
} from "@ant-design/icons/lib/icons";
import {
  Upload,
  Button,
  Input,
  Form,
  Select,
  Radio,
  App,
  type FormInstance,
} from "antd";
import type { CheckboxGroupProps } from "antd/es/checkbox";
import type { RcFile, UploadChangeParam, UploadFile } from "antd/es/upload";
const { TextArea } = Input;
import { useCallback, useEffect, useState } from "react";
import type { NotificationProps } from "../../../types/Notification";
import { userService } from "../../../services/user/userService";
import type { UserDetail } from "../../../types/UserTypes";
import { axiosClient } from "../../../services/apiClient";
import type { Expertise } from "../../../types/ExpertiseType";
import type { AvailabilityType } from "../../../types/AvailabilityType";
import { CommunicationMethod } from "../../../types/enums/CommunicationMethod";
// Populate this as needed
const communicationMethodOptions: CheckboxGroupProps<CommunicationMethod>["options"] =
  [
    {
      label: (
        <div className="flex justify-center gap-2">
          <PlaySquareOutlined />
          <span className="font-medium">Video Call</span>
        </div>
      ),
      value: CommunicationMethod.VideoCall,
    },
    {
      label: (
        <div className="flex justify-center gap-2">
          <AudioOutlined />
          <span className="font-medium">Audio Call</span>
        </div>
      ),
      value: CommunicationMethod.AudioCall,
    },
    {
      label: (
        <div className="flex justify-center gap-2">
          <CommentOutlined />
          <span className="font-medium">Text Chat</span>
        </div>
      ),
      value: CommunicationMethod.TextChat,
    },
  ];

const roleOptions: CheckboxGroupProps<number>["options"] = [
  {
    label: (
      <div className="flex-1 px-4 py-3 rounded-md flex flex-col items-center">
        <span className="text-xl mb-1">👨‍🎓</span>
        <span className="font-medium">Learner</span>
        <span className="text-xs mt-1 opacity-80">I want to find mentors</span>
      </div>
    ),
    value: 3,
    className: "h-full!",
  },
  {
    label: (
      <div className="flex-1 px-4 py-3 rounded-md flex flex-col items-center">
        <span className="text-xl mb-1">👨‍🏫</span>
        <span className="font-medium">Mentor</span>
        <span className="text-xs mt-1 opacity-80">I want to mentor others</span>
      </div>
    ),
    value: 2,
    className: "h-full!",
  },
];

interface UserProfileProps {
  userId: string;
  userDetail: UserDetail;
  updateUserDetail: React.Dispatch<React.SetStateAction<UserDetail>>;
  formRef: React.RefObject<FormInstance<UserDetail> | null>; // Add formRef prop
}

const UserProfile: React.FC<UserProfileProps> = ({
  userId,
  userDetail,
  updateUserDetail,
  formRef,
}) => {
  const [form] = Form.useForm<UserDetail>();
  const [imageUrl, setImageUrl] = useState(userDetail.profilePhotoUrl || "");
  const [notify, setNotify] = useState<NotificationProps | null>(null);
  const [selectedAvailabilities, setSelectedAvailabilities] = useState<
    string[]
  >(userDetail.availabilityIds || []);
  const { notification } = App.useApp();
  const [expertises, setExpertises] = useState<Expertise[]>([]);
  const [availabilities, setAvailabilities] = useState<AvailabilityType[]>([]);

  const fetchExpertises = useCallback(async () => {
    try {
      const response = await axiosClient.get(`Expertises`);
      setExpertises(response.data.value);
    } catch {
      setNotify({
        type: "error",
        message: "Error",
        description: "Failed to fetch expertises",
      });
    }
  }, []);

  const fetchAvalabilities = useCallback(async () => {
    try {
      const response = await axiosClient.get(`Availabilities`);
      setAvailabilities(response.data.value);
    } catch {
      setNotify({
        type: "error",
        message: "Error",
        description: "Failed to fetch availabilities",
      });
    }
  }, []);

  useEffect(() => {
    if (formRef) {
      formRef.current = form;
    }
  }, [form, formRef]);

  useEffect(() => {
    fetchAvalabilities();
    fetchExpertises();
  }, [fetchAvalabilities, fetchExpertises]);

  useEffect(() => {
    const userExpertises = expertises.filter((item) =>
      userDetail.expertiseIds.includes(item.id),
    );
    form.setFieldsValue({
      expertiseIds: userExpertises.map((item) => item.id),
    });
  }, [expertises, userDetail.expertiseIds, form]);

  useEffect(() => {
    form.setFieldsValue(userDetail);
    setImageUrl(userDetail.profilePhotoUrl);
    setSelectedAvailabilities(userDetail.availabilityIds);
  }, [form, userDetail]);

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

  const beforeUpload = (file: RcFile) => {
    const isFormatAllowed =
      file.type === "image/jpeg" ||
      file.type === "image/png" ||
      file.type === "image/jpg";
    if (!isFormatAllowed) {
      setNotify({
        type: "error",
        message: "Error",
        description: "File format not allowed!",
      });
    }
    const isLt1M = file.size / 1024 / 1024 < 1;
    if (!isLt1M) {
      setNotify({
        type: "error",
        message: "Error",
        description: "Image must smaller than 1MB!",
      });
    }
    return isFormatAllowed && isLt1M;
  };

  const handleUpload = (info: UploadChangeParam<UploadFile>) => {
    if (info.file.status === "done") {
      const newImageUrl = info.file.response?.value;
      setImageUrl(newImageUrl);
      updateUserDetail((prev) => ({
        ...prev,
        profilePhotoUrl: newImageUrl,
      }));
      form.setFieldsValue({ profilePhotoUrl: newImageUrl });
    }
  };

  const toggleSelection = (value: string) => {
    const newAvailabilities = selectedAvailabilities.includes(value)
      ? selectedAvailabilities.filter((item) => item !== value)
      : [...selectedAvailabilities, value];
    setSelectedAvailabilities(newAvailabilities);
    form.setFieldsValue({ availabilityIds: newAvailabilities });
    updateUserDetail((prev) => ({
      ...prev,
      availabilityIds: newAvailabilities,
    }));
  };

  const handleTextChange = (
    event: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>,
  ) => {
    const { name, value } = event.target;
    form.setFieldsValue({ [name]: value });
    updateUserDetail((prev) => ({
      ...prev,
      [name]: value,
    }));
  };

  const handleFormChange = (changedValues: Partial<UserDetail>) => {
    updateUserDetail((prev) => ({
      ...prev,
      ...changedValues,
    }));
  };

  return (
    <div className="text-white p-6 rounded-xl max-w-3xl my-6 mx-auto shadow-lg bg-gray-800">
      <Form
        form={form}
        layout="vertical"
        name="user_profile_form"
        requiredMark={false}
        onValuesChange={handleFormChange}
      >
        <h1 className="text-2xl font-semibold mb-6">
          Tell us more about yourself
        </h1>
        <div className="flex gap-6 items-start">
          <Form.Item
            name="profilePhotoUrl"
            label="Profile Photo"
            valuePropName="fileList"
            getValueFromEvent={(e) => {
              if (Array.isArray(e)) {
                return e;
              }
              return e && e.fileList;
            }}
          >
            <Upload
              maxCount={1}
              showUploadList={false}
              action={`${import.meta.env.VITE_BASE_URL_BE}/Users/avatar/${userId}`}
              beforeUpload={beforeUpload}
              onChange={handleUpload}
            >
              <div className="relative w-32 h-32 rounded-full bg-gray-700 flex items-center justify-center cursor-pointer">
                {imageUrl ? (
                  <>
                    <img
                      src={imageUrl}
                      alt="avatar"
                      className="w-full h-full rounded-full object-cover"
                    />
                    <div
                      className="absolute top-0 bg-gray-400 right-0 leading-none p-1 rounded-full"
                      onClick={(e) => {
                        e.stopPropagation();
                        userService.removeAvatar(imageUrl);
                        setImageUrl("");
                        updateUserDetail((prev) => ({
                          ...prev,
                          profilePhotoUrl: "",
                        }));
                        form.setFieldsValue({ profilePhotoUrl: "" });
                      }}
                    >
                      <CloseOutlined />
                    </div>
                  </>
                ) : (
                  <span className="text-4xl">
                    <UserOutlined />
                  </span>
                )}
                <div className="absolute bottom-0 bg-orange-500 right-0 leading-none p-3 rounded-full">
                  <PlusOutlined />
                </div>
              </div>
            </Upload>
          </Form.Item>
          <div className="flex-1">
            <div className="flex gap-4">
              <div className="flex-2">
                <Form.Item
                  name="fullName"
                  label="Full Name"
                  rules={[
                    { required: true, message: "Please enter your full name!" },
                    {
                      max: 100,
                      message: "Full name can not exceed 50 characters!",
                    },
                    {
                      pattern: /^[A-Za-z\s]+$/,
                      message: "Full name can only contain letters and spaces!",
                    },
                  ]}
                >
                  <Input
                    name="fullName"
                    maxLength={100}
                    size="large"
                    placeholder="Full Name"
                    onChange={handleTextChange}
                  />
                </Form.Item>
              </div>
              <div className="flex-1">
                <Form.Item
                  name="phoneNumber"
                  label="Phone Number"
                  rules={[
                    {
                      required: true,
                      message: "Please enter your phone number!",
                    },
                    {
                      pattern: /^\d{10}$/,
                      message:
                        "Phone number must consist of exactly 10 digits!",
                    },
                  ]}
                >
                  <Input
                    name="phoneNumber"
                    maxLength={10}
                    size="large"
                    placeholder="Phone Number"
                    type="tel"
                    onChange={handleTextChange}
                  />
                </Form.Item>
              </div>
            </div>

            <Form.Item
              name="bio"
              label="Bio"
              rules={[
                { max: 300, message: "Bio must be less than 300 characters!" },
              ]}
            >
              <TextArea
                name="bio"
                placeholder="A brief introduction about yourself..."
                maxLength={300}
                onChange={handleTextChange}
              />
            </Form.Item>
          </div>
        </div>

        <Form.Item
          name="roleId"
          label="I am joining as"
          rules={[{ required: true, message: "Please select a role!" }]}
        >
          <Radio.Group
            block
            options={roleOptions}
            optionType="button"
            buttonStyle="solid"
            className="h-content! gap-2"
          />
        </Form.Item>

        <Form.Item name="expertiseIds" label="Areas Of Expertise">
          <Select
            mode="multiple"
            allowClear
            placeholder="Select your field of expertise"
            className="w-full"
            size="large"
            options={expertises}
            fieldNames={{ label: "name", value: "id" }}
          />
        </Form.Item>

        <div className="flex gap-4 items-center justify-center">
          <div className="flex-1">
            <Form.Item name="skills" label="Professional Skills">
              <Input
                name="skills"
                size="large"
                placeholder="Your skills"
                onChange={handleTextChange}
              />
            </Form.Item>
          </div>
          <div className="flex-1">
            <Form.Item name="experiences" label="Industry Experience">
              <Input
                name="experiences"
                size="large"
                placeholder="Your experience"
                onChange={handleTextChange}
              />
            </Form.Item>
          </div>
        </div>

        <Form.Item
          name="availabilityIds"
          label="Your Availability"
          rules={[
            {
              required: true,
              message: "Please select your availability!",
            },
          ]}
        >
          <div className="flex gap-2 items-center justify-center flex-wrap">
            {availabilities.map((item) => (
              <Button
                key={item.id}
                type={
                  selectedAvailabilities.includes(item.id)
                    ? "primary"
                    : "default"
                }
                className={
                  (selectedAvailabilities.includes(item.id)
                    ? "bg-orange-500 border-none"
                    : "") + " flex-1"
                }
                size="large"
                onClick={() => {
                  toggleSelection(item.id);
                }}
              >
                {item.name}
              </Button>
            ))}
          </div>
        </Form.Item>

        <Form.Item
          name="preferredCommunicationMethod"
          label="Communication Method"
          rules={[
            {
              required: true,
              message: "Please select a communication method!",
            },
          ]}
          initialValue={userDetail.preferredCommunicationMethod}
        >
          <Radio.Group
            block
            options={communicationMethodOptions}
            optionType="button"
            buttonStyle="solid"
            size="large"
          />
        </Form.Item>

        <Form.Item
          name="goal"
          label="Objective"
          rules={[
            { max: 200, message: "Objective can not exceed 200 characters" },
          ]}
          initialValue={userDetail.goal}
        >
          <TextArea
            name="goal"
            placeholder="Describe your learning objectives and what you hope to achieve..."
            maxLength={200}
            onChange={handleTextChange}
          />
        </Form.Item>
      </Form>
    </div>
  );
};

export default UserProfile;
