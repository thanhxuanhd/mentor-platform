import { useState } from "react"
import { Button, Checkbox, Select, Progress } from "antd"
import type { CheckboxChangeEvent } from "antd/es/checkbox"

export default function UserPreference() {
    // State for selected topics
    const [selectedTopics, setSelectedTopics] = useState<string[]>([])

    // State for learning style
    const [learningStyle, setLearningStyle] = useState<string>("Visual")

    // State for privacy settings
    const [privacySettings, setPrivacySettings] = useState({
        privateProfile: false,
        allowMessages: true,
        receiveNotifications: true,
    })

    // Handle topic selection
    const handleTopicClick = (topic: string) => {
        if (selectedTopics.includes(topic)) {
            setSelectedTopics(selectedTopics.filter((t) => t !== topic))
        } else {
            setSelectedTopics([...selectedTopics, topic])
        }
    }

    // Handle learning style selection
    const handleLearningStyleClick = (style: string) => {
        setLearningStyle(style)
    }

    // Handle privacy settings changes
    const handlePrivacyChange = (setting: keyof typeof privacySettings) => (e: CheckboxChangeEvent) => {
        setPrivacySettings({
            ...privacySettings,
            [setting]: e.target.checked,
        })
    }

    return (
        <div className="min-h-screen bg-[#111827] text-white">
            {/* Header */}
            <header className="flex justify-between items-center p-4 border-b border-gray-800">
                <h1 className="text-2xl font-bold text-[#FF6B00]">MentorConnect</h1>
                <div className="flex items-center">
                    <Progress percent={100} showInfo={false} strokeColor="#FF6B00" trailColor="#2D3748" className="w-40 mr-4" />
                    <span className="text-gray-300">Step 3 of 3</span>
                </div>
            </header>

            {/* Main Content */}
            <main className="max-w-3xl mx-auto p-8 mt-8 bg-[#1A2235] rounded-lg">
                <h2 className="text-2xl font-semibold mb-8">Set Your Preferences</h2>

                {/* Topics Section */}
                <div className="mb-8">
                    <h3 className="text-gray-400 mb-4">Topics you're interested in learning about</h3>
                    <div className="grid grid-cols-2 md:grid-cols-3 gap-3">
                        {[
                            "Career Development",
                            "Technical Skills",
                            "Leadership",
                            "Communication",
                            "Work-Life Balance",
                            "Industry Insights",
                            "Networking",
                            "Entrepreneurship",
                        ].map((topic) => (
                            <div
                                key={topic}
                                onClick={() => handleTopicClick(topic)}
                                className={`py-2 px-4 rounded cursor-pointer text-center transition-colors ${selectedTopics.includes(topic)
                                    ? "bg-[#2D3748] text-white"
                                    : "bg-[#1E293B] text-gray-300 hover:bg-[#2D3748]"
                                    }`}
                            >
                                {topic}
                            </div>
                        ))}
                    </div>
                </div>

                {/* Session Preferences */}
                <div className="grid grid-cols-1 md:grid-cols-2 gap-6 mb-8">
                    <div>
                        <h3 className="text-gray-400 mb-4">Preferred session frequency</h3>
                        <Select
                            defaultValue="Weekly"
                            className="w-full bg-[#1E293B]"
                            options={[
                                { value: "Weekly", label: "Weekly" },
                                { value: "Bi-weekly", label: "Bi-weekly" },
                                { value: "Monthly", label: "Monthly" },
                            ]}
                            dropdownStyle={{ background: "#1E293B", color: "white" }}
                        />
                    </div>
                    <div>
                        <h3 className="text-gray-400 mb-4">Preferred session duration</h3>
                        <Select
                            defaultValue="1 hour"
                            className="w-full bg-[#1E293B]"
                            options={[
                                { value: "30 minutes", label: "30 minutes" },
                                { value: "1 hour", label: "1 hour" },
                                { value: "1.5 hours", label: "1.5 hours" },
                                { value: "2 hours", label: "2 hours" },
                            ]}
                            dropdownStyle={{ background: "#1E293B", color: "white" }}
                        />
                    </div>
                </div>

                {/* Learning Style */}
                <div className="mb-8">
                    <h3 className="text-gray-400 mb-4">Your preferred learning style</h3>
                    <div className="grid grid-cols-2 md:grid-cols-4 gap-3">
                        {["Visual", "Auditory", "Reading/Writing", "Kinesthetic"].map((style) => (
                            <div
                                key={style}
                                onClick={() => handleLearningStyleClick(style)}
                                className={`py-3 px-4 rounded cursor-pointer text-center transition-colors ${learningStyle === style ? "bg-[#FF6B00] text-white" : "bg-[#1E293B] text-gray-300 hover:bg-[#2D3748]"
                                    }`}
                            >
                                {style}
                            </div>
                        ))}
                    </div>
                </div>

                {/* Privacy Settings */}
                <div className="mb-8">
                    <h3 className="text-xl font-semibold mb-4">Privacy settings</h3>
                    <div className="space-y-4">
                        <div>
                            <Checkbox
                                checked={privacySettings.privateProfile}
                                onChange={handlePrivacyChange("privateProfile")}
                                className="text-white"
                            >
                                <span className="text-white font-medium">Private profile</span>
                            </Checkbox>
                            <p className="text-gray-400 text-sm ml-6">Only approved connections can view your full profile details</p>
                        </div>
                        <div>
                            <Checkbox
                                checked={privacySettings.allowMessages}
                                onChange={handlePrivacyChange("allowMessages")}
                                className="text-white"
                            >
                                <span className="text-white font-medium">Allow messages</span>
                            </Checkbox>
                            <p className="text-gray-400 text-sm ml-6">Let others initiate contact with you through messages</p>
                        </div>
                        <div>
                            <Checkbox
                                checked={privacySettings.receiveNotifications}
                                onChange={handlePrivacyChange("receiveNotifications")}
                                className="text-white"
                            >
                                <span className="text-white font-medium">Receive notifications</span>
                            </Checkbox>
                            <p className="text-gray-400 text-sm ml-6">
                                Get email and in-app notifications for messages, session requests, and updates
                            </p>
                        </div>
                    </div>
                </div>

                {/* Navigation Buttons */}
                <div className="flex justify-between mt-8">
                    <Button className="bg-[#1E293B] text-white border-none hover:bg-[#2D3748] h-10 px-6">Back</Button>
                    <Button type="primary" className="bg-[#FF6B00] text-white border-none hover:bg-[#E05E00] h-10 px-6">
                        Complete Registration
                    </Button>
                </div>
            </main>
        </div>
    )
}