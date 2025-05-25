import { useEffect, useState, useContext } from "react";
import { useNavigate, useParams } from "react-router-dom";
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
  type GetProp,
} from "antd";
import type { CheckboxGroupProps } from "antd/es/checkbox";
import type {
  RcFile,
  UploadChangeParam,
  UploadFile,
  UploadProps,
} from "antd/es/upload";
import type { DefaultOptionType } from "antd/es/select";
import { userService } from "../../../services/user/userService";
import { getListCategories } from "../../../services/category/categoryServices";
import { AuthContext } from "../../../contexts/AuthContext";

import type { NotificationProps } from "../../../types/Notification";
import type {
  UpdateProfileRequest,
  UserProfile as UserProfileType,
} from "../../../types/UserTypes"; // Explicitly used now

const { TextArea } = Input;

const communicationMethodMap: Record<string, number> = {
  video: 0,
  audio: 1,
  text: 2,
};

const reverseCommunicationMethodMap: Record<number, string> =
  Object.fromEntries(
    Object.entries(communicationMethodMap).map(([key, value]) => [value, key]),
  );

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

const roleMap: Record<string, number> = {
  learner: 3,
  mentor: 2,
};

const reverseAvailabilityMap: Record<string, string> = {};
const reverseTeachingApproachMap: Record<string, string> = {};
const reverseCategoryMap: Record<string, string> = {};

