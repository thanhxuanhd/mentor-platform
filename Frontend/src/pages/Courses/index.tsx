import React, {Suspense, useState} from "react";
import { SearchBar, type SearchBarOptions } from "./searchBar.tsx";
import { Pagination } from "antd";

import { CourseStatesEnumMember, initialFormData } from "./initial-values.tsx";
import type { Category, Course, CourseFormDataOptions } from "./types.tsx";
import { CourseDetail } from "./courseDetail.tsx";
import { CoursePopoverTarget } from "./coursePopoverTarget.tsx";
import { CourseTable } from "./courseTable.tsx";
import { CourseForm } from "./courseForm.tsx";

import * as CourseClient from "./courseClient.tsx";
import { CourseResource } from "./courseResource.tsx";

const Page: React.FC = () => {
  const [categories, setCategories] = useState<Category[]>([]);
  const [states] = useState<Record<string, string>>(CourseStatesEnumMember);

  const [searchBarOptions, setSearchBarOptions] = useState<
    SearchBarOptions | undefined
  >();
  const [pageIndex, setPageIndex] = useState<number>(0);
  const [pageSize, setPageSize] = useState<number>(10);
  const [totalCount, setTotalCount] = useState<number>(0);

  const [item, setItem] = useState<Course | undefined>();
  const [formData, setFormData] =
    useState<CourseFormDataOptions>(initialFormData);

  const [popoverTarget, setPopoverTarget] = useState<string | undefined>();

  const fetchCourses = async () => {

    const response = await CourseClient.list({
      pageIndex,
      pageSize,
      keyword: searchBarOptions?.keyword,
      state: searchBarOptions?.state,
      categoryId: searchBarOptions?.categoryId,
    });

    return response;
  };

  return (
    <>
      <div className="min-h-screen bg-gray-900 text-gray-200">
        <div className="container mx-auto p-4">
          <div className="bg-gray-800 rounded-lg shadow-lg overflow-hidden">
            <div className="p-6">
              <div className="flex justify-between items-center mb-6">
                <h1 className="text-2xl font-semibold">
                  Course Management (Admin)
                </h1>
                <button
                  onClick={() => {
                    setPopoverTarget(CoursePopoverTarget.add);
                    setFormData(initialFormData);
                  }}
                  className="px-4 py-2 bg-orange-500 hover:bg-orange-600 text-white rounded-md transition duration-200"
                >
                  Add New Course
                </button>
              </div>

              <SearchBar
                categories={categories}
                states={states}
                onChange={async (options) => {
                  setSearchBarOptions(options);
                  await fetchCourses();
                }}
              />

              <Suspense fallback={<div>Loading...</div>}>
                <CourseTable
                  coursesPromise={fetchCourses()}
                  states={states}
                  onResourceView={(course) => {
                    setItem(course);
                    setPopoverTarget(CoursePopoverTarget.resource);
                  }}
                  onView={(course) => {
                    setItem(course);
                    setPopoverTarget(CoursePopoverTarget.detail);
                  }}
                  onDelete={(course) => {
                    // TODO: handle within CourseTable
                    setItem(course);
                    setPopoverTarget(CoursePopoverTarget.remove);
                  }}
                  onEdit={(course) => {
                    setItem(course);
                    setFormData({
                      categoryId: course.categoryId,
                      description: course.description,
                      difficulty: course.difficulty,
                      dueDate: course.dueDate,
                      status: course.status,
                      tags: course.tags,
                      title: course.title,
                    });
                    setPopoverTarget(CoursePopoverTarget.edit);
                  }}
              /></Suspense>

              <div className="flex justify-center items-center mt-6">
                <Pagination
                  current={pageIndex}
                  pageSize={pageSize}
                  total={totalCount}
                  showTotal={(total, range) =>
                    `${range[0]}-${range[1]} of ${total} items`
                  }
                  onChange={async (page, pageSize) => {
                    setPageIndex(page);
                    setPageSize(pageSize);
                    await fetchCourses();
                  }}
                />
              </div>

              <CourseForm
                formData={formData}
                categories={categories}
                states={states}
                active={
                  popoverTarget === CoursePopoverTarget.add ||
                  popoverTarget === CoursePopoverTarget.edit
                }
                onClose={(targetAction) => setPopoverTarget(targetAction)}
              />

              <CourseDetail
                course={item}
                states={states}
                active={popoverTarget === CoursePopoverTarget.detail}
                onClose={(targetAction) => setPopoverTarget(targetAction)}
              />

              <CourseResource
                course={item}
                onDownload={(material) => window.alert(material.url)}
                active={popoverTarget === CoursePopoverTarget.resource}
                onClose={(targetAction) => setPopoverTarget(targetAction)}
              />
            </div>
          </div>
        </div>
      </div>
    </>
  );
};

export default Page;
