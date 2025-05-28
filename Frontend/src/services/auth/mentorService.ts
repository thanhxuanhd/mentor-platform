import { axiosClient } from "../apiClient";

interface MentorApplicationData {
  education?: string;
  experience?: string;
  certifications?: string;
  motivation?: string;
  documents?: [];
}

export const postMentorSubmission = async (data?: MentorApplicationData) => {
  const response = await axiosClient.post(`mentor-applications/mentor-submission`, data || {});
  return response.data.value;
};