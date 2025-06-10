"use client"

import { useEffect, useState, useContext } from "react"
import { useNavigate } from "react-router-dom"
import { Button, Tag, Spin } from "antd"

import { userService } from "../../../services/user/userService"
import { getListCategories } from "../../../services/category/categoryServices"
import { AuthContext } from "../../../contexts/AuthContext"

import type { UserProfile as UserProfileType } from "../../../types/UserTypes" 

interface ProfileProps {
  userId?: string
}

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

  const [expertises, setExpertises] = useState<{ id: string; name: string }[]>([]);
  const [availabilities, setAvailabilities] = useState<{ id: string; name: string }[]>([]);
  const [teachingApproaches, setTeachingApproaches] = useState<{ id: string; name: string }[]>([]);
  const [categories, setCategories] = useState<{ id: string; name: string }[]>([]);


  useEffect(() => {
    const fetchDropdownData = async () => {
      if (!token) {
        console.error("Authentication token not found.");
        setLoading(false);
        return;
      }
      try {
        const [fetchedAvailabilities, fetchedExpertises, fetchedTeachingApproaches, categoriesResult] = await Promise.all([
          userService.getAvailabilities(token),
          userService.getExpertises(token),
          userService.getTeachingApproaches(token),
          getListCategories(1, 100),
        ]);
        setAvailabilities(fetchedAvailabilities);
        setExpertises(fetchedExpertises);
        setTeachingApproaches(fetchedTeachingApproaches);
        const categoriesData = categoriesResult?.items || [];
        setCategories(categoriesData);
      } catch (error) {
        console.error("Error fetching dropdown data:", error);
      }
    };

    fetchDropdownData();
  }, [token]);

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
        const userProfileData = await userService.getUserProfile(currentUserId)
        setUserData(userProfileData)
        setLoading(false)
      } catch (error) {
        console.error("Error fetching user data:", error)
        setLoading(false)
        navigate("/login")
      }
  };

    if (availabilities.length > 0 && expertises.length > 0 && teachingApproaches.length > 0 && categories.length > 0) {
      fetchUserData();
    }
  }, [propUserId, user, isAuthenticated, token, navigate, availabilities, expertises, teachingApproaches, categories])

  if (loading) {
    return (
      <div className="flex items-center justify-center h-screen">
        <Spin size="large" />
      </div>
    )
  }

  const getExpertiseNames = (ids: string[] = []) => {
    return ids.map((id) => {
      const expertise = expertises.find(exp => exp.id === id);
      return expertise ? expertise.name : `Expertise ${id}`;
    });
  }

  const getAvailabilityNames = (ids: string[] = []) => {
    return ids.map((id) => {
      const availability = availabilities.find(avail => avail.id === id);
      return availability ? availability.name : `Availability ${id}`;
    });
  }

  const getTeachingApproachNames = (ids: string[] = []) => {
    return ids.map((id) => {
      const approach = teachingApproaches.find(ta => ta.id === id);
      return approach ? approach.name : `Teaching Approach ${id}`;
    });
  }

  const getCategoryNames = (ids: string[] = []) => {
    return ids.map((id) => {
      const category = categories.find(cat => cat.id === id);
      return category ? category.name : `Category ${id}`;
    });
  }

  const getCommunicationMethod = (id = 0) => {
    return communicationMethodMap[id] || "Not specified";
  }

  return (
    <div className="text-white p-6 rounded-xl max-w-3xl my-10 mx-auto shadow-lg bg-gray-800">
        <div className="flex justify-between items-center mb-6">
          <h1 className="text-2xl font-semibold">Your Profile</h1>
          <div className="flex gap-2">
            {/*<Button*/}
            {/*  type="primary"*/}
            {/*  size="large"*/}
            {/*  icon={<ShareAltOutlined />}*/}
            {/*  onClick={handleShareProfile}*/}
            {/*  className="bg-blue-500 hover:bg-blue-600"*/}
            {/*>*/}
            {/*  Share Profile*/}
            {/*</Button>*/}
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
          <div className="space-y-2 whitespace-pre-wrap break-words">
            <p><strong>Phone:</strong> {userData?.phoneNumber || "No phone number provided."}</p>
            <p><strong>Bio:</strong> {userData?.bio || "No bio provided."}</p>
          </div>
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
          <p className="whitespace-pre-wrap break-words">
            {userData?.skills || "No skills provided."}
          </p>
        </div>

        <div className="mb-8">
          <h3 className="text-gray-400 mb-2">Industry experience</h3>
        <p className="whitespace-pre-wrap break-words">
          {userData?.experiences || "No experience provided."}
        </p>
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
        <h3 className="text-gray-400 mb-2">Teaching Approaches</h3>
        <div className="flex flex-wrap gap-2">
          {userData?.teachingApproachIds && userData.teachingApproachIds.length > 0 ? (
            getTeachingApproachNames(userData.teachingApproachIds).map((item) => (
              <Tag key={item} className="bg-gray-700 text-white border-none px-4 py-1 rounded-full">
                {item}
              </Tag>
            ))
          ) : (
            <p>No teaching approaches provided.</p>
          )}
        </div>
      </div>

      <div className="mb-8">
        <h3 className="text-gray-400 mb-2">Categories</h3>
        <div className="flex flex-wrap gap-2">
          {userData?.categoryIds && userData.categoryIds.length > 0 ? (
            getCategoryNames(userData.categoryIds).map((item) => (
              <Tag key={item} className="bg-gray-700 text-white border-none px-4 py-1 rounded-full">
                {item}
              </Tag>
            ))
          ) : (
            <p>No categories provided.</p>
          )}
        </div>
      </div>

      <div className="mb-8">
        <h3 className="text-gray-400 mb-2">Preferred Communication</h3>
        <p>{getCommunicationMethod(userData?.preferredCommunicationMethod)}</p>
        </div>

        <div className="mb-8">
        <h3 className="text-gray-400 mb-2">Learning Goal</h3>
        <p className="whitespace-pre-wrap break-words">
          {userData?.goal || "No goal provided."}
        </p>
        </div>

        <div className="pt-4 border-t border-gray-700">
          {/*<Button*/}
          {/*  type="link"*/}
          {/*  className="text-orange-500 flex items-center gap-1 pl-0"*/}
          {/*  onClick={() => console.log("View additional settings")}*/}
          {/*>*/}
          {/*  View Additional Profile Settings*/}
          {/*<svg width="16" height="16" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">*/}
          {/*  <path d="M8.59 16.59L13.17 12L8.59 7.41L10 6L16 12L10 18L8.59 16.59Z" fill="currentColor" />*/}
          {/*  </svg>*/}
          {/*</Button>*/}
        </div>
      <div className="pt-4 border-gray-700">
        <Button type="primary" size="large" onClick={() => navigate("/")} className="bg-orange-500 hover:bg-orange-600">
            Back
          </Button>
      </div>
    </div>
  )
}
