import { type ChangeEvent, type FC, useEffect, useState } from "react";
import { Input, Select } from "antd";
import { categoryService } from "../../../services/category";
import { mentorService } from "../../../services/mentor";
import type { SearchBarProps } from "../../../types/pages/courses/types.ts";
import type { Category, Mentor } from "../types";
import { useAuth } from "../../../hooks";
import { applicationRole } from "../../../constants/role.ts";

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

  const [categoryKeyword, setCategoryKeyword] = useState<string>("");
  const [mentorKeyword, setMentorKeyword] = useState<string>("");
  const [filteredCategories, setFilteredCategories] =
    useState<Category[]>(categories);
  const [filteredMentors, setFilteredMentors] = useState<Mentor[]>(mentors);
  const [loadingCategories, setLoadingCategories] = useState<boolean>(false);
  const [loadingMentors, setLoadingMentors] = useState<boolean>(false);

  const { user } = useAuth();

  const updateSearchBar = (props: Partial<SearchBarOptions>) => {
    onChange({
      keyword: keyword,
      difficulty: difficulty,
      categoryId: categoryId,
      mentorId: mentorId,
      status: status,
      ...props,
    });
  };

  const fetchCategories = async (searchKeyword: string) => {
    setLoadingCategories(true);
    try {
      const response = await categoryService.list({
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
        fullName: searchKeyword,
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
      const timer = setTimeout(() => {
        fetchCategories(categoryKeyword);
      }, 300);
      return () => clearTimeout(timer);
    } else {
      setFilteredCategories(categories);
    }
  }, [categoryKeyword, categories]);

  useEffect(() => {
    if (mentorKeyword !== "") {
      const timer = setTimeout(() => {
        fetchMentors(mentorKeyword);
      }, 300);
      return () => clearTimeout(timer);
    } else {
      setFilteredMentors(mentors);
    }
  }, [mentorKeyword, mentors]);

  function handleKeywordChange(event: ChangeEvent<HTMLInputElement>) {
    setKeyword(event.target.value);
    updateSearchBar({
      keyword: event.target.value,
    });
  }

  function handleDifficultyChange(value: string | undefined) {
    // Normalize "" to undefined if it could come from an explicit option
    // For allowClear, `undefined` is passed directly.
    const valueToSet = value === "" ? undefined : value;
    setDifficulty(valueToSet);
    updateSearchBar({
      difficulty: valueToSet,
    });
  }

  function handleCategoryChange(value: string | undefined) {
    setCategoryId(value);
    updateSearchBar({ categoryId: value });
    if (!value) setCategoryKeyword("");
  }

  function handleMentorChange(value: string | undefined) {
    setMentorId(value);
    updateSearchBar({ mentorId: value });
    if (!value) setMentorKeyword("");
  }

  function handleStatusChange(value: string | undefined) {
    // Normalize "" to undefined if it could come from an explicit option
    const valueToSet = value === "" ? undefined : value;
    setStatus(valueToSet);
    updateSearchBar({
      status: valueToSet,
    });
  }

  const commonInputClassName =
    "w-full bg-gray-700 border border-gray-600 rounded-md py-2 px-3 text-sm focus:outline-none focus:ring-2 focus:ring-orange-500";

  return (
    <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4 mb-6">
      <div>
        <label htmlFor="search" className="block text-sm font-medium mb-1">
          Search
        </label>
        <Input
          id="search"
          value={keyword}
          onChange={handleKeywordChange}
          placeholder="Filter by keyword"
          className={commonInputClassName}
          allowClear
        />
      </div>
      <div>
        <label htmlFor="difficulty" className="block text-sm font-medium mb-1">
          Difficulty
        </label>
        <Select
          id="difficulty"
          value={difficulty}
          onChange={handleDifficultyChange}
          placeholder="Select Difficulty"
          allowClear
          className={commonInputClassName}
          options={Object.entries(difficulties).map(
            ([enumKey, memberName]) => ({
              key: enumKey,
              value: memberName,
              label: memberName,
            }),
          )}
        />
      </div>

      <div>
        <label htmlFor="category" className="block text-sm font-medium mb-1">
          Category
        </label>
        <Select
          id="category"
          value={categoryId}
          onChange={handleCategoryChange}
          placeholder="Select Category"
          allowClear
          showSearch
          filterOption={false}
          onSearch={setCategoryKeyword}
          loading={loadingCategories}
          className={commonInputClassName}
          options={filteredCategories.map((category) => ({
            key: category.id,
            value: category.id,
            label: category.name,
          }))}
          notFoundContent={
            loadingCategories
              ? "Loading..."
              : categoryKeyword
                ? "No matching categories"
                : "Type to search categories"
          }
        />
      </div>

      {user?.role !== applicationRole.MENTOR && (
        <div>
          <label htmlFor="mentor" className="block text-sm font-medium mb-1">
            Mentor
          </label>
          <Select
            id="mentor"
            value={mentorId}
            onChange={handleMentorChange}
            placeholder="Select Mentor"
            allowClear
            showSearch
            filterOption={false}
            className={commonInputClassName}
            onSearch={setMentorKeyword}
            loading={loadingMentors}
            options={filteredMentors.map((mentor) => ({
              key: mentor.id,
              value: mentor.id,
              label: mentor.fullName,
            }))}
            notFoundContent={
              loadingMentors
                ? "Loading..."
                : mentorKeyword
                  ? "No matching mentors"
                  : "Type to search mentors"
            }
          />
        </div>
      )}

      <div>
        <label htmlFor="status" className="block text-sm font-medium mb-1">
          Status
        </label>
        <Select
          id="status"
          value={status}
          onChange={handleStatusChange}
          placeholder="Select Status"
          allowClear
          className={commonInputClassName}
          options={Object.entries(states).map(([key, stateValue]) => ({
            key: key,
            value: stateValue,
            label: stateValue,
          }))}
        />
      </div>
    </div>
  );
};
