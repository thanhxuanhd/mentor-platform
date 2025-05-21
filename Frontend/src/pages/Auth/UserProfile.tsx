import {
  AudioOutlined,
  CloseOutlined,
  CommentOutlined,
  PlaySquareOutlined,
  PlusOutlined,
  UserOutlined,
} from "@ant-design/icons/lib/icons";
import { Upload, Button, Input, Form, Select, Radio, App } from "antd";
import type { CheckboxGroupProps } from "antd/es/checkbox";
import type { RcFile, UploadChangeParam, UploadFile } from "antd/es/upload";
import type { SelectProps } from "rc-select";
const { TextArea } = Input;
import { useEffect, useState } from "react";
import type { DefaultOptionType } from "antd/es/select";
import type { NotificationProps } from "../../types/Notification";
import { userService } from "../../services/user/userService";
const availabilityOptions = [
  "Weekdays",
  "Weekends",
  "Mornings",
  "Afternoons",
  "Evenings",
];

const userId = "01047F62-6E87-442B-B1E8-2A54C9E17D7C";

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

export default function UserProfile() {
  const [imageUrl, setImageUrl] = useState("");
  const [notify, setNotify] = useState<NotificationProps | null>(null);
  const [selectedAvailability, setSelectedAvailability] = useState<string[]>(
    [],
  );
  const { notification } = App.useApp();
  const [tags, setTags] = useState<DefaultOptionType[]>([]);
  //   const [selectedTags, setSelectedTags] = useState([]);

  useEffect(() => {
    const options: SelectProps["options"] = [];
    for (let i = 10; i < 36; i++) {
      options.push({
        label: i.toString(36) + i,
        value: i.toString(36) + i,
      });
    }
    setTags(options);
  }, []);

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
      // Show preview
      const reader = new FileReader();
      reader.addEventListener("load", () =>
        setImageUrl(info.file.response?.value),
      );
      reader.readAsDataURL(info.file.originFileObj as RcFile);
    }
  };

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

  return (
    <div className="text-white p-6 rounded-xl max-w-3xl my-10 mx-auto shadow-lg bg-gray-800">
      <Form layout="vertical" name="user_profile_form" requiredMark={false}>
        <h1 className="text-2xl font-semibold mb-6">
          Tell us more about yourself
        </h1>
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
              action={
                import.meta.env.VITE_BASE_URL_BE + `/Users/avatar/${userId}`
              }
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
                  <Input maxLength={100} size="large" placeholder="Full Name" />
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
                { max: 300, message: "Bio must be less than 300 characters!" },
              ]}
            >
              <TextArea
                placeholder="A brief introduction about yourself..."
                maxLength={300}
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
            rules={[
              {
                required: true,
                message: "Please select your field of expertise!",
              },
            ]}
          >
            <Select
              mode="multiple"
              allowClear
              placeholder="Select your field of expertise"
              className="w-full"
              size="large"
              //   onChange={handleExpertiseChange}
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
          rules={[
            {
              required: true,
              message: "Please select your availability!",
            },
          ]}
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
            security=""
            // onClick={() => navigate("/profile")}
            className="bg-gray-500! hover:bg-gray-400!"
          >
            Back
          </Button>
          <Button
            type="primary"
            size="large"
            className="flex-1"
            onClick={() => console.log("???")}
          >
            Next Step
          </Button>
        </div>
      </Form>
    </div>
  );
}
