import { Tooltip } from 'antd';
import { LockOutlined } from '@ant-design/icons';
import type { TimeBlock } from '../types';
import dayjs from 'dayjs';

interface TimeBlocksProps {
  selectedDate: dayjs.Dayjs;
  timeBlocks: TimeBlock[];
  onToggleBlock: (blockId: string) => void;
  isLocked?: boolean;
}

export function TimeBlocks({ selectedDate, timeBlocks, onToggleBlock, isLocked = false }: TimeBlocksProps) {
  return (
    <div className="space-y-4">
      <h3 className="text-lg font-medium mb-4">
        Set your availability for {selectedDate.format("dddd, MMMM D")}
      </h3>
      
      {timeBlocks.length > 0 ? (
        <div className="grid grid-cols-2 md:grid-cols-4 lg:grid-cols-5 gap-3">
          {timeBlocks.map((block) => (            <Tooltip 
              key={block.id} 
              title={
                block.booked 
                  ? "This slot is already booked" 
                  : block.isPast
                    ? "This time slot is in the past and cannot be modified"
                    : null
              }
            >
              <button
                onClick={() => !block.booked && !block.isPast && onToggleBlock(block.id)}
                className={`
                  p-4 rounded-lg text-center transition-colors relative
                  ${block.booked 
                    ? 'bg-slate-500 cursor-not-allowed text-slate-300' 
                    : block.isPast
                      ? 'bg-slate-700 cursor-not-allowed text-slate-400'
                      : block.available
                        ? 'bg-orange-500 hover:bg-orange-600 text-white'
                        : 'bg-slate-600 hover:bg-slate-500 text-slate-300'
                  }
                `}
              >
                <div className="text-sm font-medium">{block.time}</div>
                <div className="text-xs mt-1">
                  {block.booked 
                    ? 'Booked' 
                    : block.isPast 
                      ? 'Past' 
                      : (block.available ? 'Available' : 'Unavailable')
                  }
                </div>
                  {(block.booked || block.isPast) && (
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
