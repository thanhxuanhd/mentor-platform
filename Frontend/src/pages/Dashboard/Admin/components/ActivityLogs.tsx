import { Input, DatePicker, Table, Card } from "antd"
import type { ActivityLogRequest, ActivityLogResponse } from "../../../../types/ActivityLogType"
import { formatDateTime } from "../../../../utils/DateFormat"
import { CalendarOutlined, SearchOutlined, TeamOutlined } from "@ant-design/icons"
import dayjs from "dayjs"

const { RangePicker } = DatePicker;

interface ActivityLogTableProps {
  items: ActivityLogResponse[],
  loading: boolean
  filter: ActivityLogRequest;
  pagination: { pageIndex: number; totalCount: number };
  onFilter?: (filter: ActivityLogRequest) => void;
}

export default function ActivityLogTable({ items, loading, filter, pagination, onFilter = () => { } }: ActivityLogTableProps) {
  const columns = [
    {
      title: "Action",
      dataIndex: "action",
      key: "action"
    },
    {
      title: "Timestamp",
      dataIndex: "timestamp",
      key: "timestamp",
      render: (text: string) => <span className="text-slate-400">{formatDateTime(text)}</span>,
    },
  ]

  const handleKeywordChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const keyword = e.target.value.trimStart().trimEnd() || undefined;
    onFilter({ ...filter, keyword, pageIndex: 1 });
  };

  const handleDateChange = (
    dates: [dayjs.Dayjs | null, dayjs.Dayjs | null] | null
  ) => {
    onFilter({
      ...filter,
      startDateTime: dates && dates[0] ? dates[0].toDate() : undefined,
      endDateTime: dates && dates[1] ? dates[1].toDate() : undefined,
      pageIndex: 1,
    });
  };

  const handleTableChange = (pagination: any) => {
    onFilter({
      ...filter,
      pageIndex: pagination.current,
      pageSize: pagination.pageSize,
    });
  };

  return (
    <Card
      title={
        <span className=" text-white text-xl font-semibold flex items-center gap-2" >
          <TeamOutlined className="text-blue-400" />
          Recent Activity
        </span >
      }
      className="bg-slate-600/50 border-slate-500/30 backdrop-blur-sm lg:col-span-2"
    >
      <div className="grid grid-cols-1 lg:grid-cols-2 items-center gap-4 w-full mb-6">
        <Input
          placeholder="Search by keyword"
          value={filter.keyword}
          onChange={handleKeywordChange}
          prefix={<SearchOutlined className="text-slate-400" />}
          className="bg-slate-500/30 border-slate-400/50 text-white placeholder:text-slate-400"
        />
        <RangePicker
          onChange={handleDateChange}
          value={[
            filter.startDateTime ? dayjs(filter.startDateTime) : null,
            filter.endDateTime ? dayjs(filter.endDateTime) : null,
          ]}
          size="large"
          className="bg-slate-500/30 border-slate-400/50 text-white"
          style={{
            backgroundColor: "rgba(71, 85, 105, 0.3)",
            borderColor: "rgba(148, 163, 184, 0.5)",
          }}
          placeholder={["From", "To"]}
          suffixIcon={<CalendarOutlined className="text-slate-400" />}
        />
      </div>

      <Table
        rowKey={"id"}
        columns={columns}
        dataSource={items}
        pagination={{
          current: pagination.pageIndex,
          pageSize: filter.pageSize || 5,
          total: pagination.totalCount,
        }}
        loading={loading}
        rowClassName="bg-slate-500/30 hover:bg-slate-500/50 transition-colors"
        onChange={handleTableChange}
        locale={{ emptyText: "No recent activity" }}
      />
    </Card>
  )
}