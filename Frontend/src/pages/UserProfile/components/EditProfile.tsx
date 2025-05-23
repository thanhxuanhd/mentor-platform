import {
  AudioOutlined,
  CloseOutlined,
  CommentOutlined,
  PlaySquareOutlined,
  PlusOutlined,
  UserOutlined,
} from "@ant-design/icons/lib/icons";
import { useNavigate, useParams } from "react-router-dom";
import { Upload, Button, Input, Form, Select, Radio, App } from "antd";
import type { CheckboxGroupProps } from "antd/es/checkbox";
import type { RcFile, UploadChangeParam, UploadFile } from "antd/es/upload";
const { TextArea } = Input;
import { useEffect, useState, useContext } from "react";
import type { DefaultOptionType } from "antd/es/select";
import type { NotificationProps } from "../../../types/Notification";
import UserProfileClient, { type UpdateProfileRequest } from "../userProfileClient";
import { AuthContext } from "../../../contexts/AuthContext";

const availabilityMap: Record<string, string> = {
  "67eb556e-d860-498d-212a-08dd9819fea6": "Weekdays",
  "3eab46db-9a91-4ab7-212b-08dd9819fea6": "Weekends",
  "eec61c62-5198-4e1d-212c-08dd9819fea6": "Mornings",
  "9574cf95-2df0-4982-212d-08dd9819fea6": "Afternoons",
  "25cfac78-2bc0-46bf-212e-08dd9819fea6": "Evenings",
};

const reverseAvailabilityMap: Record<string, string> = Object.fromEntries(
  Object.entries(availabilityMap).map(([guid, value]) => [value, guid])
);

const expertiseMap: Record<string, string> = {
  "ecbea5a8-62a1-48d8-fb6d-08dd9819fec3": "Business",
  "9ea17990-b9b6-4041-fb6f-08dd9819fec3": "Communication",
  "2b7ac1a6-29c6-4b74-fb6c-08dd9819fec3": "Data Science",
  "02d3db0e-0ce1-493e-fb6a-08dd9819fec3": "Design",
  "8f0585c6-6b78-46dc-fb68-08dd9819fec3": "Leadership",
  "5a1f1d05-8e7c-4dce-fb6b-08dd9819fec3": "Marketing",
  "5c9dd8a1-a6f3-4fdd-fb69-08dd9819fec3": "Programming",
  "a36ffa6c-b908-4b70-fb6e-08dd9819fec3": "Project Management",
};

const teachingApproachMap: Record<string, string> = {
  "9178c57a-b963-469b-06aa-08dd9819fec9": "Discussion Based",
  "b9a50969-32e1-40b7-06a9-08dd9819fec9": "Hands-on Practice",
  "75e450d4-3928-46e1-06ac-08dd9819fec9": "Lecture Style",
  "3c916553-b282-48cf-06ab-08dd9819fec9": "Project Based",
};

const reverseTeachingApproachMap: Record<string, string> = Object.fromEntries(
  Object.entries(teachingApproachMap).map(([guid, value]) => [value, guid])
);

const categoryMap: Record<string, string> = {
  "Time Management": "4b896130-3727-46c7-98d1-214107bd4709",
  "Communication Skills": "07e80bb4-5fbb-4016-979d-847878ab81d5",
  "Public Speaking": "4aa8eb25-7bb0-4bdc-b391-9924bc218eb2",
  "Leadership Coaching": "3144da58-deaa-4bf7-a777-cd96e7f1e3b1",
  "Career Development": "ead230f7-76ff-4c10-b025-d1f80fcdd277",
};

const communicationMethodMap: Record<string, number> = {
  "video": 0,
  "audio": 1,
  "text": 2,
};

const reverseCommunicationMethodMap: Record<number, string> = Object.fromEntries(
  Object.entries(communicationMethodMap).map(([key, value]) => [value, key])
);

const availabilityOptions = [
  "Weekdays",
  "Weekends",
  "Mornings",
  "Afternoons",
  "Evenings",
];

const communicationMethodOptions: CheckboxGroupProps<string>["options"] = [
  {
    label: (
      <div className="flex justify-center gap-2">
        <PlaySquareOutlined />
        <span className="font-medium">Video Call</span>
      </div>
    ),
    value: "video",
  },
  {
    label: (
      <div className="flex justify-center gap-2">
        <AudioOutlined />
        <span className="font-medium">Audio Call</span>
      </div>
    ),
    value: "audio",
  },
  {
    label: (
      <div className="flex justify-center gap-2">
        <CommentOutlined />
        <span className="font-medium">Text Chat</span>
      </div>
    ),
    value: "text",
  },
];

const roleOptions: CheckboxGroupProps<string>["options"] = [
  {
    label: (
      <div className="flex-1 px-4 py-3 rounded-md flex flex-col items-center">
        <span className="text-xl mb-1">👨‍🎓</span>
        <span className="font-medium">Learner</span>
        <span className="text-xs mt-1 opacity-80">I want to find mentors</span>
      </div>
    ),
    value: "learner",
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
    value: "mentor",
    className: "h-full!",
  },
];

