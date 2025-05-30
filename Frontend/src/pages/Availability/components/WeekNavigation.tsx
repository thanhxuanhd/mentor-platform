import { Button } from 'antd';
import { LeftOutlined, RightOutlined } from '@ant-design/icons';
import dayjs, { Dayjs } from 'dayjs';

interface WeekNavigationProps {
  currentWeekStart: Dayjs;
  onWeekChange: (direction: 'prev' | 'next') => void;
  onTodayClick: () => void;
}

export function WeekNavigation({ currentWeekStart, onWeekChange, onTodayClick }: WeekNavigationProps) {
  const weekEnd = currentWeekStart.add(6, 'day');
  const today = dayjs();
  // Calculate the start of the current week
  const thisWeekStart = today.startOf('week');
  // Calculate the maximum allowed week start date (4 weeks from today)
  const maxWeekStart = today.add(4, 'weeks').startOf('week');
  // Check if the current week start is at or before the current week
  const isPrevDisabled = currentWeekStart.isSame(thisWeekStart, 'week') || currentWeekStart.isBefore(thisWeekStart);
  // Check if the current week start is at or beyond the max week
  const isNextDisabled = currentWeekStart.isSame(maxWeekStart, 'week') || currentWeekStart.isAfter(maxWeekStart);

  return (
    <div className="space-y-3">
      <h3 className="text-lg font-medium">Calendar Navigation</h3>

      <div className="flex items-center justify-between mb-2">
        <Button
          icon={<LeftOutlined />}
          onClick={() => !isPrevDisabled && onWeekChange('prev')}
          disabled={isPrevDisabled}
          className={`bg-transparent border-slate-500 text-white 
            ${isPrevDisabled ? 'opacity-50 cursor-not-allowed' : 'hover:bg-slate-600'}`}
        />
        <span className="text-sm font-medium">
          {currentWeekStart.format("MMM D")} - {weekEnd.format("MMM D, YYYY")}
        </span>
        <Button
          icon={<RightOutlined />}
          onClick={() => !isNextDisabled && onWeekChange('next')}
          disabled={isNextDisabled}
          className={`bg-transparent border-slate-500 text-white 
            ${isNextDisabled ? 'opacity-50 cursor-not-allowed' : 'hover:bg-slate-600'}`}
        />
      </div>

      <Button
        onClick={onTodayClick}
        className="w-full bg-slate-600 border-slate-500 text-white hover:bg-slate-500"
      >
        Today
      </Button>
    </div>
  );
}