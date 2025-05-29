import { axiosClient } from "../apiClient";

export const postMentorSubmission = async (data: FormData) => {
  const response = await axiosClient.post(
    `mentor-applications/mentor-submission`,
    data,
    {
      headers: { "Content-Type": "multipart/form-data" },
    },
  );
  return response.data.value;
};
