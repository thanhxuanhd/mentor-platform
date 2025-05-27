import { Form, Select, InputNumber, Tooltip, Alert } from 'antd';
import { InfoCircleOutlined, LockOutlined } from '@ant-design/icons';

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
  // Generate time options in 30-minute increments
  const generateTimeOptions = () => {
    const options = [];
    for (let hour = 0; hour < 24; hour++) {
      for (let minute of [0, 30]) {
        const timeString = `${hour.toString().padStart(2, '0')}:${minute.toString().padStart(2, '0')}`;
        const displayTime = hour === 0 
          ? `12:${minute.toString().padStart(2, '0')} AM`
          : hour < 12 
            ? `${hour}:${minute.toString().padStart(2, '0')} AM`
            : hour === 12
              ? `12:${minute.toString().padStart(2, '0')} PM`
              : `${hour - 12}:${minute.toString().padStart(2, '0')} PM`;
              
        options.push(
          <Option key={timeString} value={timeString}>
            {displayTime}
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

  return (
    <div className="space-y-6">
      {hasBookedSessions && (
        <Alert
          icon={<LockOutlined />}
          type="warning"
          message="Settings Locked"
          description="You have booked sessions. Contact admin to change work hours or session settings."
          showIcon
          className="mb-4"
        />
      )}

      <div>
        <h3 className="text-lg font-medium mb-4">Work Hours</h3>
        <Form layout="vertical">
          <div className="grid grid-cols-2 gap-4 mb-3">
            <Form.Item 
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
          <div className="grid grid-cols-2 gap-4">
            <Form.Item label={
              <span className="text-slate-300">
                Session duration (minutes)
                <Tooltip title="Duration of each mentoring session">
                  <InfoCircleOutlined className="ml-1 text-slate-400" />
                </Tooltip>
              </span>
            }>
              <InputNumber
                min={15}
                max={180}
                step={15}
                value={sessionDuration}
                onChange={(value) => onSessionDurationChange(value as number)}
                className="w-full"
                disabled={hasBookedSessions}
              />
            </Form.Item>
            
            <Form.Item label={
              <span className="text-slate-300">
                Buffer time (minutes)
                <Tooltip title="Break time between consecutive sessions">
                  <InfoCircleOutlined className="ml-1 text-slate-400" />
                </Tooltip>
              </span>
            }>
              <InputNumber
                min={0}
                max={60}
                step={5}
                value={bufferTime}
                onChange={(value) => onBufferTimeChange(value as number)}
                className="w-full"
                disabled={hasBookedSessions}
              />
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
