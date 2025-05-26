import { type ChangeEvent, type FC, useState } from "react";
import { Button, Flex, Input, Select, Tooltip } from "antd";
import { CloseOutlined, FilterOutlined } from "@ant-design/icons";

import type { SearchBarProps } from "../../../types/pages/courses/types.ts";

export type SearchBarOptions = {
  keyword?: string;
  difficulty?: string;
  status?: string;
  categoryId?: string;
  mentorId?: string;
};

export const SearchBar: FC<SearchBarProps> = ({
  states,
  categories,
  mentors,
  difficulties,
  onChange,
}) => {
  const [keyword, setKeyword] = useState<string | undefined>();
  const [difficulty, setDifficulty] = useState<string | undefined>();
  const [categoryId, setCategoryId] = useState<string | undefined>();
  const [mentorId, setMentorId] = useState<string | undefined>();
  const [status, setStatus] = useState<string | undefined>();

  const updateParentWithFilters = (updatedField: Partial<SearchBarOptions>) => {
    onChange({
      keyword: keyword,
      difficulty: difficulty,
      categoryId: categoryId,
      mentorId: mentorId,
      status: status,
      ...updatedField,
    });
  };

  function handleKeywordChange(event: ChangeEvent<HTMLInputElement>) {
    const value = event.target.value;
    const actualValue = value === "" ? undefined : value;
    setKeyword(actualValue);
    updateParentWithFilters({ keyword: actualValue });
  }

  function handleDifficultyChange(value: string | undefined) {
    setDifficulty(value);
    updateParentWithFilters({ difficulty: value });
  }

  function handleCategoryChange(value: string | undefined) {
    setCategoryId(value);
    updateParentWithFilters({ categoryId: value });
  }

  function handleMentorChange(value: string | undefined) {
    setMentorId(value);
    updateParentWithFilters({ mentorId: value });
  }

  function handleStatusChange(value: string | undefined) {
    setStatus(value);
    updateParentWithFilters({ status: value });
  }

  const clearAllFilters = () => {
    setKeyword(undefined);
    setDifficulty(undefined);
    setCategoryId(undefined);
    setMentorId(undefined);
    setStatus(undefined);
    onChange({
      keyword: undefined,
      difficulty: undefined,
      categoryId: undefined,
      mentorId: undefined,
      status: undefined,
    });
  };

  const anyFilterActive = !!(
    keyword ||
    difficulty ||
    categoryId ||
    mentorId ||
    status
  );

  const selectStyle: React.CSSProperties = { minWidth: "110px" }; // Adjusted for a slightly more compact look like image

  return (
    <Flex
      align="center"
      gap="small"
      wrap="wrap"
      style={{
        width: "100%",
        padding: "8px 12px",
        background: "#1F2937", // Distinct background color (e.g., Tailwind slate-800)
        borderRadius: "4px", // Matches the slight rounding in the image
        marginBottom: "20px", // Spacing between search bar and table
      }}
    >
      <Input
        id="search"
        value={keyword || ""}
        onChange={handleKeywordChange}
        placeholder="Filter by keyword"
        prefix={<FilterOutlined />}
        allowClear
        variant="borderless"
        style={{
          flex: "1 1 200px",
          minWidth: "180px",
        }}
      />
      <Select
        id="difficulty"
        value={difficulty}
        onChange={handleDifficultyChange}
        placeholder="Types"
        allowClear
        variant="borderless"
        style={selectStyle}
        options={Object.entries(difficulties).map(([key, memberName]) => ({
          key: key,
          value: String(memberName),
          label: String(memberName),
        }))}
      />
      <Select
        id="category"
        value={categoryId}
        onChange={handleCategoryChange}
        placeholder="Category"
        allowClear
        variant="borderless"
        style={selectStyle}
        options={categories.map((category) => ({
          key: category.id,
          value: String(category.id),
          label: category.name,
        }))}
      />
      <Select
        id="mentor"
        value={mentorId}
        onChange={handleMentorChange}
        placeholder="Mentor"
        allowClear
        variant="borderless"
        style={selectStyle}
        options={mentors.map((mentor) => ({
          key: mentor.id,
          value: String(mentor.id),
          label: mentor.fullName,
        }))}
      />
      <Select
        id="status"
        value={status}
        onChange={handleStatusChange}
        placeholder="States"
        allowClear
        variant="borderless"
        style={selectStyle}
        options={Object.entries(states).map(([key, stateValue]) => ({
          key: key,
          value: String(stateValue),
          label: String(stateValue),
        }))}
      />
      {anyFilterActive && (
        <Tooltip title="Clear all filters">
          <Button
            icon={<CloseOutlined />}
            onClick={clearAllFilters}
            type="text"
            ghost
            style={{ marginLeft: "auto" }}
          />
        </Tooltip>
      )}
    </Flex>
  );
};
