import { useEffect, useState } from "react";
import { Avatar, Card, Modal, App } from "antd";
import { getAvailableMentors, type AvailableMentorForBookingResponse } from "../../../services/session-booking/sessionBookingService";
import type { Mentor } from "../../../types/SessionsType";
import type { NotificationProps } from "../../../types/Notification";

interface MentorSelectionModalProps {
  open: boolean;
  onCancel: () => void;
  onMentorSelect: (mentor: Mentor) => void;
}

export function MentorSelectionModal({ open, onCancel, onMentorSelect }: MentorSelectionModalProps) {
  const [mentors, setMentors] = useState<Mentor[]>([]);
  const [loading, setLoading] = useState(false);
  const [notify, setNotify] = useState<NotificationProps | null>(null);
  const { notification } = App.useApp();

  useEffect(() => {
    if (notify) {
      notification[notify.type]({
        message: notify.message,
        description: notify.description,
        placement: "topRight",
      });
      setNotify(null);
    }
  }, [notify, notification]);

  useEffect(() => {
    if (!open) return;

    const fetchMentors = async () => {
      setLoading(true);
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
        setNotify({
          type: "error",
          message: "Loading Error",
          description: err.response.data.error || "Failed to load mentors. Please try again."
        });
      } finally {
        setLoading(false);
      }
    };

    fetchMentors();
  }, [open]);

  return (
    <Modal
      title={<span className="text-white">Select a Mentor</span>}
      open={open}
      onCancel={onCancel}
      footer={null}
      className="mentor-modal"
      styles={{
        content: { backgroundColor: "#334155" },
        header: { backgroundColor: "#334155", borderBottom: "1px solid #475569" },
      }}
    >
      <div className="space-y-4">
        {loading ? (
          <div className="text-center py-8">
            <p className="text-gray-400">Loading mentor...</p>
          </div>
        ) : mentors.length > 0 ? (
          mentors.map((mentor) => (
            <Card
              key={mentor.id}
              className="cursor-pointer transition-all border-slate-600 bg-slate-700 hover:border-orange-500"
              onClick={() => onMentorSelect(mentor)}
            >
              <div className="flex items-center space-x-4">
                <div className="relative">
                  <Avatar size={50} src={mentor.avatar} />
                </div>
                <div className="flex-1">
                  <h4 className="text-white font-medium">{mentor.name}</h4>
                  <p className="text-gray-400 text-sm">{mentor.expertise.join(", ")}</p>
                  <p className="text-green-400 text-xs">{mentor.availability}</p>
                </div>
              </div>
            </Card>
          ))
        ) : (
          <p className="text-gray-400">No mentors available.</p>
        )}
      </div>
    </Modal>
  );
}