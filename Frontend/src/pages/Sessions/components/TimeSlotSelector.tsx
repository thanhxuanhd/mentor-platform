import { Button, Spin } from "antd";
import type { TimeSlot } from "../../../types/SessionsType";

interface TimeSlotSelectorProps {
  timeSlots: TimeSlot[];
  selectedTime: string;
  onTimeSelect: (time: string, id: string) => void;
  loading: boolean;
}

export default function TimeSlotSelector({
  timeSlots,
  selectedTime,
  onTimeSelect,
  loading,
}: TimeSlotSelectorProps) {
  return (
    <div className="mt-8">
      <h3 className="text-lg font-medium mb-4 text-center">Select a time slot</h3>
      {loading ? (
        <Spin tip="Loading time slots..." />
      ) : (
        <div className="grid grid-cols-5 gap-3">
          {timeSlots.length > 0 ? (
            timeSlots.map((slot) => {
              const timeDisplay = `${slot.startTime.slice(0, 5)} - ${slot.endTime.slice(0, 5)}`;
              const isDisabled = slot.status === "Pending";
              console.log(`Slot ${slot.id}: Status=${slot.status}, Disabled=${isDisabled}`);
              return (
                <Button
                  key={slot.id}
                  type={selectedTime === slot.startTime ? "primary" : "default"}
                  className={`h-12 ${selectedTime === slot.startTime
                    ? "bg-orange-500 border-orange-500"
                    : "bg-orange-500 border-orange-500 text-white hover:bg-orange-600"
                    } ${isDisabled ? "opacity-50 cursor-not-allowed" : ""}`}
                  onClick={() => onTimeSelect(slot.startTime, slot.id)}
                  disabled={isDisabled}
                >
                  {timeDisplay}
                </Button>
              );
            })
          ) : (
            <div className="flex justify-center items-center col-span-5">
              <p className="text-gray-400 text-center">No time slots available</p>
            </div>
          )}
        </div>
      )}
    </div>
  );
}