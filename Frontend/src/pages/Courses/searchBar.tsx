import { type ChangeEvent, useState } from "react";
import type { Category } from "./types.tsx";

export type SearchBarOptions = {
  keyword?: string;
  state?: string;
  categoryId?: string;
};

type SearchBarProp = {
  categories: Category[];
  states: Record<string, string>;
  onChange: (options: SearchBarOptions) => void;
};

export const SearchBar = ({ categories, states, onChange }: SearchBarProp) => {
  const [keyword, setKeyword] = useState<string | undefined>();
  const [state, setState] = useState<string | undefined>();
  const [categoryId, setCategoryId] = useState<string | undefined>();

  const updateSearchBar = () => {
    onChange({
      keyword: keyword,
      state: state,
      categoryId: categoryId,
    });
  };

  function handleCategoryChange(event: ChangeEvent<HTMLSelectElement>) {
    setCategoryId(event.target.value);
    updateSearchBar();
  }

  function handleKeywordChange(event: ChangeEvent<HTMLInputElement>) {
    setKeyword(event.target.value);
    updateSearchBar();
  }

  return (
    <>
      <div className="grid grid-cols-1 md:grid-cols-3 gap-4 mb-6">
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
          <label htmlFor="status" className="block text-sm font-medium mb-1">
            Status
          </label>
          <select
            id="status"
            value={state}
            onChange={(event) => setState(event.target.value)}
            className="w-full bg-gray-700 border border-gray-600 rounded-md py-2 px-3 text-sm focus:outline-none focus:ring-2 focus:ring-orange-500"
          >
            <option value="">-</option>
            {Object.entries(states).map(([value, memberName]) => (
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
      </div>
    </>
  );
};