const roleMap: Record<string, number> = {
  "learner": 3,
  "mentor": 2,
};

export default function EditProfile() {
  const [form] = Form.useForm();
  const [imageUrl, setImageUrl] = useState("");
  const [notify, setNotify] = useState<NotificationProps | null>(null);
  const [loading, setLoading] = useState(false);
  const navigate = useNavigate();
  const { userId } = useParams<{ userId: string }>();
  const [selectedAvailability, setSelectedAvailability] = useState<string[]>([]);
  const { notification } = App.useApp();
  const [tags, setTags] = useState<DefaultOptionType[]>([]);
  const { user } = useContext(AuthContext);
  const token = localStorage.getItem("token");

  useEffect(() => {
    const fetchUserData = async () => {
      try {
        setLoading(true);
        if (!token) {
          throw new Error("Token not found");
        }

        const currentUserId = userId || user?.id;
        if (!currentUserId) {
          throw new Error("User ID not found");
        }
        
        const userProfileData = await UserProfileClient.getProfile(currentUserId, token);
        
        const expertiseOptions = Object.entries(expertiseMap).map(([guid, label]) => ({
          label,
          value: guid, 
        }));
        setTags(expertiseOptions);

        const mappedExpertise = userProfileData.expertiseIds || [];
        
        const mappedAvailability = userProfileData.availabilityIds?.map(id => availabilityMap[id] || "").filter(Boolean) || [];

        form.setFieldsValue({
          fullname: userProfileData.fullName,
          phone: userProfileData.phoneNumber,
          bio: userProfileData.bio,
          roleSelect: userProfileData.roleId === 3 ? "learner" : userProfileData.roleId === 2 ? "mentor" : "learner",
          expertise: mappedExpertise,
          skills: userProfileData.skills,
          experience: userProfileData.experiences,
          objective: userProfileData.goal,
          communicationMethod: reverseCommunicationMethodMap[userProfileData.preferredCommunicationMethod] || "video",
          availability: mappedAvailability,
        });

        setSelectedAvailability(mappedAvailability);
        
        if (userProfileData.profilePhotoUrl) {
          setImageUrl(userProfileData.profilePhotoUrl);
        }
        
        setLoading(false);
      } catch (error) {
        console.error("Error fetching user data:", error);
        setLoading(false);
        setNotify({
          type: "error",
          message: "Error",
          description: "Failed to load user profile data",
        });
        navigate("/profile");
      }
    };

    fetchUserData();
  }, [userId, user, token, navigate, form]);

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

  const handleChange = (info: UploadChangeParam<UploadFile>) => {
    if (info.file.status === "done") {
      setImageUrl(info.file.response?.value);
    } else if (info.file.status === "error") {
      setNotify({
        type: "error",
        message: "Error",
        description: "Failed to upload photo",
      });
    }
  };

  const toggleSelection = (
    value: string,
    list: string[],
    setter: (val: string[]) => void,
  ) => {
    const newList = list.includes(value)
      ? list.filter((item) => item !== value)
      : [...list, value];
    setter(newList);
    form.setFieldsValue({ availability: newList });
  };

  const handleSubmit = async () => {
    try {
      const values = await form.validateFields();
      setLoading(true);
      
      if (!token) {
        throw new Error("Token not found");
      }

      const currentUserId = userId || user?.id;
      if (!currentUserId) {
        throw new Error("User ID not found");
      }
      
      const availabilityIds = values.availability.map((item: string) => reverseAvailabilityMap[item]).filter(Boolean);
      const expertiseIds = values.expertise;
      const teachingApproachIds = values.availability.map((item: string) => reverseTeachingApproachMap[item]).filter(Boolean);
      const categoryIds = values.availability.map((item: string) => categoryMap[item]).filter(Boolean);
      
      const updateData: UpdateProfileRequest = {
        fullName: values.fullname,
        phoneNumber: values.phone,
        roleId: roleMap[values.roleSelect] || 3,
        bio: values.bio || "",
        skills: values.skills || "",
        experiences: values.experience || "",
        goal: values.objective || "",
        preferredCommunicationMethod: communicationMethodMap[values.communicationMethod] || 0,
        preferredSessionFrequency: 0,
        preferredSessionDuration: 60,
        preferredLearningStyle: 0,
        isPrivate: false,
        isAllowedMessage: true,
        isReceiveNotification: true,
        expertiseIds,
        availabilityIds,
        teachingApproachIds,
        categoryIds,
        profilePhotoUrl: imageUrl || undefined,
      };
      
      console.log("Sending update to API:", JSON.stringify(updateData, null, 2));
      const result = await UserProfileClient.updateProfile(currentUserId, updateData);
      console.log("API update result:", result);
      
      setNotify({
        type: "success",
        message: "Success",
        description: "Profile updated successfully!",
      });
    
      setTimeout(() => {
        navigate("/profile");
      }, 1000);
    } catch (error) {
      console.error("Error updating profile:", error);
      setNotify({
        type: "error",
        message: "Error",
        description: "Failed to update profile",
      });
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="text-white p-6 rounded-xl max-w-3xl my-10 mx-auto shadow-lg bg-gray-800">
      <Form 
        form={form} 
        layout="vertical" 
        name="user_profile_form" 
        requiredMark={false}
        onFinish={handleSubmit}
      >
        <h1 className="text-2xl font-semibold mb-6">
          Edit Your Profile
        </h1>
        {loading ? (
          <div className="flex justify-center py-10">
            <span className="loading loading-spinner loading-lg"></span>
          </div>
        ) : (
          <>
            <div className="flex gap-6 items-start">
              <Form.Item
                name="avatar"
                label="Profile Picture"
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
                  action={`${import.meta.env.VITE_BASE_URL_BE}/Users/avatar/${userId || user?.id}`}
                  headers={{
                    Authorization: `Bearer ${token}`,
                  }}
                  beforeUpload={beforeUpload}
                  onChange={handleChange}
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
                            setImageUrl(""); 
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
                      name="fullname"
                      label="Full Name"
                      rules={[
                        { required: true, message: "Please enter your full name!" },
                        {
                          max: 50,
                          message: "Full name can not exceed 100 characters!",
                        },
                        {
                          pattern: /^[A-Za-z\s]+$/,
                          message: "Full name can only contain letters and spaces!",
                        },
                      ]}
                    >
                      <Input maxLength={50} size="large" placeholder="Full Name" />
                    </Form.Item>
                  </div>
                  <div className="flex-1">
                    <Form.Item
                      name="phone"
                      label="Phone Number"
                      rules={[
                        {
                          required: true,
                          message: "Please enter your phone number!",
                        },
                        {
                          pattern: /^\d{10}$/,
                          message: "Phone number must consist of exactly 10 digits!",
                        },
                      ]}
                    >
                      <Input
                        maxLength={10}
                        size="large"
                        placeholder="Phone Number"
                        type="tel"
                      />
                    </Form.Item>
                  </div>
                </div>

                <Form.Item
                  name="bio"
                  label="Bio"
                  rules={[
                    { max: 200, message: "Bio must be less than 200 characters!" },
                  ]}
                >
                  <TextArea
                    placeholder="A brief introduction about yourself..."
                    maxLength={200}
                  />
                </Form.Item>
              </div>
            </div>

            <Form.Item
              name="roleSelect"
              label="I am joining as"
              initialValue={"learner"}
              rules={[]}
            >
              <Radio.Group
                block
                options={roleOptions}
                optionType="button"
                buttonStyle="solid"
                className="h-content! gap-2"
              />
            </Form.Item>

            <div>
              <Form.Item
                name="expertise"
                label="Areas Of Expertise"
                rules={[]}
              >
                <Select
                  mode="multiple"
                  allowClear
                  placeholder="Select your field of expertise"
                  className="w-full"
                  size="large"
                  options={tags}
                />
              </Form.Item>
            </div>
            <div className="flex gap-4 items-center justify-center">
              <div className="flex-1">
                <Form.Item name="skills" label="Professional Skills"
                rules={[
                { max: 200, message: "Professional Skills can not exceed 200 characters" },
              ]}>
                  <Input size="large" placeholder="Your skills" />
                </Form.Item>
              </div>
              <div className="flex-1">
                <Form.Item name="experience" label="Industry Experience"
                rules={[
                { max: 200, message: "Industry Experience can not exceed 200 characters" },
              ]}>
                  <Input size="large" placeholder="Your experience" />
                </Form.Item>
              </div>
            </div>
            <div className="flex gap-4 items-center justify-center"></div>
            <Form.Item
              name="availability"
              label="Your Availability"
              rules={[]}
            >
              <div className="flex gap-2 items-center justify-center flex-wrap">
                {availabilityOptions.map((item) => (
                  <Button
                    key={item}
                    type={
                      selectedAvailability.includes(item) ? "primary" : "default"
                    }
                    className={
                      (selectedAvailability.includes(item)
                        ? "bg-orange-500 border-none"
                        : "") + " flex-1"
                    }
                    size="large"
                    onClick={() =>
                      toggleSelection(
                        item,
                        selectedAvailability,
                        setSelectedAvailability,
                      )
                    }
                  >
                    {item}
                  </Button>
                ))}
              </div>
            </Form.Item>

            <Form.Item
              name="communicationMethod"
              label="Communication Method"
              initialValue={"video"}
              rules={[]}
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
              name="objective"
              label="Objective"
              rules={[
                { max: 200, message: "Objective can not exceed 200 characters" },
              ]}
            >
              <TextArea
                placeholder="Describe your learning objectives and what you hope to achieve..."
                maxLength={200}
              />
            </Form.Item>

            <div className="flex justify-between mt-6 border-t border-gray-700 pt-4 gap-4">
              <Button
                type="primary"
                size="large"
                onClick={() => navigate("/profile")}
                className="bg-gray-500! hover:bg-gray-400!"
              >
                Back
              </Button>
              <Button
                type="primary"
                size="large"
                className="flex-1 bg-orange-500 hover:bg-orange-600"
                htmlType="submit"
                loading={loading}
                onClick={handleSubmit}
              >
                Save Changes
              </Button>
            </div>
          </>
        )}
      </Form>
    </div>
  );
}