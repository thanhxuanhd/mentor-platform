import { ShareAltOutlined } from "@ant-design/icons/lib/icons";
import { Button, Input, Form, Select, Radio, Tag, Spin } from "antd";
import type { CheckboxGroupProps } from "antd/es/checkbox";
import type { SelectProps } from "rc-select";
const { TextArea } = Input;
import { useEffect, useState } from "react";
import type { DefaultOptionType } from "antd/es/select";
import { useNavigate } from "react-router-dom";

// Sample availability options - these could come from an API
const availabilityOptions = [
  "Weekdays",
  "Weekends",
  "Mornings",
  "Afternoons",
  "Evenings",
];

const communicationMethodOptions: CheckboxGroupProps<string>["options"] = [
  { label: "Video Call", value: "video" },
  { label: "Audio Call", value: "audio" },
  { label: "Text Chat", value: "text" },
];

// type FileType = Parameters<GetProp<UploadProps, "beforeUpload">>[0];

// const getBase64 = (file: FileType): Promise<string> =>
//   new Promise((resolve, reject) => {
//     const reader = new FileReader();
//     reader.readAsDataURL(file);
//     reader.onload = () => resolve(reader.result as string);
//     reader.onerror = (error) => reject(error);
//   });

interface UserProfileData {
  fullName: string;
  bio: string;
  role: string;
  expertise: string[];
  skills: string;
  experience: string;
  avatar: string;
  availability: string[];
  communicationMethod: string;
  objective: string;
  email: string;
}

