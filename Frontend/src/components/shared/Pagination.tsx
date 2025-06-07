import { Pagination, Select } from "antd";
import type { PaginationProps } from "../../types/Pagination";

const PaginationControls = ({
  pageIndex,
  pageSize,
  totalCount,
  onPageChange,
  onPageSizeChange,
  pageSizeOptions = [5, 10, 25],
  itemName = "items",
}: PaginationProps) => {
  // Only show pagination if there are items
  if (totalCount === 0) return null;

  return (
    <div className="flex justify-between items-center mb-2 flex-wrap">
      <div>
        {`${(pageIndex - 1) * pageSize + 1}-${Math.min(
          pageIndex * pageSize,
          totalCount,
        )} of ${totalCount} ${itemName}`}
      </div>

      <Pagination
        current={pageIndex}
        pageSize={pageSize}
        total={totalCount}
        onChange={onPageChange}
        showSizeChanger={false}
      />

      <div className="flex items-center">
        <span className="mr-2">Items per page:</span>
        <Select
          value={pageSize}
          onChange={onPageSizeChange}
          options={pageSizeOptions.map((option) => {
            return { label: option.toString(), value: option };
          })}
          style={{ width: 80 }}
        />
      </div>
    </div>
  );
};

export default PaginationControls;
