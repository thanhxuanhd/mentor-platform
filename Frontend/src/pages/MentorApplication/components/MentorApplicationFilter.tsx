import { Segmented } from "antd";
import Search from "antd/es/input/Search";
import { ApplicationStatus } from "../../../types/enums/ApplicationStatus";
import type { SegmentedOptions } from "antd/es/segmented";
import type { MentorApplicationFilterProp } from "../../../types/MentorApplicationType";

export default function MentorApplicationFilter({ onFilterChange }: { onFilterChange: (newFilters: MentorApplicationFilterProp) => void }) {

  const statusOptions: SegmentedOptions<ApplicationStatus> = [
    { label: "Submitted", value: ApplicationStatus.Submitted },
    { label: "Waiting Info", value: ApplicationStatus.WaitingInfo },
    { label: "Approved", value: ApplicationStatus.Approved },
    { label: "Rejected", value: ApplicationStatus.Rejected },
  ]

  return (
    <div className="flex flex-row items-center justify-between w-full text-slate-400 text-sm mb-2">
      <Search
        placeholder="Search by mentor name..."
        allowClear
        size="large"
        enterButton
        className="max-w-sm"
        maxLength={200}
        onSearch={(value) => onFilterChange({ keyword: value.trimStart().trimEnd() })}
      />
      <Segmented<ApplicationStatus>
        options={statusOptions}
        size="large"
        name="roleSegment"
        className="max-w-sm"
        onChange={(value) => onFilterChange({ status: value })}
      />
    </div>
  );
}