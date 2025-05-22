"use client"

import { ShareAltOutlined } from "@ant-design/icons/lib/icons"
import { Button, Tag, Spin } from "antd"
import { useEffect, useState, useContext } from "react"
import { useNavigate } from "react-router-dom"
import UserProfileClient, { type UserProfile as UserProfileType } from "../userProfileClient"
import { AuthContext } from "../../../contexts/AuthContext" 

interface ProfileProps {
  userId?: string
}

const expertiseMap: Record<string, string> = {
  "ecbea5a8-62a1-48d8-fb6d-08dd9819fec3": "Business",
  "9ea17990-b9b6-4041-fb6f-08dd9819fec3": "Communication",
  "2b7ac1a6-29c6-4b74-fb6c-08dd9819fec3": "Data Science",
  "02d3db0e-0ce1-493e-fb6a-08dd9819fec3": "Design",
  "8f0585c6-6b78-46dc-fb68-08dd9819fec3": "Leadership",
  "5a1f1d05-8e7c-4dce-fb6b-08dd9819fec3": "Marketing",
  "5c9dd8a1-a6f3-4fdd-fb69-08dd9819fec3": "Programming",
  "a36ffa6c-b908-4b70-fb6e-08dd9819fec3": "Project Management",
};

const availabilityMap: Record<string, string> = {
  "67eb556e-d860-498d-212a-08dd9819fea6": "Weekdays",
  "3eab46db-9a91-4ab7-212b-08dd9819fea6": "Weekends",
  "eec61c62-5198-4e1d-212c-08dd9819fea6": "Mornings",
  "9574cf95-2df0-4982-212d-08dd9819fea6": "Afternoons",
  "25cfac78-2bc0-46bf-212e-08dd9819fea6": "Evenings",
};


const communicationMethodMap: Record<number, string> = {
  0: "Video Call",
  1: "Audio Call",
  2: "Text Chat",
};

export default function UserProfile({ userId: propUserId }: ProfileProps) {
  const [userData, setUserData] = useState<UserProfileType | null>(null)
  const [loading, setLoading] = useState<boolean>(true)
  const navigate = useNavigate()
  const { user, isAuthenticated } = useContext(AuthContext) 
  const token = localStorage.getItem("token")

  useEffect(() => {
    const fetchUserData = async () => {
      try {
        setLoading(true)
        if (!isAuthenticated || !user) {
          throw new Error("User not authenticated")
        }

        const currentUserId = propUserId || user.id 
        if (!currentUserId) {
          throw new Error("User ID not found")
        }

        if (!token) {
          throw new Error("Token not found")
        }
        const userProfileData = await UserProfileClient.getProfile(currentUserId, token)
        setUserData(userProfileData)
        setLoading(false)
      } catch (error) {
        console.error("Error fetching user data:", error)
        setLoading(false)
        navigate("/login") 
      }
    }

    fetchUserData()
  }, [propUserId, user, isAuthenticated, token, navigate])

  const handleShareProfile = () => {
    console.log("Sharing profile...")
  }

  if (loading) {
    return (
      <div className="flex items-center justify-center h-screen">
        <Spin size="large" />
      </div>
    )
  }

  const getExpertiseNames = (ids: string[] = []) => {
    return ids.map((id) => expertiseMap[id] || `Expertise ${id}`);
  }

  const getAvailabilityNames = (ids: string[] = []) => {
    return ids.map((id) => availabilityMap[id] || `Availability ${id}`);
  }

  const getCommunicationMethod = (id = 0) => {
    return communicationMethodMap[id] || "Not specified";
  }

  return (
    <div className="text-white p-6 rounded-xl max-w-3xl my-10 mx-auto shadow-lg bg-gray-800">
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
            onClick={() => navigate(`/profile/edit/${userData?.id || ""}`)}
            className="bg-orange-500 hover:bg-orange-600"
          >
            Edit Profile
          </Button>
        </div>
      </div>

      <div className="flex items-center gap-6 mb-8">
        <div className="w-24 h-24 rounded-full bg-gray-700 flex items-center justify-center overflow-hidden">
          {userData?.profilePhotoUrl ? (
            <img
              src={userData.profilePhotoUrl || "/placeholder.svg"}
              alt="Profile"
              className="w-full h-full object-cover"
            />
          ) : (
            <div className="text-gray-400">
              <svg width="40" height="40" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                <path
                  d="M12 12C14.21 12 16 10.21 16 8C16 5.79 14.21 4 12 4C9.79 4 8 5.79 8 8C8 10.21 9.79 12 12 12ZM12 14C9.33 14 4 15.34 4 18V20H20V18C20 15.34 14.67 14 12 14Z"
                  fill="currentColor"
                />
              </svg>
            </div>
          )}
        </div>
        <div>
          <h2 className="text-2xl font-medium">{userData?.fullName || "Loading..."}</h2>
          <Tag color="orange" className="mt-1">
            {userData?.roleId === 3 ? "Learner" : userData?.roleId === 2 ? "Mentor" : "Unknown"}
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
          {userData?.expertiseIds && userData.expertiseIds.length > 0 ? (
            getExpertiseNames(userData.expertiseIds).map((item) => (
              <Tag key={item} className="bg-gray-700 text-white border-none px-4 py-1 rounded-full">
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
        <p>{userData?.experiences || "No experience provided."}</p>
      </div>

      <div className="mb-8">
        <h3 className="text-gray-400 mb-2">Availability</h3>
        <div className="flex flex-wrap gap-2">
          {userData?.availabilityIds && userData.availabilityIds.length > 0 ? (
            getAvailabilityNames(userData.availabilityIds).map((item) => (
              <Tag key={item} className="bg-gray-700 text-white border-none px-4 py-1 rounded-full">
                {item}
              </Tag>
            ))
          ) : (
            <p>No availability provided.</p>
          )}
        </div>
      </div>

      <div className="mb-8">
        <h3 className="text-gray-400 mb-2">Preferred Communication</h3>
        <p>{getCommunicationMethod(userData?.preferredCommunicationMethod)}</p>
      </div>

      <div className="mb-8">
        <h3 className="text-gray-400 mb-2">Learning Goal</h3>
        <p>{userData?.goal || "No goal provided."}</p>
      </div>

      <div className="pt-4 border-t border-gray-700">
        <Button
          type="link"
          className="text-orange-500 flex items-center gap-1 pl-0"
          onClick={() => console.log("View additional settings")}
        >
          View Additional Profile Settings
          <svg width="16" height="16" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
            <path d="M8.59 16.59L13.17 12L8.59 7.41L10 6L16 12L10 18L8.59 16.59Z" fill="currentColor" />
          </svg>
        </Button>
      </div>
      <div className="pt-4 border-gray-700">
        <Button type="primary" size="large" onClick={() => navigate("/")} className="bg-orange-500 hover:bg-orange-600">
          Back
        </Button>
      </div>
    </div>
  )
}