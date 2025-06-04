import { Button, Avatar } from "antd";
import { PlusOutlined, MessageOutlined, CalendarOutlined } from "@ant-design/icons";
import type { Mentor } from "./MentorSelectionModal";

interface MentorProfileProps {
  selectedMentor: Mentor;
  onSelectMentor: () => void;
  onMessage: () => void;
  onViewSessions: () => void;
}

export default function MentorProfile({
  selectedMentor,
  onSelectMentor,
  onMessage,
  onViewSessions,
}: MentorProfileProps) {
  return (
    <div className="flex items-center justify-between mb-8 bg-slate-700 rounded-lg p-6">
      <div className="flex items-center space-x-4">
        <div className="relative">
          <Avatar size={60} src={selectedMentor.avatar} />
          {selectedMentor.isOnline && (
            <div className="absolute -bottom-1 -right-1 w-4 h-4 bg-green-500 rounded-full border-2 border-slate-700"></div>
          )}
        </div>
        <div>
          <h3 className="text-xl font-semibold">{selectedMentor.name}</h3>
          <p className="text-gray-400">{selectedMentor.expertise}</p>
          <p className="text-green-400 text-sm">{selectedMentor.availability}</p>
        </div>
      </div>
      <div className="flex space-x-2">
        <Button
          type="primary"
          shape="circle"
          icon={<PlusOutlined />}
          className="bg-orange-500 border-orange-500 hover:bg-orange-600"
          onClick={onSelectMentor}
        />
        <Button
          type="primary"
          shape="circle"
          icon={<MessageOutlined />}
          className="bg-orange-500 border-orange-500 hover:bg-orange-600"
          onClick={onMessage}
        />
        <Button
          type="primary"
          shape="circle"
          icon={<CalendarOutlined />}
          className="bg-orange- Soudalize this line: orange-500 border-orange-500 hover:bg-orange-600"
          onClick={onViewSessions}
        />
      </div>
    </div>
  );
}