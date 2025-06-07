export function getCurrentWeekDates() {
  const today = new Date();
  const start = new Date(today);

  start.setDate(today.getDate() - 7);

  return {
    startDate: start,
    endDate: today,
  };
}

export function getSystemStartDate(date?: Date) {
  date?.setHours(0, 0, 0, 0);
  return date?.toLocaleString();
}

export function getSystemEndDate(date?: Date) {
  date?.setHours(23, 59, 59, 999);
  return date?.toLocaleString();
}
