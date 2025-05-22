import { useEffect, useState } from "react";
import { Checkbox, Form, Select, Tag, type SelectProps } from "antd";
import type { CheckboxChangeEvent } from "antd/es/checkbox";
import { getListCategories } from "../../../services/category/categoryServices";
import type { TeachingApproach, UserDetail } from "../../../types/UserTypes";
import { getAllTeachingApproaches } from "../../../services/user/userService";
import { LearningStyle } from "../../../types/enums/LearningStyle";
import { SessionFrequency } from "../../../types/enums/SessionFrequency"; // Import the SessionFrequency enum

interface UserProfileProps {
  userId: string;
  userDetail: UserDetail;
  updateUserDetail: React.Dispatch<React.SetStateAction<UserDetail>>;
}

const UserPreference: React.FC<UserProfileProps> = ({
  userDetail,
  updateUserDetail,
}) => {
  const [tags, setTags] = useState<SelectProps["options"]>([]);
  const [approaches, setApproaches] = useState<TeachingApproach[]>([]);

  useEffect(() => {
    const fetchApproaches = async () => {
      try {
        const data = await getAllTeachingApproaches();
        setApproaches(data);
      } catch (err) {
        console.error(err);
      }
    };
    fetchCategories();
    fetchApproaches();
  }, []);

  const fetchCategories = async (keyword: string = "") => {
    try {
      const response = await getListCategories(1, 5, keyword);
      setTags(
        response.items.map((category: { name: any; id: any }) => ({
          label: category.name,
          value: category.id,
        })),
      );
    } catch (error) {
      console.error("Error fetching categories:", error);
    }
  };
  const learningStyleOptions = Object.entries(LearningStyle).map(
    ([key, value]) => ({
      value: value,
      label: key.replace(/([A-Z])/g, " $1").trim(),
    }),
  );

  const handleLearningStyleClick = (style: LearningStyle) => {
    updateUserDetail((prev) => ({
      ...prev,
      preferredLearningStyle: style,
    }));
  };

  const handlePrivacyChange =
    (
      setting: keyof Pick<
        UserDetail,
        "isPrivate" | "isAllowedMessage" | "isReceiveNotification"
      >,
    ) =>
      (e: CheckboxChangeEvent) => {
        updateUserDetail((prev) => ({
          ...prev,
          [setting]: e.target.checked,
        }));
      };

  const handleTeachingApproachChange = (
    approachId: string,
    checked: boolean,
  ) => {
    updateUserDetail((prev) => ({
      ...prev,
      teachingApproachIds: checked
        ? [...prev.teachingApproachIds, approachId]
        : prev.teachingApproachIds.filter((id) => id !== approachId),
    }));
  };

  const handleSearch = (value: string) => {
    fetchCategories(value);
  };

  const handleFrequencyChange = (value: SessionFrequency) => {
    updateUserDetail((prev) => ({
      ...prev,
      preferredSessionFrequency: value,
    }));
  };

  const handleDurationChange = (value: number) => {
    updateUserDetail((prev) => ({
      ...prev,
      preferredSessionDuration: value,
    }));
  };

  const frequencyOptions = Object.entries(SessionFrequency).map(
    ([key, value]) => ({
      value: value,
      label: key.replace(/([A-Z])/g, " $1").trim(),
    }),
  );

  return (
    <div className="text-white p-8 rounded-xl max-w-3xl my-6 mx-auto shadow-2xl bg-gray-800">
      <Form layout="vertical" name="user_profile_form" requiredMark={false}>
        <h2 className="text-2xl font-bold mb-8">Set Your Preferences</h2>

        <div className="mb-6">
          <Form.Item
            name="topics"
            label={
              <span className="text-gray-300 text-lg">
                Topics you're interested in learning about
              </span>
            }
            rules={[
              {
                required: true,
                message: "Please select your field of topics!",
              }
            ]}
          >
            <Select
              mode="multiple"
              allowClear
              showSearch
              placeholder="Search and select topics"
              className="w-full"
              size="large"
              options={tags}
              style={{ background: "#1E293B" }}
              filterOption={false}
              onSearch={handleSearch}
              onChange={(value) =>
                updateUserDetail((prev) => ({ ...prev, categoryIds: value }))
              }
              value={userDetail.categoryIds}
              maxCount={5}
            />
          </Form.Item>
        </div>

        <div className="grid grid-cols-1 md:grid-cols-2 gap-8 mb-8">
          <div className="rounded-lg transition-all duration-300">
            <h3 className="text-gray-300 mb-4 text-lg">
              Preferred session frequency
            </h3>
            <Select
              className="w-full"
              size="large"
              style={{ background: "#1E293B" }}
              onChange={handleFrequencyChange}
              options={frequencyOptions}
              value={userDetail.preferredSessionFrequency}
            />
          </div>
          <div className="rounded-lg transition-all duration-300">
            <h3 className="text-gray-300 mb-4 text-lg">
              Preferred session duration
            </h3>
            <Select
              className="w-full"
              size="large"
              style={{ background: "#1E293B" }}
              onChange={handleDurationChange}
              options={[
                { value: 30, label: "30 minutes" },
                { value: 60, label: "1 hour" },
                { value: 90, label: "1.5 hours" },
                { value: 120, label: "2 hours" },
              ]}
              value={userDetail.preferredSessionDuration}
            />
          </div>
        </div>

        <div className="mb-8 rounded-lg">
          <h3 className="text-gray-300 mb-4 text-lg">
            Your preferred learning style
          </h3>
          <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
            {learningStyleOptions.map((style) => (
              <div
                key={style.label}
                onClick={() => handleLearningStyleClick(style.value)}
                className={`py-4 px-6 rounded-lg cursor-pointer text-center transition-all duration-300 transform ${userDetail.preferredLearningStyle === style.value
                    ? "bg-gradient-to-r from-[#FF6B00] to-[#FF8533] text-white shadow-lg"
                    : "bg-[#2D3748] text-gray-300 hover:bg-[#374151]"
                  }`}
              >
                {style.label}
              </div>
            ))}
          </div>
        </div>
        {userDetail.roleId === 2 && (
          <div className="mb-8 rounded-lg">
            <h3 className="text-lg mb-2">Your Teaching Approach</h3>
            <p className="text-gray-400 mb-4">
              Select all teaching methods that match your style
            </p>
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              {approaches.map((approach) => (
                <Tag.CheckableTag
                  key={approach.id}
                  checked={userDetail.teachingApproachIds.includes(approach.id)}
                  onChange={(checked) =>
                    handleTeachingApproachChange(approach.id, checked)
                  }
                  className={`group p-8 rounded-2xl cursor-pointer text-left transition-all duration-300 transform ${userDetail.teachingApproachIds.includes(approach.id)
                      ? "!bg-gradient-to-r from-[#FF6B00] to-[#FF8533] !text-white shadow-lg"
                      : "!bg-[#2D3748] !text-gray-300 hover:!bg-[#374151]"
                    }`}
                >
                  <div className="text-lg m-4">{approach.name}</div>
                </Tag.CheckableTag>
              ))}
            </div>
          </div>
        )}

        <div className="mb-8 rounded-xl">
          <h3 className="text-xl mb-4">Privacy Settings</h3>
          <div className="space-y-6">
            <Checkbox
              checked={userDetail.isPrivate}
              onChange={handlePrivacyChange("isPrivate")}
              className="text-white scale-110"
            >
              <span className="text-white text-md">Private Profile</span>
            </Checkbox>
            <p className="text-gray-400 text-sm ml-6 mt-2">
              Only approved connections can view your full profile details
            </p>
            <Checkbox
              checked={userDetail.isAllowedMessage}
              onChange={handlePrivacyChange("isAllowedMessage")}
              className="text-white scale-110"
            >
              <span className="text-white text-md">Allow Messages</span>
            </Checkbox>
            <p className="text-gray-400 text-sm ml-6 mt-2">
              Let others initiate contact with you through messages
            </p>
            <Checkbox
              checked={userDetail.isReceiveNotification}
              onChange={handlePrivacyChange("isReceiveNotification")}
              className="text-white scale-110"
            >
              <span className="text-white text-md">Receive Notifications</span>
            </Checkbox>
            <p className="text-gray-400 text-sm ml-6 mt-2">
              Get email and in-app notifications for messages, session requests,
              and updates
            </p>
          </div>
        </div>
      </Form>
    </div>
  );
};

export default UserPreference;
