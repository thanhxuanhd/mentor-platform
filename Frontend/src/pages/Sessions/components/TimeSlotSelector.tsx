import { Button } from "antd";

interface TimeSlotSelectorProps {
  timeSlots: string[];
  selectedTime: string;
  onTimeSelect: (time: string) => void;
}

export default function TimeSlotSelector({
  timeSlots,
  selectedTime,
  onTimeSelect,
}: TimeSlotSelectorProps) {
  return (
    <div className="mt-8">
      <h3 className="text-lg font-medium mb-4 text-center">Select a time slot</h3>
      <div className="grid grid-cols-5 gap-3">
        {timeSlots.map((time) => (
          <Button
            key={time}
            type={selectedTime === time ? "primary" : "default"}
            className={`h-12 ${selectedTime === time
              ? "bg-orange-500 border-orange-500"
              : "bg-orange-500 border-orange-500 text-white hover:bg-orange-600"
              }`}
            onClick={() => onTimeSelect(time)}
          >
            {time}
          </Button>
        ))}
      </div>
    </div>
  );
}