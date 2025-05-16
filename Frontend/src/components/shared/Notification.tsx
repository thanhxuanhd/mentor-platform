import { notification } from "antd";
import type { NotificationPlacement } from "antd/es/notification/interface";

type NotificationType = "success" | "error" | "info" | "warning";

const openNotification = (
  type: NotificationType,
  message: string,
  description?: string,
  placement: NotificationPlacement = "topRight",
) => {
  notification[type]({
    message,
    description,
    placement,
  });
};

export const notifySuccess = (message: string, description?: string) => {
  openNotification("success", message, description);
};

export const notifyError = (message: string, description?: string) => {
  openNotification("error", message, description);
};

export const notifyInfo = (message: string, description?: string) => {
  openNotification("info", message, description);
};

export const notifyWarning = (message: string, description?: string) => {
  openNotification("warning", message, description);
};
