import { App, Button, Card } from "antd";
import { FileOutlined } from "@ant-design/icons";
import { HiOutlineDocumentDownload } from "react-icons/hi";
import { useCallback, useEffect, useState } from "react";
import { dashboardService } from "../../../../services/adminDashboard/adminDashboardService";
import type { NotificationProps } from "../../../../types/Notification";

export default function ReportActions() {
  const [notify, setNotify] = useState<NotificationProps | null>(null);
  const { notification } = App.useApp();

  useEffect(() => {
    if (notify) {
      notification[notify.type]({
        message: notify.message,
        description: notify.description,
        placement: "topRight",
        showProgress: true,
        duration: 3,
        pauseOnHover: true,
      });
      setNotify(null);
    }
  }, [notify, notification]);

  const generateMonthlyMentorApplicationReport = useCallback(async () => {
    try {
      await dashboardService.generateMonthlyMentorApplicationReport();
      setNotify({
        type: "success",
        message: "Download Successful",
        description: "Monthly Mentor Application Report downloaded successfully.",
      });
    } catch (error: any) {
      console.log(error)
      setNotify({
        type: "error",
        message: "Failed to download data",
        description: error?.response?.data?.error || "Error downloading data",
      });
    }
  }, []);

  const generateMentorActivityReport = useCallback(async () => {
    try {
      await dashboardService.generateMentorActivityReport();
      setNotify({
        type: "success",
        message: "Download Successful",
        description: "Mentor Activity Report downloaded successfully.",
      });
    } catch (error: any) {
      setNotify({
        type: "error",
        message: "Failed to download data",
        description: error || "Error downloading data",
      });
    }
  }, []);

  return (
    <Card
      title={
        <span className="text-white text-lg font-semibold flex items-center gap-2">
          <HiOutlineDocumentDownload className="text-purple-400" />
          Report Actions
        </span>
      }
      className="border-slate-500/30 backdrop-blur-sm lg:col-span-1"
    >
      <div className="grid grid-cols-1 gap-4">
        <Button
          size="large"
          className="bg-purple-500 hover:bg-purple-600 text-white border-purple-500 h-12 flex items-center justify-between"
          onClick={generateMonthlyMentorApplicationReport}
        >
          <span className="flex items-center gap-2">
            <FileOutlined />
            Monthly Mentor Application Report
          </span>
        </Button>
        <Button
          size="large"
          className="bg-purple-500 hover:bg-purple-600 text-white border-purple-500 h-12 flex items-center justify-between"
          onClick={generateMentorActivityReport}
        >
          <span className="flex items-center gap-2">
            <FileOutlined />
            Mentor Activity Report
          </span>
        </Button>
      </div>
    </Card>
  );
}