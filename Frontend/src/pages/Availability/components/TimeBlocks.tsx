import { Tooltip } from 'antd';
import { LockOutlined } from '@ant-design/icons';
import type { TimeSlot } from '../types';
import dayjs from 'dayjs';

interface TimeBlocksProps {
  selectedDate: dayjs.Dayjs;
  timeSlots: TimeSlot[];
  onToggleSlot: (slotId: string) => void;
}

export function TimeBlocks({ selectedDate, timeSlots, onToggleSlot }: TimeBlocksProps) {
  return (
    <div className="space-y-4">
      <h3 className="text-lg font-medium mb-4">
        Set your availability for {selectedDate.format("dddd, MMMM D")}
      </h3>
      
      {timeSlots.length > 0 ? (
        <div className="grid grid-cols-2 md:grid-cols-4 lg:grid-cols-5 gap-3">
          {timeSlots.map((slot) => (
            <Tooltip 
              key={slot.id} 
              title={slot.booked ? "This slot is already booked" : null}
            >
              <button
                onClick={() => !slot.booked && onToggleSlot(slot.id)}
                className={`
                  p-4 rounded-lg text-center transition-colors relative
                  ${slot.booked 
                    ? 'bg-slate-500 cursor-not-allowed text-slate-300' 
                    : slot.available
                      ? 'bg-orange-500 hover:bg-orange-600 text-white'
                      : 'bg-slate-600 hover:bg-slate-500 text-slate-300'
                  }
                `}
              >
                <div className="text-sm font-medium">{slot.time}</div>
                <div className="text-xs mt-1">
                  {slot.booked ? 'Booked' : (slot.available ? 'Available' : 'Unavailable')}
                </div>
                
                {slot.booked && (
                  <div className="absolute top-2 right-2">
                    <LockOutlined />
                  </div>
                )}
              </button>
            </Tooltip>
          ))}
        </div>
      ) : (
        <div className="text-center text-slate-400 py-8">
          No time slots available for the current settings
        </div>
      )}
    </div>
  );
}
