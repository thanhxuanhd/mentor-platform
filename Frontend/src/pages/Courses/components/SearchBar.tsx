import {
  type ChangeEvent,
  type CSSProperties,
  type FC,
  useEffect,
  useState,
} from "react";
import { Button, Flex, Input, Select, Tooltip } from "antd";
import { CloseOutlined, SearchOutlined } from "@ant-design/icons";
import { categoryService } from "../../../services/category";
import { mentorService } from "../../../services/mentor";
import type { SearchBarProps } from "../../../types/pages/courses/types.ts";
import type { Category, Mentor } from "../types";

export type SearchBarOptions = {
  keyword?: string;
  difficulty?: string;
  status?: string;
  categoryId?: string;
  mentorId?: string;
};

export const SearchBar: FC<SearchBarProps> = ({
                                                states,
                                                categories: initialCategories,
                                                mentors: initialMentors,
                                                difficulties,
                                                onChange,
                                              }) => {
  const [keyword, setKeyword] = useState<string | undefined>();
  const [difficulty, setDifficulty] = useState<string | undefined>();
  const [categoryId, setCategoryId] = useState<string | undefined>();
  const [mentorId, setMentorId] = useState<string | undefined>();
  const [status, setStatus] = useState<string | undefined>();

  const [categoryKeyword, setCategoryKeyword] = useState<string>("");
  const [mentorKeyword, setMentorKeyword] = useState<string>("");
  const [filteredCategories, setFilteredCategories] =
      useState<Category[]>(initialCategories);
  const [filteredMentors, setFilteredMentors] =
      useState<Mentor[]>(initialMentors);
  const [loadingCategories, setLoadingCategories] = useState<boolean>(false);
  const [loadingMentors, setLoadingMentors] = useState<boolean>(false);

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

  const fetchCategories = async (searchKeyword: string) => {
    setLoadingCategories(true);
    try {
      const response = await categoryService.list({
        pageSize: 100,
        keyword: searchKeyword,
        status: undefined,
      });
      setFilteredCategories(response.items);
    } catch (error) {
      console.error("Error fetching categories:", error);
      setFilteredCategories([]);
    } finally {
      setLoadingCategories(false);
    }
  };

  const fetchMentors = async (searchKeyword: string) => {
    setLoadingMentors(true);
    try {
      const response = await mentorService.list({
        pageSize: 100,
        fullName: searchKeyword
      });
      setFilteredMentors(response.items);
    } catch (error) {
      console.error("Error fetching mentors:", error);
      setFilteredMentors([]);
    } finally {
      setLoadingMentors(false);
    }
  };

  useEffect(() => {
    if (categoryKeyword !== "") {
      fetchCategories(categoryKeyword);
    } else {
      setFilteredCategories(initialCategories);
    }
  }, [categoryKeyword, initialCategories]);

  useEffect(() => {
    if (mentorKeyword !== "") {
      fetchMentors(mentorKeyword);
    } else {
      setFilteredMentors(initialMentors);
    }
  }, [mentorKeyword, initialCategories]);

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
    setCategoryKeyword("");
    setMentorKeyword("");
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
  const selectStyle: CSSProperties = { minWidth: "110px" };

  return (
      <Flex
          align="center"
          gap="small"
          wrap="wrap"
          style={{
            width: "100%",
            padding: "8px 12px",
            background: "#28323f",
            borderRadius: "4px",
            marginBottom: "20px",
          }}
      >
        <Input
            id="search"
            value={keyword || ""}
            onChange={handleKeywordChange}
            placeholder="Filter by keyword"
            prefix={<SearchOutlined />}
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
            placeholder="Difficulty"
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
            showSearch
            filterOption={false}
            onSearch={setCategoryKeyword}
            loading={loadingCategories}
            style={selectStyle}
            options={filteredCategories.map((category) => ({
              key: category.id,
              value: category.id,
              label: category.name,
            }))}
            notFoundContent={
              categoryKeyword && !loadingCategories
                  ? "No matching categories"
                  : loadingCategories
                      ? "Loading..."
                      : "Type to search categories"
            }
        />
        <Select
            id="mentor"
            value={mentorId}
            onChange={handleMentorChange}
            placeholder="Mentor"
            allowClear
            variant="borderless"
            showSearch
            filterOption={false}
            onSearch={setMentorKeyword}
            loading={loadingMentors}
            style={selectStyle}
            options={filteredMentors.map((mentor) => ({
              key: mentor.id,
              value: mentor.id,
              label: mentor.fullName,
            }))}
            notFoundContent={
              mentorKeyword && !loadingMentors
                  ? "No matching mentors"
                  : loadingMentors
                      ? "Loading..."
                      : "Type to search mentors"
            }
        />
        <Select
            id="status"
            value={status}
            onChange={handleStatusChange}
            placeholder="Status"
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