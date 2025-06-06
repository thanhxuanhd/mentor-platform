import { useState, useEffect } from 'react';
import { Avatar, Badge } from 'antd';
import { CalendarOutlined, VideoCameraOutlined, UserOutlined, HomeOutlined } from '@ant-design/icons';
import { learnerDashboardService, type GetLearnerDashboardResponse, type LearnerUpcomingSessionResponse } from '../../../services/learnerDashboard/learnerDashboardService';
import dayjs from 'dayjs';

interface Session {
  id: string;
  name: string;
  date: string;
  time: string;
  avatarUrl?: string;
  type: 'Virtual' | 'OneOnOne' | 'Onsite';
}

export default function LearnerDashboard() {
  const [sessions, setSessions] = useState<Session[]>([]);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchSessions = async () => {
      try {
        setLoading(true);
        const response: GetLearnerDashboardResponse = await learnerDashboardService.getLearnerDashboard();

        const mappedSessions: Session[] = response.upcomingSessions.map((session: LearnerUpcomingSessionResponse) => ({
          id: session.sessionId,
          name: session.mentorName,
          date: dayjs(session.scheduledDate).format('MMM D, YYYY'),
          time: session.timeRange,
          type: session.type as 'Virtual' | 'OneOnOne' | 'Onsite',
          avatarUrl: session.mentorProfilePictureUrl
        }));

        setSessions(mappedSessions);
      } catch (err) {
        setError('Failed to fetch sessions. Please try again later.');
        console.error('Error fetching learner dashboard:', err);
      } finally {
        setLoading(false);
      }
    };

    fetchSessions();
  }, []);

  const getSessionTypeButton = (type: string) => {
    switch (type) {
      case 'Virtual':
        return (
          <button className="flex items-center gap-2 px-3 py-1.5 bg-blue-600">
            <VideoCameraOutlined />
            Virtual
          </button>
        );
      case 'OneOnOne':
        return (
          <button className="flex items-center gap-2 px-3 py-1.5 bg-green-600">
            <UserOutlined />
            One On One
          </button>
        );
      case 'Onsite':
        return (
          <button className="flex items-center gap-2 px-3 py-1.5 bg-orange-400">
            <HomeOutlined />
            Onsite
          </button>
        );
      default:
        return null;
    }
  };

  return (
    <div className="bg-gray-800 rounded-lg overflow-hidden shadow-lg p-6">
      <div className="max-w-7xl mx-auto">
        <h1 className="text-2xl font-semibold text-white mb-2">Learner Dashboard</h1>
        <p className="text-gray-400">Welcome to your learning dashboard. Navigate using the sidebar.</p>
      </div>

      <div className="max-w-7xl mx-auto px-6 py-8">
        <div className="bg-slate-800 rounded-lg p-6">
          <div className="flex items-center justify-between mb-6">
            <div className="flex items-center gap-3">
              <h2 className="text-xl font-semibold text-white">Upcoming Sessions</h2>
              <Badge
                count={sessions.length}
                style={{ backgroundColor: '#dc2626' }}
              />
            </div>
            <button className="text-blue-400 hover:text-blue-300 text-sm font-medium transition-colors">
              View Schedule →
            </button>
          </div>

          <div className="space-y-4">
            {loading ? (
              <p className="text-gray-300">Loading sessions...</p>
            ) : error ? (
              <p className="text-red-400">{error}</p>
            ) : sessions.length === 0 ? (
              <p className="text-gray-300">No upcoming sessions.</p>
            ) : (
              sessions.map((session) => (
                <div
                  key={session.id}
                  className="bg-slate-700 hover:bg-slate-600 rounded-lg p-5 transition-colors cursor-pointer"
                >
                  <div className="flex items-start justify-between">
                    <div className="flex items-center gap-4 flex-1">
                      <Avatar
                        src={session.avatarUrl || undefined}
                        icon={!session.avatarUrl ? <UserOutlined /> : undefined}
                        size={100}
                        className="flex-shrink-0"
                      />
                      <div>
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
                </div>
              ))
            )}
          </div>
        </div>
      </div>
    </div>
  );
};