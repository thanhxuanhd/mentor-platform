import React, { useEffect, useState } from "react";

import {CourseDifficultyEnumMember, CourseStatesEnumMember, initialFormData} from "./initial-values.tsx";
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
import { courseService } from "../../services/course";
import { categoryService } from "../../services/category";
import { mentorService } from "../../services/mentor";
import { CourseDetail } from "./CourseDetail.tsx";
import { SearchBar } from "./SearchBar.tsx";
import { App, Modal } from "antd";

const Page: React.FC = () => {  const [pageIndex, setPageIndex] = useState<number>(0);
  const [pageSize, setPageSize] = useState<number>(10);
  const [totalCount, setTotalCount] = useState<number>(0);
  const [loading, setLoading] = useState<boolean>(false);
  const [keyword, setKeyword] = useState<string | undefined>();
  const [difficulty, setDifficulty] = useState<string | undefined>();
  const [categoryId, setCategoryId] = useState<string | undefined>();
  const [mentorId, setMentorId] = useState<string | undefined>();
  const [refreshTrigger, setRefreshTrigger] = useState<number>(0);
  const [isRefreshing, setIsRefreshing] = useState<boolean>(false);

  const [popoverTarget, setPopoverTarget] = useState<string | undefined>();

  const [categories, setCategories] = useState<Category[]>([]);
  const [mentors, setMentors] = useState<Mentor[]>([]);
  const [states] = useState<Record<string, string>>(CourseStatesEnumMember);
  const [difficulties] = useState<Record<string, string>>(CourseDifficultyEnumMember);
  const [courses, setCourses] = useState<Course[]>([]);
  const [item, setItem] = useState<Course | undefined>();
  const [formData, setFormData] =
    useState<CourseFormDataOptions>(initialFormData);
  const { modal } = App.useApp();

  useEffect(() => {
    if (refreshTrigger > 0) {
      setIsRefreshing(true);
      const refreshData = async () => {
        try {
          const courseResponse = await courseService.list({
            pageIndex: 0, 
            pageSize,
            keyword,
            difficulty,
            categoryId,
            mentorId,
          });
          
          setCourses(courseResponse.items);
          setTotalCount(courseResponse.totalPages);
          console.log("Course list refreshed after create/update");
        } catch (error) {
          console.error("Error refreshing courses:", error);
        } finally {
          setIsRefreshing(false);
        }
      };
      
      refreshData();
    }
  }, [refreshTrigger]);

  useEffect(() => {    const fetchCourses = async () => {
      setLoading(true);

      try {
        // Get courses
        const courseResponse = await courseService.list({
          pageIndex,
          pageSize,
          keyword: keyword,
          difficulty: difficulty,
          categoryId: categoryId,
          mentorId: mentorId,
        });

        const categoryResponse = await categoryService.list();
        const mentorResponse = await mentorService.list();

        setTotalCount(courseResponse.totalPages);
        setCategories(categoryResponse.items);
        setMentors(mentorResponse.items);
        setCourses(courseResponse.items);
      } catch (error) {
        console.error("Error fetching data:", error);
      } finally {
        setLoading(false);
      }
    };

    fetchCourses();
  }, [pageIndex, pageSize, keyword, difficulty, categoryId, mentorId, refreshTrigger]);

  const handleDeleteCourse = async (course: Course) => {
    modal.confirm({
      title: "Are you sure delete this course?",
      content: `Course: ${course.title}`,
      okText: "Yes",
      okType: "danger",
      cancelText: "No",
      onOk: async () => {
        try {
          await courseService.delete(course.id);
          setRefreshTrigger(prev => prev + 1); // Refresh the list after deletion
        } catch (error) {
          console.error("Error deleting course:", error);
          Modal.error({
            title: "Failed to delete course",
            content: "There was an error deleting the course. Please try again.",
          });
        }
      },
    });
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
                difficulties={difficulties}
                mentors={mentors}
                onChange={(options) => {
                  setKeyword(options.keyword);
                  setDifficulty(options.difficulty);
                  setCategoryId(options.categoryId);
                  setMentorId(options.mentorId);
                }}
              />              
              <CourseTable
                courses={courses}
                states={states}
                tableProps={{
                  loading: loading || isRefreshing,
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
                onDelete={handleDeleteCourse}                onEdit={(course) => {
                  setItem(course);
                  setFormData({
                    id: course.id,
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
                onClose={(targetAction) => {
                  if (targetAction === "refresh") {
                    // Trigger a refresh of the course list
                    setRefreshTrigger(prev => prev + 1);
                  }
                  setPopoverTarget(targetAction);
                }}
              />              
              <CourseDetail
                course={item}
                states={states}
                active={popoverTarget === CoursePopoverTarget.detail}
                onClose={(targetAction) => {
                  if (targetAction === "refresh") {
                    // Trigger a refresh of the course list
                    setRefreshTrigger(prev => prev + 1);
                  }
                  setPopoverTarget(targetAction);
                }}
              />              
              <CourseResource
                course={item}
                onDownload={(material) => window.alert(material.webAddress)}
                active={popoverTarget === CoursePopoverTarget.resource}
                onClose={(targetAction) => {
                  if (targetAction === "refresh") {
                    setRefreshTrigger(prev => prev + 1);
                  }
                  setPopoverTarget(targetAction);
                }}
              />
            </div>
          </div>
        </div>
      </div>
    </>
  );
};

export default Page;
