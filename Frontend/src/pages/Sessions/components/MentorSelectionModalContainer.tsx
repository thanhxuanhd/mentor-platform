import { useEffect, useState } from "react";
import { MentorSelectionModal, type Mentor } from "./MentorSelectionModal";
import { getAvailableMentors, type AvailableMentorForBookingResponse } from "../../../services/session-booking/sessionBookingService";
import { Alert, Spin } from "antd";

interface MentorSelectionModalContainerProps {
  open: boolean;
  onCancel: () => void;
  onMentorSelect: (mentor: Mentor) => void;
}

export function MentorSelectionModalContainer({ open, onCancel, onMentorSelect }: MentorSelectionModalContainerProps) {
  const [mentors, setMentors] = useState<Mentor[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!open) return;

    const fetchMentors = async () => {
      setLoading(true);
      setError(null);
      try {
        const response = await getAvailableMentors();
        console.log("Available Mentors Response:", response);
        const transformedMentors: Mentor[] = response.map((item: AvailableMentorForBookingResponse) => ({
          id: item.mentorId,
          name: item.mentorName,
          expertise: item.mentorExpertise,
          availability: `${item.workingStartTime.slice(0, 5)} - ${item.workingEndTime.slice(0, 5)}`,
          avatar: item.mentorAvatarUrl,
        }));
        setMentors(transformedMentors);
      } catch (err: any) {
        setError("Failed to load mentors. Please try again.");
        console.error("Fetch error:", err.response ? err.response.data : err.message);
      } finally {
        setLoading(false);
      }
    };

    fetchMentors();
  }, [open]);

  if (loading) {
    return <Spin tip="Loading mentors..." />;
  }

  if (error) {
    return <Alert message={error} type="error" showIcon />;
  }

  return (
    <MentorSelectionModal
      open={open}
      onCancel={onCancel}
      mentors={mentors}
      onMentorSelect={onMentorSelect}
    />
  );
}