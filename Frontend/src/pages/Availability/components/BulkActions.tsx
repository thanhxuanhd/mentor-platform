import { Button } from 'antd';
import { CheckOutlined, CloseOutlined, CopyOutlined } from '@ant-design/icons';
import dayjs from 'dayjs';

interface BulkActionsProps {
  selectedDate: dayjs.Dayjs;
  onSelectAll: () => void;
  onClearAll: () => void;
  onCopyToWeek: () => void;
}

export function BulkActions({ selectedDate, onSelectAll, onClearAll, onCopyToWeek }: BulkActionsProps) {
  return (
    <div className="space-y-3">
      <h3 className="text-lg font-medium mb-4">Bulk Actions</h3>      
      <Button
        icon={<CheckOutlined />}
        onClick={onSelectAll}
        className="w-full bg-blue-600 border-blue-600 text-white hover:bg-blue-700"
      >
        Select all slots for {selectedDate.format("ddd MMM D")}
      </Button>

      <Button
        icon={<CloseOutlined />}
        onClick={onClearAll}
        className="w-full bg-slate-600 border-slate-500 text-white hover:bg-slate-500"
      >
        Clear all slots for {selectedDate.format("ddd MMM D")}
      </Button>      <Button
        icon={<CopyOutlined />}
        onClick={onCopyToWeek}
        className="w-full bg-green-600 border-green-600 text-white hover:bg-green-700"
      >
        Copy schedule to all days in week
      </Button>
    </div>
  );
}
