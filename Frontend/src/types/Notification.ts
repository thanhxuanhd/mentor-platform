export interface NotificationProps {
  message: string;
  description: string;
  type: "info" | "success" | "error" | "warning";
}
