import React, { useEffect, useState } from "react";

import { CourseStatesEnumMember, initialFormData } from "./initial-values.tsx";
import type {
  Category,
  Course,
  CourseFormDataOptions,
  Mentor,
} from "./types.tsx";
import { CoursePopoverTarget } from "./coursePopoverTarget.tsx";
import { CourseTable } from "./CourseTable.tsx";
import { CourseForm } from "./CourseForm.tsx";

import { CourseResource } from "./CourseResource.tsx";
import * as CourseClient from "./courseClient.tsx";
import { CourseDetail } from "./CourseDetail.tsx";
import { SearchBar } from "./SearchBar.tsx";

const Page: React.FC = () => {
  const [pageIndex, setPageIndex] = useState<number>(0);
  const [pageSize, setPageSize] = useState<number>(10);
  const [totalCount, setTotalCount] = useState<number>(0);
  const [loading, setLoading] = useState<boolean>(false);
  const [keyword, setKeyword] = useState<string | undefined>();
  const [state, setState] = useState<string | undefined>();
  const [categoryId, setCategoryId] = useState<string | undefined>();
  const [mentorId, setMentorId] = useState<string | undefined>();

  const [popoverTarget, setPopoverTarget] = useState<string | undefined>();

  const [categories, setCategories] = useState<Category[]>([]);
  const [mentors, setMentors] = useState<Mentor[]>([]);
  const [states] = useState<Record<string, string>>(CourseStatesEnumMember);
  const [courses, setCourses] = useState<Course[]>([]);
  const [item, setItem] = useState<Course | undefined>();
  const [formData, setFormData] =
    useState<CourseFormDataOptions>(initialFormData);

  useEffect(() => {
    const fetchCourses = async () => {
      setLoading(true);

      const courseResponse = await CourseClient.list({
        pageIndex,
        pageSize,
        keyword: keyword,
        state: state,
        categoryId: categoryId,
        mentorId: mentorId,
      });

      const categoryResponse = await CourseClient.categoryList();

      const mentorResponse = await CourseClient.mentorList();

      setTotalCount(courseResponse.totalPages);
      setCategories(categoryResponse.items);
      setMentors(mentorResponse.items);
      setCourses(courseResponse.items);

      setLoading(false);
    };

    fetchCourses();
  }, [pageIndex, pageSize, keyword, state, categoryId, mentorId]);

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
                mentors={mentors}
                onChange={(options) => {
                  setKeyword(options.keyword);
                  setState(options.state);
                  setCategoryId(options.categoryId);
                  setMentorId(options.mentorId);
                }}
              />

              <CourseTable
                courses={courses}
                states={states}
                tableProps={{
                  loading: loading,
                  pagination: {
                    pageSize: pageSize,
                    total: totalCount,
                    position: ["bottomRight"],
                    showTotal: (total, range) =>
                      `${range[0]}-${range[1]} of ${total} items`,
                    onChange: async (pageNumber, pageSize) => {
                      setPageIndex(pageNumber - 1);
                      setPageSize(pageSize);
                    },
                  },
                }}
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
              />

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
