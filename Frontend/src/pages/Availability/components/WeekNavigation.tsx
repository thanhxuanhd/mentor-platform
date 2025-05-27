import { Button } from 'antd';
import { LeftOutlined, RightOutlined } from '@ant-design/icons';
import { Dayjs } from 'dayjs';

interface WeekNavigationProps {
  currentWeekStart: Dayjs;
  onWeekChange: (direction: 'prev' | 'next') => void;
  onTodayClick: () => void;
}

export function WeekNavigation({ currentWeekStart, onWeekChange, onTodayClick }: WeekNavigationProps) {
  const weekEnd = currentWeekStart.add(6, 'day');
  
  return (
    <div className="space-y-3">
      <h3 className="text-lg font-medium">Calendar Navigation</h3>
      
      <div className="flex items-center justify-between mb-2">
        <Button
          icon={<LeftOutlined />}
          onClick={() => onWeekChange('prev')}
          className="bg-transparent border-slate-500 text-white hover:bg-slate-600"
        />
        <span className="text-sm font-medium">
          {currentWeekStart.format("MMM D")} - {weekEnd.format("MMM D, YYYY")}
        </span>
        <Button
          icon={<RightOutlined />}
          onClick={() => onWeekChange('next')}
          className="bg-transparent border-slate-500 text-white hover:bg-slate-600"
        />
      </div>
      
      <Button
        onClick={onTodayClick}
        className="w-full bg-slate-600 border-slate-500 text-white hover:bg-slate-500"
      >
        Go to current week
      </Button>
    </div>
  );
}
