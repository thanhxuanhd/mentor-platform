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

const availabilityOptions = [
  "Weekdays",
  "Weekends", 
  "Mornings",
  "Afternoons",
  "Evenings",
];


const availabilityMap: Record<string, string> = {
  "Weekdays": "3fa85f64-5717-4562-b3fc-2c963f66afa1",
  "Weekends": "3fa85f64-5717-4562-b3fc-2c963f66afa2", 
  "Mornings": "3fa85f64-5717-4562-b3fc-2c963f66afa3",
  "Afternoons": "3fa85f64-5717-4562-b3fc-2c963f66afa4",
  "Evenings": "3fa85f64-5717-4562-b3fc-2c963f66afa5",
};

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

const communicationMethodMap: Record<string, number> = {
  "video": 0,
  "audio": 1,
  "text": 2,
};

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

const expertiseMap: Record<string, string> = {
  "Programming": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "Project Management": "3fa85f64-5717-4562-b3fc-2c963f66afa7",
  "Data Science": "3fa85f64-5717-4562-b3fc-2c963f66afa8",
  "UX Design": "3fa85f64-5717-4562-b3fc-2c963f66afa9",
  "Digital Marketing": "3fa85f64-5717-4562-b3fc-2c963f66afaa",
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
        
        const expertiseOptions = Object.entries(expertiseMap).map(([label, value]) => ({
          label,
          value: value 
        }));
        setTags(expertiseOptions);
        
        form.setFieldsValue({
          fullname: userProfileData.fullName,
          phone: userProfileData.phoneNumber,
          bio: userProfileData.bio,
          roleSelect: userProfileData.roleId === 3 ? "learner" : userProfileData.roleId === 2 ? "mentor" : "learner",
          expertise: userProfileData.expertiseIds?.map(id => String(id)),
          skills: userProfileData.skills,
          experience: userProfileData.experiences,
          objective: userProfileData.goal,
          communicationMethod: Object.keys(communicationMethodMap)[userProfileData.preferredCommunicationMethod] || "video",
          availability: userProfileData.availabilityIds?.map(id => Object.keys(availabilityMap).find(key => availabilityMap[key] === String(id)) || "").filter(Boolean) || [],
        });

        const mappedAvailability = userProfileData.availabilityIds?.map(id => {
          return Object.keys(availabilityMap).find(key => availabilityMap[key] === String(id)) || "";
        }).filter(Boolean);
        
        setSelectedAvailability(mappedAvailability || []);
        
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
        description: "Image must smaller than 2MB!",
      });
    }
    return isFormatAllowed && isLt1M;
  };

  const handleChange = (info: UploadChangeParam<UploadFile>) => {
    if (info.file.status === "done" || info.file.status === "uploading") {
      const reader = new FileReader();
      reader.addEventListener("load", () =>
        setImageUrl(reader.result as string),
      );
      reader.readAsDataURL(info.file.originFileObj as RcFile);
    }
  };

  const handleUploadPhoto = async (file: RcFile): Promise<void> => {
    if (!token || !userId) return;
    
    try {
      const photoUrl = await UserProfileClient.uploadProfilePhoto(userId, file, token);
      setImageUrl(photoUrl);
      return;
    } catch (error) {
      console.error("Error uploading photo:", error);
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
      const updateData: UpdateProfileRequest = {
        fullName: values.fullname,
        phoneNumber: values.phone,
        roleId: roleMap[values.roleSelect] || 3, 
        bio: values.bio || "",
        skills: values.skills || "",
        experiences: values.experience || "",
        goal: values.objective || "",
        preferredCommunicationMethod: communicationMethodMap[values.communicationMethod] || 0,
        preferredSessionFrequency: 0, // Default value - you may want to add this to the form
        preferredSessionDuration: 60, // Default 60 minutes - you may want to add this to the form  
        preferredLearningStyle: 0, // Default value - you may want to add this to the form
        isPrivate: false, // Default value - you may want to add this to the form
        isAllowedMessage: true, // Default value - you may want to add this to the form
        isReceiveNotification: true, // Default value - you may want to add this to the form
        expertiseIds: values.expertise?.filter(Boolean) || ["ECBEA5A8-62A1-48D8-FB6D-08DD9819FEC3"],
        availabilityIds: selectedAvailability.map(item => availabilityMap[item]).filter(Boolean),
        teachingApproachIds: ["9178C57A-B963-469B-06AA-08DD9819FEC9"], // Empty array as default - you may want to add this to the form
        categoryIds: ["4B896130-3727-46C7-98D1-214107BD4709"], // Empty array as default - you may want to add this to the form
        profilePhotoUrl: imageUrl || undefined
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
                  beforeUpload={beforeUpload}
                  onChange={handleChange}
                  customRequest={async ({ file, onSuccess }) => {
                    const uploadFile = file as RcFile;
                    await handleUploadPhoto(uploadFile);
                    onSuccess?.("ok");
                  }}
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
                          message: "Full name can not exceed 50 characters!",
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
                          message:
                            "Phone number must consist of exactly 10 digits!",
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
                <Form.Item name="skills" label="Professional Skills">
                  <Input size="large" placeholder="Your skills" />
                </Form.Item>
              </div>
              <div className="flex-1">
                <Form.Item name="experience" label="Industry Experience">
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