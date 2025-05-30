import { Tooltip } from 'antd';
import dayjs, { Dayjs } from 'dayjs';
import type { WeekDay } from '../types';

interface WeekCalendarProps {
  weekDays: WeekDay[];
  onDaySelect: (date: Dayjs) => void;
}

export function WeekCalendar({ weekDays, onDaySelect }: WeekCalendarProps) {
  return (
    <div className="grid grid-cols-7 gap-2">
      {weekDays.map((day, index) => (
        <button
          key={index}
          onClick={() => onDaySelect(dayjs(day.fullDate))}
          className={`
            p-4 rounded-lg text-center transition-colors relative
            ${day.isToday ? 'ring-2 ring-white/30' : ''}
            ${day.active 
              ? 'bg-orange-500 text-white' 
              : 'bg-slate-700 text-slate-300 hover:bg-slate-600'
            }
          `}
        >
          <div className="md:block hidden font-medium">{day.day}</div>
          <div className="md:hidden block font-medium">{day.shortDay}</div>
          <div className="text-sm">{day.date}</div>
          
          {day.hasAvailability && (
            <Tooltip title="Has available time slots">
              <div className="absolute top-1 right-1 w-2 h-2 bg-green-400 rounded-full"></div>
            </Tooltip>
          )}
        </button>
      ))}
    </div>
  );
}
