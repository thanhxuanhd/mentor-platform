import { Tag } from "antd";

export const renderStatusTag = (status: string) => {
  const statusColors: Record<string, string> = {
    "Submitted": "orange",
    "WaitingInfo": "blue",
    "Approved": "green",
    "Rejected": "red",
  };

  return (
    <Tag
      color={statusColors[status]}
      className="text-xs px-3 py-1 rounded-full m-0 font-medium shadow-lg"
    >
      {status}
    </Tag>
  );
};
