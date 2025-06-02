export function getCurrentWeekDates() {
  const today = new Date();
  const start = new Date(today);

  start.setDate(today.getDate() - 7);
  start.setHours(0, 0, 0, 0);

  today.setHours(23, 59, 59, 999);

  return {
    startDate: start,
    endDate: today,
  };
}
