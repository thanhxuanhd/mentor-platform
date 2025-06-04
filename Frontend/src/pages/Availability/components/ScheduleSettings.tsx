import { Form, Select, Alert } from 'antd';
import { LockOutlined } from '@ant-design/icons';

const { Option } = Select;

interface ScheduleSettingsProps {
  startTime: string;
  endTime: string;
  sessionDuration: number;
  bufferTime: number;
  onStartTimeChange: (time: string) => void;
  onEndTimeChange: (time: string) => void;
  onSessionDurationChange: (duration: number) => void;
  onBufferTimeChange: (buffer: number) => void;
  hasBookedSessions: boolean;
}

export function ScheduleSettings({
  startTime,
  endTime,
  sessionDuration,
  bufferTime,
  onStartTimeChange,
  onEndTimeChange,
  onSessionDurationChange,
  onBufferTimeChange,
  hasBookedSessions
}: ScheduleSettingsProps) {
  const generateTimeOptions = () => {
    const options = [];
    for (let hour = 0; hour < 24; hour++) {
      for (let minute of [0, 30]) {
        const timeString = `${hour.toString().padStart(2, '0')}:${minute.toString().padStart(2, '0')}`;
        options.push(
          <Option key={timeString} value={timeString}>
            {timeString}
          </Option>
        );
      }
    }
    return options;
  };

  const isEndTimeValid = () => {
    const [startHour, startMinute] = startTime.split(':').map(Number);
    const [endHour, endMinute] = endTime.split(':').map(Number);

    if (startHour > endHour) return false;
    if (startHour === endHour && startMinute >= endMinute) return false;
    return true;
  };

  const sessionDurationOptions = [30, 45, 60, 75, 90];
  const bufferTimeOptions = [0, 15, 30, 45, 60];

  return (
    <div className="space-y-6">
      {hasBookedSessions && (
        <Alert
          icon={<LockOutlined />}
          type="warning"
          message="Settings Locked"
          description="You have upcoming booked sessions. Contact admin to change work hours or session settings."
          showIcon
          className="mb-4"
        />
      )}

      <div>
        <h3 className="text-lg font-medium mb-4">Work Hours</h3>
        <Form layout="vertical">          
          <div className="grid grid-cols-2 gap-4 mb-3">
            <Form.Item
              id="start-time-form-item"
              label={<span className="text-slate-300">Start time</span>}
              validateStatus={isEndTimeValid() ? '' : 'error'}
              help={!isEndTimeValid() && "Start time must be before end time"}
            >
              <Select
                value={startTime}
                onChange={onStartTimeChange}
                className="w-full"
                disabled={hasBookedSessions}
              >
                {generateTimeOptions()}
              </Select>
            </Form.Item>            
            <Form.Item
              id="end-time-form-item"
              label={<span className="text-slate-300">End time</span>}
              validateStatus={isEndTimeValid() ? '' : 'error'}
            >
              <Select
                value={endTime}
                onChange={onEndTimeChange}
                className="w-full"
                disabled={hasBookedSessions}
              >
                {generateTimeOptions()}
              </Select>
            </Form.Item>
          </div>
        </Form>
      </div>

      <div>
        <h3 className="text-lg font-medium mb-4">Session Settings</h3>
        <Form layout="vertical">          
          <div className="grid grid-cols-2 gap-4 items-end">
            <Form.Item id="session-duration-form-item" label={<span className="text-slate-300">Session duration</span>} className="mb-0">
              <Select
                value={sessionDuration}
                onChange={onSessionDurationChange}
                className="w-full h-10"
                disabled={hasBookedSessions}
                size="middle"
              >
                {sessionDurationOptions.map((duration) => (
                  <Option key={duration} value={duration}>
                    {duration} minutes
                  </Option>
                ))}
              </Select>
            </Form.Item>

            <Form.Item id="buffer-time-form-item" label={<span className="text-slate-300">Buffer time</span>} className="mb-0">
              <Select
                value={bufferTime}
                onChange={onBufferTimeChange}
                className="w-full h-10"
                disabled={hasBookedSessions}
                size="middle"
              >
                {bufferTimeOptions.map((buffer) => (
                  <Option key={buffer} value={buffer}>
                    {buffer} minutes
                  </Option>
                ))}
              </Select>
            </Form.Item>
          </div>
        </Form>

        {hasBookedSessions && (
          <div className="mt-4 p-3 bg-amber-900/30 border border-amber-700/50 rounded text-amber-200 text-sm">
            <p>
              <strong>Note:</strong> Settings are locked because you have booked sessions.
              Changing these settings could affect existing appointments.
            </p>
            <p className="mt-1">
              Please contact an administrator if you need to make changes.
            </p>
          </div>
        )}
      </div>
    </div>
  );
}