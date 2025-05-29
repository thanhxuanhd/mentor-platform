import { type ChangeEvent, type FC, useState } from "react";

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

  const updateSearchBar = (props: Record<string, string>) => {
    onChange({
      keyword: keyword,
      difficulty: difficulty,
      categoryId: categoryId,
      mentorId: mentorId,
      ...props,
    });
  };

  function handleCategoryChange(event: ChangeEvent<HTMLSelectElement>) {
    setCategoryId(event.target.value);
    updateSearchBar({
      categoryId: event.target.value,
    });
  }

  function handleKeywordChange(event: ChangeEvent<HTMLInputElement>) {
    setKeyword(event.target.value);
    updateSearchBar({
      keyword: event.target.value,
    });
  }

  function handleDifficultyChange(event: ChangeEvent<HTMLSelectElement>) {
    setDifficulty(event.target.value);
    updateSearchBar({
      difficulty: event.target.value,
    });
  }

  function handleMentorChange(event: ChangeEvent<HTMLSelectElement>) {
    setMentorId(event.target.value);
    updateSearchBar({
      mentorId: event.target.value,
    });
  }

  function handleStatusChange(event: ChangeEvent<HTMLSelectElement>) {
    setStatus(event.target.value);
    updateSearchBar({
      status: event.target.value,
    });
  }

  return (
    <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4 mb-6">
      <div>
        <label htmlFor="search" className="block text-sm font-medium mb-1">
          Search
        </label>
        <input
          type="text"
          id="search"
          value={keyword}
          onChange={handleKeywordChange}
          placeholder="Filter by keyword"
          className="w-full bg-gray-700 border border-gray-600 rounded-md py-2 px-3 text-sm focus:outline-none focus:ring-2 focus:ring-orange-500"
        />
      </div>
      <div>
        <label htmlFor="difficulty" className="block text-sm font-medium mb-1">
          Difficulty
        </label>
        <select
          id="difficulty"
          value={difficulty}
          onChange={handleDifficultyChange}
          className="w-full bg-gray-700 border border-gray-600 rounded-md py-2 px-3 text-sm focus:outline-none focus:ring-2 focus:ring-orange-500"
        >
          <option value="">-</option>
          {Object.entries(difficulties).map(([value, memberName]) => (
            <option key={value} value={memberName}>
              {memberName}
            </option>
          ))}
        </select>
      </div>
      <div>
        <label htmlFor="category" className="block text-sm font-medium mb-1">
          Category
        </label>
        <select
          id="category"
          value={categoryId}
          onChange={handleCategoryChange}
          className="w-full bg-gray-700 border border-gray-600 rounded-md py-2 px-3 text-sm focus:outline-none focus:ring-2 focus:ring-orange-500"
        >
          <option value="">-</option>
          {categories.map((category) => (
            <option key={category.id} value={category.id}>
              {category.name}
            </option>
          ))}
        </select>
      </div>
      <div>
        <label htmlFor="mentor" className="block text-sm font-medium mb-1">
          Mentor
        </label>
        <select
          id="mentor"
          value={mentorId}
          onChange={handleMentorChange}
          className="w-full bg-gray-700 border border-gray-600 rounded-md py-2 px-3 text-sm focus:outline-none focus:ring-2 focus:ring-orange-500"
        >
          <option value="">-</option>
          {mentors.map((mentor) => (
            <option key={mentor.id} value={mentor.id}>
              {mentor.fullName}
            </option>
          ))}
        </select>
      </div>
      <div>
        <label htmlFor="status" className="block text-sm font-medium mb-1">
          Status
        </label>
        <select
          id="status"
          value={status}
          onChange={handleStatusChange}
          className="w-full bg-gray-700 border border-gray-600 rounded-md py-2 px-3 text-sm focus:outline-none focus:ring-2 focus:ring-orange-500"
        >
          <option value="">-</option>
          {Object.entries(states).map(([key, value]) => (
            <option key={key} value={value}>
              {value}
            </option>
          ))}
        </select>
      </div>
    </div>
  );
};
