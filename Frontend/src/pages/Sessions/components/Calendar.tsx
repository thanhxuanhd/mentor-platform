import { Button } from "antd"
import type { Dayjs } from "dayjs"
import dayjs from "dayjs"
import {
  LeftOutlined,
  RightOutlined,
} from "@ant-design/icons"

interface CalendarComponentProps {
  selectedDate: Dayjs | null
  currentMonth: Dayjs
  onDateSelect: (date: Dayjs) => void
  onMonthChange: (month: Dayjs) => void
}

export function CalendarComponent({
  selectedDate,
  currentMonth,
  onDateSelect,
  onMonthChange,
}: CalendarComponentProps) {
  const today = dayjs();

  // Get the start of the month
  const startOfMonth = currentMonth.startOf("month");
  const endOfMonth = currentMonth.endOf("month");

  // Adjust startOfWeek to Monday
  const startOfWeek = startOfMonth.day(); // 0 = Sunday, 1 = Monday, ..., 6 = Saturday
  const startDate = startOfMonth.subtract((startOfWeek === 0 ? 6 : startOfWeek - 1), "day");
  const endDate = endOfMonth.endOf("week"); // This will still work as we adjust the start

  const days = [];
  let current = startDate;

  while (current.isBefore(endDate) || current.isSame(endDate, "day")) {
    days.push(current);
    current = current.add(1, "day");
  }

  const weekDays = ["MON", "TUE", "WED", "THU", "FRI", "SAT", "SUN"];

  const handlePreviousMonth = () => {
    onMonthChange(currentMonth.subtract(1, "month"));
  };

  const handleNextMonth = () => {
    onMonthChange(currentMonth.add(1, "month"));
  };

  return (
    <div className="bg-slate-700 rounded-lg p-6">
      <div className="flex items-center justify-between mb-6">
        <Button
          type="text"
          icon={<LeftOutlined />}
          className="text-white hover:text-orange-400"
          onClick={handlePreviousMonth}
          disabled={currentMonth.isSame(today, "month")}
        />
        <h3 className="text-white text-lg font-medium">{currentMonth.format("MMMM YYYY")}</h3>
        <Button
          type="text"
          icon={<RightOutlined />}
          className="text-white hover:text-orange-400"
          onClick={handleNextMonth}
        />
      </div>

      <div className="grid grid-cols-7 gap-1 mb-4">
        {weekDays.map((day) => (
          <div key={day} className="text-center text-gray-400 text-sm py-2">
            {day}
          </div>
        ))}
      </div>

      <div className="grid grid-cols-7 gap-1">
        {days.map((day, index) => {
          const isCurrentMonth = day.month() === currentMonth.month();
          const isSelected = selectedDate && day.isSame(selectedDate, "day");
          const isPastDate = day.isBefore(today, "day");
          const isClickable = isCurrentMonth && !isPastDate;

          return (
            <button
              key={index}
              onClick={() => isClickable && onDateSelect(day)}
              className={`
                flex items-center justify-center h-10 w-full rounded-lg text-sm font-medium transition-colors
                ${!isCurrentMonth || isPastDate
                  ? "text-gray-600 cursor-not-allowed"
                  : "text-white hover:bg-slate-600 cursor-pointer"
                }
                ${isSelected ? "bg-orange-500 text-white" : ""}
              `}
              disabled={!isClickable}
            >
              {day.date()}
            </button>
          );
        })}
      </div>
    </div>
  );
}