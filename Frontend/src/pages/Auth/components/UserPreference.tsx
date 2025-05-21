import { useEffect, useState } from "react"
import { Button, Checkbox, Form, Select, Tag, type SelectProps } from "antd"
import type { CheckboxChangeEvent } from "antd/es/checkbox"
import type { DefaultOptionType } from "antd/es/select"

export default function UserPreference() {

    const [learningStyle, setLearningStyle] = useState<string>("Visual")

    const [privacySettings, setPrivacySettings] = useState({
        privateProfile: false,
        allowMessages: true,
        receiveNotifications: true,
    })

    const [teachingApproaches, setTeachingApproaches] = useState<string[]>(["Hands-on Practice", "Discussion Based"])

    const handleLearningStyleClick = (style: string) => {
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
                            },
                        ]}
                    >
                        <Select
                            mode="multiple"
                            allowClear
                            placeholder="Select your field of topics"
                            className="w-full"
                            size="large"
                            options={tags}
                            style={{ background: '#1E293B' }}
                        />
                    </Form.Item>
                </div>

                <div className="grid grid-cols-1 md:grid-cols-2 gap-8 mb-8">
                    <div className="rounded-lg transition-all duration-300">
                        <h3 className="text-gray-300 mb-4 text-lg">Preferred session frequency</h3>
                        <Select
                            defaultValue="Weekly"
                            className="w-full"
                            size="large"
                            style={{ background: '#1E293B' }}
                            options={[
                                { value: "Weekly", label: "Weekly" },
                                { value: "Every two week", label: "Every two week" },
                                { value: "Monthly", label: "Monthly" },
                                { value: "As needed", label: "As needed" },
                            ]}
                        />
                    </div>
                    <div className="rounded-lg transition-all duration-300">
                        <h3 className="text-gray-300 mb-4 text-lg">Preferred session duration</h3>
                        <Select
                            defaultValue="1 hour"
                            className="w-full"
                            size="large"
                            style={{ background: '#1E293B' }}
                            options={[
                                { value: "30 minutes", label: "30 minutes" },
                                { value: "1 hour", label: "1 hour" },
                                { value: "1.5 hours", label: "1.5 hours" },
                                { value: "2 hours", label: "2 hours" },
                            ]}
                        />
                    </div>
                </div>

                <div className="mb-8 rounded-lg">
                    <h3 className="text-gray-300 mb-4 text-lg">Your preferred learning style</h3>
                    <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
                        {["Visual", "Auditory", "Reading/Writing", "Kinesthetic"].map((style) => (
                            <div
                                key={style}
                                onClick={() => handleLearningStyleClick(style)}
                                className={`py-4 px-6 rounded-lg cursor-pointer text-center transition-all duration-300 transform ${learningStyle === style
                                    ? "bg-gradient-to-r from-[#FF6B00] to-[#FF8533] text-white shadow-lg"
                                    : "bg-[#2D3748] text-gray-300 hover:bg-[#374151]"
                                    }`}
                            >
                                {style}
                            </div>
                        ))}
                    </div>
                </div>

                <div className="mb-8 rounded-lg">
                    <h3 className="text-lg mb-2">
                        Your Teaching Approach
                    </h3>
                    <p className="text-gray-400 mb-4">Select all teaching methods that match your style</p>
                    <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                        {[
                            { id: "hands-on", label: "Hands-on Practice", icon: "⚒️", description: "Learn by doing" },
                            { id: "discussion", label: "Discussion Based", icon: "💬", description: "Interactive dialogues" },
                            { id: "project", label: "Project Based", icon: "📋", description: "Real-world applications" },
                            { id: "lecture", label: "Lecture Style", icon: "📝", description: "Structured learning" },
                        ].map((approach) => (
                            <Tag.CheckableTag
                                key={approach.id}
                                checked={teachingApproaches.includes(approach.label)}
                                onChange={(checked) => handleTeachingApproachChange(approach.label, checked)}
                                className={`group p-4 rounded-xl cursor-pointer text-left transition-all duration-300 transform ${teachingApproaches.includes(approach.label)
                                    ? "!bg-gradient-to-r from-[#FF6B00] to-[#FF8533] !text-white shadow-lg"
                                    : "!bg-[#2D3748] !text-gray-300 hover:!bg-[#374151]"
                                    }`}
                            >
                                <div className="flex items-center py-2 space-x-3">
                                    <span className="text-xl">
                                        {approach.icon}
                                    </span>
                                    <div>
                                        <div className="font-semibold text-lg">{approach.label}</div>
                                        <div className="text-sm text-gray-400">{approach.description}</div>
                                    </div>
                                </div>
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