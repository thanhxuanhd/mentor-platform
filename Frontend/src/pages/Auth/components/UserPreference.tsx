import { useEffect, useState } from "react"
import { Button, Checkbox, Form, Select, Tag, type SelectProps } from "antd"
import type { CheckboxChangeEvent } from "antd/es/checkbox"
import { getListCategories } from "../../../services/category/categoryServices";
import type { TeachingApproach } from "../../../types/UserTypes";
import { getAllTeachingApproaches } from "../../../services/user/userService";
import { learningStyleDisplayMap, learningStyleValues } from "../../../types/enums/LearningStyle";

export default function UserPreference() {
  const [learningStyle, setLearningStyle] = useState<number>(0);
  const [tags, setTags] = useState<SelectProps['options']>([]);
  const [selectedTags, setSelectedTags] = useState([]);
  const [teachingApproaches, setTeachingApproaches] = useState<string[]>([]);
  const [approaches, setApproaches] = useState<TeachingApproach[]>([]);
  const [selectedFrequency, setSelectedFrequency] = useState(0);
  const [selectedDuration, setSelectedDuration] = useState(30);

  useEffect(() => {
    const fetchApproaches = async () => {
      try {
        const data = await getAllTeachingApproaches();
        setApproaches(data);
        setTeachingApproaches([]);
      } catch (err) {
        console.error(err);
      }
    };
    fetchCategories();
    fetchApproaches();
  }, []);

  const fetchCategories = async (keyword: string = '') => {
    try {
      const response = await getListCategories(1, 5, keyword);
      setTags(
        response.items.map((category: { name: any; id: any }) => ({
          label: category.name,
          value: category.id,
        }))
      );
    } catch (error) {
      console.error('Error fetching categories:', error);
    }
  };

  const [privacySettings, setPrivacySettings] = useState({
    privateProfile: false,
    allowMessages: true,
    receiveNotifications: true,
  })

  const handleLearningStyleClick = (style: number) => {
    setLearningStyle(style)
  }

  const handlePrivacyChange = (setting: keyof typeof privacySettings) => (e: CheckboxChangeEvent) => {
    setPrivacySettings({
      ...privacySettings,
      [setting]: e.target.checked,
    })
  }

  const handleTeachingApproachChange = (approach: string, checked: boolean) => {
    if (checked) {
      setTeachingApproaches([...teachingApproaches, approach])
    } else {
      setTeachingApproaches(teachingApproaches.filter((a) => a !== approach))
    }
  }

  const handleSearch = (value: string) => {
    fetchCategories(value);
  };

  const handleFrequencyChange = (value: number) => {
    setSelectedFrequency(value);
  }

  const handleDurationChange = (value: number) => {
    setSelectedDuration(value);
  }
  return (
    <div className="text-white p-8 rounded-xl max-w-3xl my-12 mx-auto shadow-2xl bg-gradient-to-b from-gray-800 to-gray-900">
      <Form layout="vertical" name="user_profile_form" requiredMark={false}>
        <h2 className="text-2xl font-bold mb-8">
          Set Your Preferences
        </h2>

        <div className="mb-6">
          <Form.Item
            name="topics"
            label={<span className="text-gray-300 text-lg">Topics you're interested in learning about</span>}
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
              style={{ background: '#1E293B' }}
              filterOption={false}
              onSearch={handleSearch}
              onChange={(value) => setSelectedTags(value)}
              value={selectedTags}
              maxCount={5}
            />
          </Form.Item>
        </div>

        <div className="grid grid-cols-1 md:grid-cols-2 gap-8 mb-8">
          <div className="rounded-lg transition-all duration-300">
            <h3 className="text-gray-300 mb-4 text-lg">Preferred session frequency</h3>
            <Select
              defaultValue={0}
              className="w-full"
              size="large"
              style={{ background: '#1E293B' }}
              onChange={handleFrequencyChange}
              options={[
                { value: 0, label: 'Weekly' },
                { value: 1, label: 'Every two weeks' },
                { value: 2, label: 'Monthly' },
                { value: 3, label: 'As needed' },
              ]}
              value={selectedFrequency}
            />
          </div>
          <div className="rounded-lg transition-all duration-300">
            <h3 className="text-gray-300 mb-4 text-lg">Preferred session duration</h3>
            <Select
              defaultValue={30}
              className="w-full"
              size="large"
              style={{ background: '#1E293B' }}
              onChange={handleDurationChange}
              options={[
                { value: 30, label: "30 minutes" },
                { value: 60, label: "1 hour" },
                { value: 90, label: "1.5 hours" },
                { value: 120, label: "2 hours" },
              ]}
              value={selectedDuration}
            />
          </div>
        </div>

        <div className="mb-8 rounded-lg">
          <h3 className="text-gray-300 mb-4 text-lg">Your preferred learning style</h3>
          <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
            {learningStyleValues.map((style) => (
              <div
                key={style}
                onClick={() => handleLearningStyleClick(style)}
                className={`py-4 px-6 rounded-lg cursor-pointer text-center transition-all duration-300 transform ${learningStyle === style
                  ? "bg-gradient-to-r from-[#FF6B00] to-[#FF8533] text-white shadow-lg"
                  : "bg-[#2D3748] text-gray-300 hover:bg-[#374151]"
                  }`}
              >
                {learningStyleDisplayMap[style]}
              </div>
            ))}
          </div>
        </div>

        <div className="mb-8 rounded-lg">
          <h3 className="text-lg mb-2">Your Teaching Approach</h3>
          <p className="text-gray-400 mb-4">Select all teaching methods that match your style</p>
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            {approaches.map((approach) => (
              <Tag.CheckableTag
                key={approach.id}
                checked={teachingApproaches.includes(approach.name)}
                onChange={(checked) => handleTeachingApproachChange(approach.name, checked)}
                className={`group p-8 rounded-2xl cursor-pointer text-left transition-all duration-300 transform ${teachingApproaches.includes(approach.name)
                  ? '!bg-gradient-to-r from-[#FF6B00] to-[#FF8533] !text-white shadow-lg'
                  : '!bg-[#2D3748] !text-gray-300 hover:!bg-[#374151]'
                  }`}
              >
                <div className="text-lg m-4">{approach.name}</div>
              </Tag.CheckableTag>
            ))}
          </div>
        </div>

        <div className="mb-8 rounded-xl">
          <h3 className="text-xl mb-4">Privacy Settings</h3>
          <div className="space-y-6">
            <Checkbox
              checked={privacySettings.privateProfile}
              onChange={handlePrivacyChange("privateProfile")}
              className="text-white scale-110"
            >
              <span className="text-white text-md">Private Profile</span>
            </Checkbox>
            <p className="text-gray-400 text-sm ml-6 mt-2">Only approved connections can view your full profile details</p>
            <Checkbox
              checked={privacySettings.allowMessages}
              onChange={handlePrivacyChange("allowMessages")}
              className="text-white scale-110"
            >
              <span className="text-white text-md">Allow Messages</span>
            </Checkbox>
            <p className="text-gray-400 text-sm ml-6 mt-2">Let others initiate contact with you through messages</p>
            <Checkbox
              checked={privacySettings.receiveNotifications}
              onChange={handlePrivacyChange("receiveNotifications")}
              className="text-white scale-110"
            >
              <span className="text-white text-md">Receive Notifications</span>
            </Checkbox>
            <p className="text-gray-400 text-sm ml-6 mt-2">
              Get email and in-app notifications for messages, session requests, and updates
            </p>
          </div>
        </div>

        <div className="flex justify-between mt-8">
          <Button
            className="h-12 px-8 text-lg rounded-lg"
            size="large"
          >
            Back
          </Button>
          <Button
            type="primary"
            className=" text-white border-none h-12 px-8 text-lg rounded-lg"
            size="large"
          >
            Complete Registration
          </Button>
        </div>
      </Form>
    </div>
  )
}