export default function UserProfile() {
  // const [previewOpen, setPreviewOpen] = useState(false);
  // const [previewImage, setPreviewImage] = useState("");
  const [selectedAvailability, setSelectedAvailability] = useState<string[]>(
    [],
  );
  const [tags, setTags] = useState<DefaultOptionType[]>([]);
  const [userData, setUserData] = useState<UserProfileData | null>(null);
  const [loading, setLoading] = useState<boolean>(true);
  const [form] = Form.useForm();
  const navigate = useNavigate();

  // Fetch user data from API
  useEffect(() => {
    const fetchUserData = async () => {
      try {
        // For demo purposes, simulate API response
        const data = {
          fullName: "Ngô Văn Minh",
          bio: "",
          role: "Mentor",
          expertise: ["Programming", "Project Management"],
          skills: "",
          experience: "",
          avatar: "",
          availability: ["Weekdays", "Evenings"],
          communicationMethod: "video",
          objective: "",
          email: "ngominh24122001@gmail.com",
        };

        setUserData(data);
        setSelectedAvailability(data.availability);
        form.setFieldsValue({
          fullname: data.fullName,
          bio: data.bio,
          expertise: data.expertise,
          skills: data.skills,
          experience: data.experience,
          availability: data.availability,
          communicationMethod: data.communicationMethod,
          objective: data.objective,
        });
        setLoading(false);
      } catch (error) {
        console.error("Error fetching user data:", error);
        setLoading(false);
      }
    };

    fetchUserData();
  }, [form]);

  // Load expertise options
  useEffect(() => {
    // This could also come from an API
    const options: SelectProps["options"] = [
      { label: "Programming", value: "Programming" },
      { label: "Project Management", value: "Project Management" },
      { label: "Data Science", value: "Data Science" },
      { label: "UX Design", value: "UX Design" },
      { label: "Digital Marketing", value: "Digital Marketing" },
    ];
    setTags(options);
  }, []);

  // const handlePreview = async (file: UploadFile) => {
  //   if (!file.url && !file.preview) {
  //     await getBase64(file.originFileObj as FileType).then((imageUrl) => {
  //       setPreviewImage(imageUrl);
  //     });
  //   }
  //   setPreviewOpen(true);
  // };

  // const handleChange: UploadProps["onChange"] = (info) => {
  //   if (info.file.status === "done") {
  //     getBase64(info.file.originFileObj as FileType).then((imageUrl) => {
  //       setPreviewImage(imageUrl);
  //     });
  //   }
  // };

  const toggleSelection = (
    value: string,
    list: string[],
    setter: (val: string[]) => void,
  ) => {
    if (list.includes(value)) {
      setter(list.filter((item) => item !== value));
    } else {
      setter([...list, value]);
    }
  };

  const handleShareProfile = () => {
    // Implement sharing functionality
    console.log("Sharing profile...");
    // Could open a modal with sharing options or copy link to clipboard
  };

  // const uploadButton = (
  //   <Button style={{ border: 0, background: "none", boxShadow: "none" }}>
  //     <PlusOutlined />
  //     <div>Upload</div>
  //   </Button>
  // );

  if (loading) {
    return (
      <div className="flex items-center justify-center h-screen">
        <Spin size="large" />
      </div>
    );
  }

  return (
    <div className="text-white p-6 rounded-xl max-w-3xl my-10 mx-auto shadow-lg bg-gray-800">
      <div className="text-white p-6 rounded-xl max-w-3xl my-10 mx-auto">
        <div className="flex justify-between items-center mb-6">
          <h1 className="text-2xl font-semibold">Your Profile</h1>
          <div className="flex gap-2">
            <Button
              type="primary"
              size="large"
              icon={<ShareAltOutlined />}
              onClick={handleShareProfile}
              className="bg-blue-500 hover:bg-blue-600"
            >
              Share Profile
            </Button>
            <Button
              type="primary"
              size="large"
              onClick={() => navigate("/profile/edit")}
              className="bg-orange-500 hover:bg-orange-600"
            >
              Edit Profile
            </Button>
          </div>
        </div>

        <div className="flex items-center gap-6 mb-8">
          <div className="w-24 h-24 rounded-full bg-gray-700 flex items-center justify-center overflow-hidden">
            {userData?.avatar ? (
              <img
                src={userData.avatar}
                alt="Profile"
                className="w-full h-full object-cover"
              />
            ) : (
              <div className="text-gray-400">
                <svg
                  width="40"
                  height="40"
                  viewBox="0 0 24 24"
                  fill="none"
                  xmlns="http://www.w3.org/2000/svg"
                >
                  <path
                    d="M12 12C14.21 12 16 10.21 16 8C16 5.79 14.21 4 12 4C9.79 4 8 5.79 8 8C8 10.21 9.79 12 12 12ZM12 14C9.33 14 4 15.34 4 18V20H20V18C20 15.34 14.67 14 12 14Z"
                    fill="currentColor"
                  />
                </svg>
              </div>
            )}
          </div>
          <div>
            <h2 className="text-2xl font-medium">
              {userData?.fullName || "Loading..."}
            </h2>
            <Tag color="orange" className="mt-1">
              {userData?.role || "User"}
            </Tag>
          </div>
        </div>

        <div className="mb-8">
          <h3 className="text-gray-400 mb-2">About</h3>
          <p>{userData?.bio || "No bio provided."}</p>
        </div>

        <div className="mb-8">
          <h3 className="text-gray-400 mb-2">Areas of expertise</h3>
          <div className="flex flex-wrap gap-2">
            {userData?.expertise && userData.expertise.length > 0 ? (
              userData.expertise.map((item) => (
                <Tag
                  key={item}
                  className="bg-gray-700 text-white border-none px-4 py-1 rounded-full"
                >
                  {item}
                </Tag>
              ))
            ) : (
              <p>No expertise provided.</p>
            )}
          </div>
        </div>

        <div className="mb-8">
          <h3 className="text-gray-400 mb-2">Professional skills</h3>
          <p>{userData?.skills || "No skills provided."}</p>
        </div>

        <div className="mb-8">
          <h3 className="text-gray-400 mb-2">Industry experience</h3>
          <p>{userData?.experience || "No experience provided."}</p>
        </div>

        <div className="mb-8">
          <h3 className="text-gray-400 mb-2">Contact</h3>
          <p>{userData?.email || "No contact provided."}</p>
        </div>

        <div className="pt-4 border-t border-gray-700">
          <Button
            type="link"
            className="text-orange-500 flex items-center gap-1 pl-0"
            onClick={() => console.log("View additional settings")}
          >
            View Additional Profile Settings
            <svg
              width="16"
              height="16"
              viewBox="0 0 24 24"
              fill="none"
              xmlns="http://www.w3.org/2000/svg"
            >
              <path
                d="M8.59 16.59L13.17 12L8.59 7.41L10 6L16 12L10 18L8.59 16.59Z"
                fill="currentColor"
              />
            </svg>
          </Button>
        </div>
        <div className="pt-4border-gray-700">
          <Button
            type="primary"
            size="large"
            onClick={() => navigate("/")}
            className="bg-orange-500 hover:bg-orange-600"
          >
            Back
          </Button>
        </div>
        <div className="hidden">
          <Form
            form={form}
            layout="vertical"
            name="user_profile_form"
            requiredMark={false}
          >
            <Form.Item name="fullname" label="Full Name">
              <Input size="large" placeholder="Full Name" />
            </Form.Item>

            <Form.Item name="bio" label="Bio">
              <TextArea
                placeholder="A brief introduction about yourself..."
                maxLength={200}
              />
            </Form.Item>

            <Form.Item name="expertise" label="Areas Of Expertise">
              <Select
                mode="multiple"
                allowClear
                placeholder="Select your field of expertise"
                className="w-full"
                size="large"
                options={tags}
              />
            </Form.Item>

            <Form.Item name="skills" label="Professional Skills">
              <Input size="large" placeholder="Your skills" />
            </Form.Item>

            <Form.Item name="experience" label="Industry Experience">
              <Input size="large" placeholder="Your experience" />
            </Form.Item>

            <Form.Item name="availability" label="Your Availability">
              <div className="flex gap-2 items-center justify-center">
                {availabilityOptions.map((item) => (
                  <Button
                    key={item}
                    type={
                      selectedAvailability.includes(item)
                        ? "primary"
                        : "default"
                    }
                    className={
                      selectedAvailability.includes(item)
                        ? "bg-orange-500 border-none"
                        : ""
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

            <Form.Item name="communicationMethod" label="Communication Method">
              <Radio.Group
                block
                options={communicationMethodOptions}
                optionType="button"
                buttonStyle="solid"
                size="large"
              />
            </Form.Item>

            <Form.Item name="objective" label="Objective">
              <TextArea
                placeholder="Describe your learning objectives and what you hope to achieve..."
                maxLength={200}
              />
            </Form.Item>
          </Form>
        </div>
      </div>
    </div>
  );
}
