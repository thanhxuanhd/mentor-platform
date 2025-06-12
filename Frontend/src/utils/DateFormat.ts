import dayjs from "dayjs";
import { useAuth } from "../hooks";

export const formatDate = (dateString: string | undefined): string => {
  if (!dateString) return "N/A";
  try {
    const date = new Date(dateString);
    return new Intl.DateTimeFormat("vi-VN", {
      year: "numeric",
      month: "2-digit",
      day: "2-digit",
    }).format(date);
  } catch {
    return dateString;
  }
};

export const formatDateTime = (dateString: string | undefined): string => {
  const { user } = useAuth();

  if (!dateString) return "N/A";
  try {
    const utcDateTime = dayjs.utc(`${dateString}`);
    const localDateTime = utcDateTime.tz(user?.timezone);
    return localDateTime.format("HH:mm:ss DD/MM/YYYY");
  } catch {
    return dateString;
  }
};
