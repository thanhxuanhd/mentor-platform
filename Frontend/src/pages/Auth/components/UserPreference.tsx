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
                <h2 className="text-3xl font-bold mb-8 text-transparent bg-clip-text bg-gradient-to-r from-[#FF6B00] to-[#FF8533]">
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
                    <div className="bg-[#1E293B] p-6 rounded-lg transition-all duration-300 hover:shadow-xl">
                        <h3 className="text-gray-300 mb-4 text-lg font-semibold">Preferred session frequency</h3>
                        <Select
                            defaultValue="Weekly"
                            className="w-full"
                            style={{ background: '#1E293B' }}
                            options={[
                                { value: "Weekly", label: "Weekly" },
                                { value: "Every two week", label: "Every two week" },
                                { value: "Monthly", label: "Monthly" },
                                { value: "As needed", label: "As needed" },
                            ]}
                        />
                    </div>
                    <div className="bg-[#1E293B] p-6 rounded-lg transition-all duration-300 hover:shadow-xl">
                        <h3 className="text-gray-300 mb-4 text-lg font-semibold">Preferred session duration</h3>
                        <Select
                            defaultValue="1 hour"
                            className="w-full"
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

                <div className="mb-8 bg-[#1E293B] p-6 rounded-lg shadow-xl">
                    <h3 className="text-gray-300 mb-6 font-semibold text-xl">Your preferred learning style</h3>
                    <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
                        {["Visual", "Auditory", "Reading/Writing", "Kinesthetic"].map((style) => (
                            <div
                                key={style}
                                onClick={() => handleLearningStyleClick(style)}
                                className={`py-4 px-6 rounded-lg cursor-pointer text-center transition-all duration-300 transform hover:scale-105 ${learningStyle === style
                                    ? "bg-gradient-to-r from-[#FF6B00] to-[#FF8533] text-white shadow-lg"
                                    : "bg-[#2D3748] text-gray-300 hover:bg-[#374151]"
                                    }`}
                            >
                                {style}
                            </div>
                        ))}
                    </div>
                </div>

                <div className="mb-8 bg-[#1E293B] p-6 rounded-lg shadow-xl">
                    <h3 className="text-transparent bg-clip-text bg-gradient-to-r from-[#FF6B00] to-[#FF8533] text-2xl font-semibold mb-6">
                        Your Teaching Approach
                    </h3>
                    <p className="text-gray-400 mb-6">Select all teaching methods that match your style</p>
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
                                className={`group p-4 rounded-xl cursor-pointer text-left transition-all duration-300 transform hover:scale-105 ${teachingApproaches.includes(approach.label)
                                    ? "!bg-gradient-to-r from-[#FF6B00] to-[#FF8533] !text-white shadow-lg"
                                    : "!bg-[#2D3748] !text-gray-300 hover:!bg-[#374151]"
                                    }`}
                            >
                                <div className="flex items-center py-2 space-x-3">
                                    <span className="text-2xl group-hover:scale-110 transition-transform">
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

                <div className="mb-8 bg-[#1E293B] p-8 rounded-xl shadow-2xl">
                    <h3 className="text-transparent bg-clip-text bg-gradient-to-r from-[#FF6B00] to-[#FF8533] text-2xl font-semibold mb-6">Privacy Settings</h3>
                    <div className="space-y-6">
                        <div className="transform transit</div>ion-all duration-300 hover:translate-x-2">
                            <Checkbox
                                checked={privacySettings.privateProfile}
                                onChange={handlePrivacyChange("privateProfile")}
                                className="text-white scale-110"
                            >
                                <span className="text-white font-semibold text-lg">Private Profile</span>
                            </Checkbox>
                            <p className="text-gray-400 text-sm ml-6 mt-2">Only approved connections can view your full profile details</p>
                        </div>
                        <div className="transform transition-all duration-300 hover:translate-x-2">
                            <Checkbox
                                checked={privacySettings.allowMessages}
                                onChange={handlePrivacyChange("allowMessages")}
                                className="text-white scale-110"
                            >
                                <span className="text-white font-semibold text-lg">Allow Messages</span>
                            </Checkbox>
                            <p className="text-gray-400 text-sm ml-6 mt-2">Let others initiate contact with you through messages</p>
                        </div>
                        <div className="transform transition-all duration-300 hover:translate-x-2">
                            <Checkbox
                                checked={privacySettings.receiveNotifications}
                                onChange={handlePrivacyChange("receiveNotifications")}
                                className="text-white scale-110"
                            >
                                <span className="text-white font-semibold text-lg">Receive Notifications</span>
                            </Checkbox>
                            <p className="text-gray-400 text-sm ml-6 mt-2">
                                Get email and in-app notifications for messages, session requests, and updates
                            </p>
                        </div>
                    </div>
                </div>

                <div className="flex justify-between mt-8">
                    <Button
                        className="bg-[#1E293B] text-white border-none hover:bg-[#2D3748] h-12 px-8 text-lg rounded-lg
                        transform transition-all duration-300 hover:scale-105 hover:shadow-lg"
                    >
                        Back
                    </Button>
                    <Button
                        type="primary"
                        className="bg-gradient-to-r from-[#FF6B00] to-[#FF8533] text-white border-none h-12 px-8 text-lg rounded-lg
                        transform transition-all duration-300 hover:scale-105 hover:shadow-lg hover:from-[#FF8533] hover:to-[#FF6B00]"
                    >
                        Complete Registration
                    </Button>
                </div>
            </Form>
        </div>
    )
}