import React from "react";
import { Pagination, Select } from "antd";
import type { PaginationProps } from "../../types/Pagination";

const PaginationControls = <T,>({
  pagination,
  onPageChange,
  onPageSizeChange,
  itemName = "items",
}: PaginationProps<T>) => {
  const { pageIndex, pageSize, totalCount } = pagination;

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
          options={[
            { value: 5, label: "5" },
            { value: 10, label: "10" },
            { value: 25, label: "25" },
          ]}
          style={{ width: 80 }}
        />
      </div>
    </div>
  );
};

export default PaginationControls;
