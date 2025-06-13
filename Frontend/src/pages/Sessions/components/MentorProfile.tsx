import { Button, Avatar } from "antd";
import { PlusOutlined, CalendarOutlined } from "@ant-design/icons";
import type { Mentor } from "../../../types/SessionsType";

interface MentorProfileProps {
  selectedMentor: Mentor | null;
  onSelectMentor: () => void;
  onViewSessions: () => void;
}

export default function MentorProfile({
  selectedMentor,
  onSelectMentor,
  onViewSessions,
}: MentorProfileProps) {
  return (
    <div className="flex items-center justify-between mb-8 bg-slate-700 rounded-lg p-6">
      {selectedMentor ? (
        <>
          <div className="flex items-center space-x-4">
            <div className="relative">
              <Avatar size={60} src={selectedMentor.avatar} />
            </div>
            <div>
              <h3 className="text-xl font-semibold">{selectedMentor.name}</h3>
              <p className="text-gray-400">
                {selectedMentor.expertise.join(", ")}
              </p>
              <p className="text-green-400 text-sm">
                {selectedMentor.availability}
              </p>
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
              icon={<CalendarOutlined />}
              className="bg-orange-500 border-orange-500 hover:bg-orange-600"
              onClick={onViewSessions}
            />
          </div>
        </>
      ) : (
        <div className="flex items-center justify-between w-full">
          <div>
            <h3 className="text-xl font-semibold text-gray-400">
              No mentor selected
            </h3>
            <p className="text-gray-400">
              Please select a mentor to book a session.
            </p>
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
              icon={<CalendarOutlined />}
              className="bg-orange-500 border-orange-500 hover:bg-orange-600"
              onClick={onViewSessions}
            />
          </div>
        </div>
      )}
    </div>
  );
}
