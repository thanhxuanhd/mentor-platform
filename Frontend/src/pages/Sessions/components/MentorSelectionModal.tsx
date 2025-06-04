import { Avatar, Card, Modal } from "antd"

export interface Mentor {
  id: string;
  name: string;
  expertise: string[];
  availability: string;
  avatar: string;
}

interface MentorSelectionModalProps {
  open: boolean;
  onCancel: () => void;
  mentors: Mentor[];
  onMentorSelect: (mentor: Mentor) => void;
}

export function MentorSelectionModal({ open, onCancel, mentors, onMentorSelect }: MentorSelectionModalProps) {
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
        {mentors.length > 0 ? (
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