export default function EditProfile() {
  const [form] = Form.useForm();
  const [imageUrl, setImageUrl] = useState("");
  const [notify, setNotify] = useState<NotificationProps | null>(null);
  const [loading, setLoading] = useState(false);
  const navigate = useNavigate();
  const { userId } = useParams<{ userId: string }>();
  const [selectedAvailability, setSelectedAvailability] = useState<string[]>(
    [],
  );
  const { notification } = App.useApp();
  const [expertiseOptions, setExpertiseOptions] = useState<DefaultOptionType[]>(
    [],
  );
  const [availabilityOptions, setAvailabilityOptions] = useState<
    { id: string; name: string }[]
  >([]);
  const [teachingApproachOptions, setTeachingApproachOptions] = useState<
    DefaultOptionType[]
  >([]);
  const [categoryOptions, setCategoryOptions] = useState<DefaultOptionType[]>(
    [],
  );

  const { user } = useContext(AuthContext);
  const token = localStorage.getItem("token");
  type FileType = Parameters<GetProp<UploadProps, "beforeUpload">>[0];
  const getBase64 = (img: FileType, callback: (url: string) => void) => {
    const reader = new FileReader();
    reader.addEventListener("load", () => callback(reader.result as string));
    reader.readAsDataURL(img);
  };
  useEffect(() => {
    const fetchDropdownData = async () => {
      if (!token) {
        setNotify({
          type: "error",
          message: "Error",
          description: "Authentication token not found.",
        });
        setLoading(false);
        return;
      }
      try {
        const [
          availabilities,
          expertises,
          teachingApproaches,
          categoriesResult,
        ] = await Promise.all([
          userService.getAvailabilities(token),
          userService.getExpertises(token),
          userService.getTeachingApproaches(token),
          getListCategories(1, 100),
        ]);

        availabilities.forEach((item) => {
          reverseAvailabilityMap[item.name] = item.id;
        });
        teachingApproaches.forEach((item) => {
          reverseTeachingApproachMap[item.name] = item.id;
        });

        const categoriesData = categoriesResult?.items || [];
        categoriesData.forEach((item: { id: string; name: string }) => {
          reverseCategoryMap[item.name] = item.id;
        });

        setAvailabilityOptions(availabilities);
        setExpertiseOptions(
          expertises.map((item) => ({ label: item.name, value: item.id })),
        );
        setTeachingApproachOptions(
          teachingApproaches.map((item) => ({
            label: item.name,
            value: item.id,
          })),
        );
        setCategoryOptions(
          categoriesData.map((item: { id: string; name: string }) => ({
            label: item.name,
            value: item.id,
          })),
        );
      } catch (error) {
        console.error("Error fetching dropdown data:", error);
        setNotify({
          type: "error",
          message: "Error",
          description: "Failed to load dropdown data.",
        });
      }
    };

    fetchDropdownData();
  }, [token]);

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

        const userProfileData: UserProfileType =
          await userService.getUserProfile(currentUserId);

        form.setFieldsValue({
          fullname: userProfileData.fullName,
          phone: userProfileData.phoneNumber,
          bio: userProfileData.bio,
          roleSelect:
            userProfileData.roleId === 3
              ? "learner"
              : userProfileData.roleId === 2
                ? "mentor"
                : "learner",
          expertise: userProfileData.expertiseIds || [],
          skills: userProfileData.skills,
          experience: userProfileData.experiences,
          objective: userProfileData.goal,
          communicationMethod:
            reverseCommunicationMethodMap[
              userProfileData.preferredCommunicationMethod
            ] || "video",
          availability: userProfileData.availabilityIds || [],
          teachingApproach: userProfileData.teachingApproachIds || [],
          categoryIds: userProfileData.categoryIds || [],
        });

        setSelectedAvailability(
          userProfileData.availabilityIds
            ?.map((id) => {
              const found = availabilityOptions.find((opt) => opt.id === id);
              return found ? found.name : "";
            })
            .filter(Boolean) || [],
        );

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

    if (
      availabilityOptions.length > 0 &&
      expertiseOptions.length > 0 &&
      teachingApproachOptions.length > 0 &&
      categoryOptions.length > 0
    ) {
      fetchUserData();
    }
  }, [
    userId,
    user,
    token,
    navigate,
    form,
    availabilityOptions,
    expertiseOptions,
    teachingApproachOptions,
    categoryOptions,
  ]);

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
      getBase64(info.file.originFileObj as FileType, (url) => {
        setImageUrl(url);
        form.setFieldsValue({ profilePhotoUrl: info.file.response?.value });
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

    const newIds = newList
      .map((name) => reverseAvailabilityMap[name])
      .filter(Boolean);
    form.setFieldsValue({ availability: newIds });
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

      const availabilityIds = values.availability;
      const expertiseIds = values.expertise;
      const teachingApproachIds = values.teachingApproach;
      const categoryIds = values.categoryIds;

      const updateData: UpdateProfileRequest = {
        fullName: values.fullname,
        phoneNumber: values.phone,
        roleId: roleMap[values.roleSelect] || 3,
        bio: values.bio || "",
        skills: values.skills || "",
        experiences: values.experience || "",
        goal: values.objective || "",
        preferredCommunicationMethod:
          communicationMethodMap[values.communicationMethod] || 0,
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
        profilePhotoUrl: values.avatar[0].response.value,
      };

      console.log(
        "Sending update to API:",
        JSON.stringify(updateData, null, 2),
      );
      const result = await userService.updateUserProfile(
        currentUserId,
        updateData,
      );
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
        <h1 className="text-2xl font-semibold mb-6">Edit Your Profile</h1>
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
                        {
                          required: true,
                          message: "Please enter your full name!",
                        },
                        {
                          max: 50,
                          message: "Full name can not exceed 100 characters!",
                        },
                        {
                          pattern: /^[A-Za-z\s]+$/,
                          message:
                            "Full name can only contain letters and spaces!",
                        },
                      ]}
                    >
                      <Input
                        maxLength={50}
                        size="large"
                        placeholder="Full Name"
                      />
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
                    {
                      max: 200,
                      message: "Bio must be less than 200 characters!",
                    },
                  ]}
                >
                  <TextArea
                    placeholder="A brief introduction about yourself..."
                    maxLength={200}
                  />
                </Form.Item>
              </div>
            </div>

            <div>
              <Form.Item name="expertise" label="Areas Of Expertise" rules={[]}>
                <Select
                  mode="multiple"
                  allowClear
                  placeholder="Select your field of expertise"
                  className="w-full"
                  size="large"
                  options={expertiseOptions}
                />
              </Form.Item>
            </div>
            <div className="flex gap-4 items-center justify-center">
              <div className="flex-1">
                <Form.Item
                  name="skills"
                  label="Professional Skills"
                  rules={[
                    {
                      max: 200,
                      message:
                        "Professional Skills can not exceed 200 characters",
                    },
                  ]}
                >
                  <Input size="large" placeholder="Your skills" />
                </Form.Item>
              </div>
              <div className="flex-1">
                <Form.Item
                  name="experience"
                  label="Industry Experience"
                  rules={[
                    {
                      max: 200,
                      message:
                        "Industry Experience can not exceed 200 characters",
                    },
                  ]}
                >
                  <Input size="large" placeholder="Your experience" />
                </Form.Item>
              </div>
            </div>
            <div className="flex gap-4 items-center justify-center"></div>
            <Form.Item name="availability" label="Your Availability" rules={[]}>
              <div className="flex gap-2 items-center justify-center flex-wrap">
                {availabilityOptions.map((item) => (
                  <Button
                    key={item.id}
                    type={
                      selectedAvailability.includes(item.name)
                        ? "primary"
                        : "default"
                    }
                    className={
                      (selectedAvailability.includes(item.name)
                        ? "bg-orange-500 border-none"
                        : "") + " flex-1"
                    }
                    size="large"
                    onClick={() =>
                      toggleSelection(
                        item.name,
                        selectedAvailability,
                        setSelectedAvailability,
                      )
                    }
                  >
                    {item.name}
                  </Button>
                ))}
              </div>
            </Form.Item>

            <Form.Item
              name="teachingApproach"
              label="Teaching Approach"
              rules={[]}
            >
              <Select
                mode="multiple"
                allowClear
                placeholder="Select your preferred teaching approaches"
                className="w-full"
                size="large"
                options={teachingApproachOptions}
              />
            </Form.Item>

            <Form.Item name="categoryIds" label="Categories" rules={[]}>
              <Select
                mode="multiple"
                allowClear
                placeholder="Select relevant categories"
                className="w-full"
                size="large"
                options={categoryOptions}
              />
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
                {
                  max: 200,
                  message: "Objective can not exceed 200 characters",
                },
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
