import axios from 'axios';

export interface UserProfile {
  id: string;
  fullName: string;
  roleId: number;
  bio: string | null;
  profilePhotoUrl: string | null;
  phoneNumber: string;
  skills: string | null;
  experiences: string | null;
  preferredCommunicationMethod: number;
  goal: string | null;
  preferredSessionFrequency: number;
  preferredSessionDuration: number;
  preferredLearningStyle: number;
  isPrivate: boolean;
  isAllowedMessage: boolean;
  isReceiveNotification: boolean;
  availabilityIds: string[];     
  expertiseIds: string[];        
  teachingApproachIds: string[]; 
  categoryIds: string[]; 
}

export interface UpdateProfileRequest {
  fullName?: string;
  roleId?: number;
  bio?: string;
  profilePhotoUrl?: string;
  phoneNumber?: string;
  skills?: string;
  experiences?: string;
  preferredCommunicationMethod?: number;
  goal?: string;
  preferredSessionFrequency?: number;
  preferredSessionDuration?: number;
  preferredLearningStyle?: number;
  isPrivate?: boolean;
  isAllowedMessage?: boolean;
  isReceiveNotification?: boolean;
  availabilityIds: string[];     
  expertiseIds: string[];        
  teachingApproachIds: string[]; 
  categoryIds: string[]; 
}

const API_BASE_URL = 'https://localhost:5000/api/Users';

const UserProfileClient = {
  getProfile: async (userId: string, token: string): Promise<UserProfile> => {
    try {
      const response = await axios.get(`${API_BASE_URL}/${userId}/detail`, {
        headers: {
          Authorization: `Bearer ${token}`,
        },
      });
      return response.data.value;
    } catch (error) {
      console.error('Error fetching user profile:', error);
      throw error;
    }
  },

  updateProfile: async (userId: string, profileData: UpdateProfileRequest): Promise<UserProfile> => {
    try {
      const response = await axios.put(`${API_BASE_URL}/${userId}/detail`, profileData, {
      
      });
      return response.data.value;
    } catch (error) {
      console.error('Error updating user profile:', error);
      throw error;
    }
  },

  uploadProfilePhoto: async (userId: string, file: File, token: string): Promise<string> => {
    try {
      const formData = new FormData();
      formData.append('file', file);

      const response = await axios.post(
        `${API_BASE_URL}/${userId}/photo`,
        formData,
        {
          headers: {
            'Content-Type': 'multipart/form-data',
            Authorization: `Bearer ${token}`,
          },
        }
      );
      return response.data.url;
    } catch (error) {
      console.error('Error uploading profile photo:', error);
      throw error;
    }
  },
};

export default UserProfileClient;