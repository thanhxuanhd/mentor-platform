import { Badge } from 'antd';
import { CalendarOutlined, VideoCameraOutlined, MessageOutlined, UserOutlined } from '@ant-design/icons';

interface Session {
  id: string;
  name: string;
  date: string;
  time: string;
  type: 'video' | 'chat' | 'in-person';
}

export default function DashboardPage() {
  const sessions: Session[] = [
    {
      id: '1',
      name: 'Alex Johnson',
      date: 'Today',
      time: '10:30 AM - 11:30 AM',
      type: 'video'
    },
    {
      id: '2',
      name: 'Sophia Chen',
      date: 'May 7',
      time: '1:00 PM - 2:00 PM',
      type: 'chat'
    },
    {
      id: '3',
      name: 'James Wilson',
      date: 'May 9',
      time: '3:30 PM - 4:30 PM',
      type: 'in-person'
    }
  ];

  const getSessionTypeButton = (type: string) => {
    switch (type) {
      case 'video':
        return (
          <button className="flex items-center gap-2 px-3 py-1.5 bg-blue-600">
            <VideoCameraOutlined />
            Video Call
          </button>
        );
      case 'chat':
        return (
          <button className="flex items-center gap-2 px-3 py-1.5 bg-green-600">
            <MessageOutlined />
            Chat Session
          </button>
        );
      case 'in-person':
        return (
          <button className="flex items-center gap-2 px-3 py-1.5 bg-orange-400">
            <UserOutlined />
            In-Person
          </button>
        );
      default:
        return null;
    }
  };

  return (
    <div className="bg-gray-800 rounded-lg overflow-hidden shadow-lg p-6">
      {/* Header */}
      <div className="max-w-7xl mx-auto">
        <h1 className="text-2xl font-semibold text-white mb-2">Learner Dashboard</h1>
        <p className="text-gray-400">Welcome to your learning dashboard. Navigate using the sidebar.</p>
      </div>

      {/* Main Content */}
      <div className="max-w-7xl mx-auto px-6 py-8">
        {/* Upcoming Sessions Section */}
        <div className="bg-slate-800 rounded-lg p-6">
          <div className="flex items-center justify-between mb-6">
            <div className="flex items-center gap-3">
              <h2 className="text-xl font-semibold text-white">Upcoming Sessions</h2>
              <Badge
                count={3}
                style={{ backgroundColor: '#dc2626' }}
              />
            </div>
            <button className="text-blue-400 hover:text-blue-300 text-sm font-medium transition-colors">
              Manage Schedule →
            </button>
          </div>

          {/* Sessions List */}
          <div className="space-y-4">
            {sessions.map((session) => (
              <div
                key={session.id}
                className="bg-slate-700 hover:bg-slate-600 rounded-lg p-5 transition-colors cursor-pointer"
              >
                <div className="flex items-start justify-between">
                  <div className="flex-1">
                    <h3 className="text-lg font-medium text-white mb-3">
                      {session.name}
                    </h3>
                    <div className="flex items-center gap-2 text-gray-300 mb-4">
                      <CalendarOutlined className="text-gray-400" />
                      <span className="text-sm">
                        {session.date} • {session.time}
                      </span>
                    </div>
                    {getSessionTypeButton(session.type)}
                  </div>
                </div>
              </div>
            ))}
          </div>
        </div>
      </div>
    </div>
  );
};