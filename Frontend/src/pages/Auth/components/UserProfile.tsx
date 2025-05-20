import {
  CloseOutlined,
  PlusOutlined,
  UserOutlined,
} from "@ant-design/icons/lib/icons";
import { Upload, Button, Input, Form, Select, Radio } from "antd";
import type { CheckboxGroupProps } from "antd/es/checkbox";
import type { RcFile, UploadChangeParam, UploadFile } from "antd/es/upload";
import type { SelectProps } from "rc-select";
const { TextArea } = Input;
import { useEffect, useState } from "react";
import type { DefaultOptionType } from "antd/es/select";
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

const roleOptions: CheckboxGroupProps<string>["options"] = [
  { label: "Learner", value: "learner" },
  { label: "Mentor", value: "mentor" },
];

export default function UserProfile() {
  const [imageUrl, setImageUrl] = useState("");
  const [selectedAvailability, setSelectedAvailability] = useState<string[]>(
    [],
  );

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

  const handleChange = (info: UploadChangeParam<UploadFile>) => {
    if (info.file.status === "done" || info.file.status === "uploading") {
      // Show preview
      const reader = new FileReader();
      reader.addEventListener("load", () =>
        setImageUrl(reader.result as string),
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
              beforeUpload={() => true}
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
            <Form.Item
              name="fullname"
              label="Full Name"
              rules={[
                { required: true, message: "Please enter your full name!" },
              ]}
            >
              <Input size="large" placeholder="Full Name" />
            </Form.Item>
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

        <Form.Item name="roleSelect" label="I am joining as" rules={[]}>
          <Radio.Group
            block
            options={roleOptions}
            optionType="button"
            buttonStyle="solid"
            defaultValue={"learner"}
            size="large"
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
          <div className="flex gap-2 items-center justify-center">
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
          rules={[]}
        >
          <Radio.Group
            block
            options={communicationMethodOptions}
            optionType="button"
            buttonStyle="solid"
            defaultValue={"video"}
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
      </Form>
    </div>
  );
}
