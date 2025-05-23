import type {Category, Course, CourseFormDataOptions, CourseItem, Mentor} from "../../../pages/Courses/types.tsx";
import type {TableProps} from "antd";
import type {CourseDifficulty} from "../../../pages/Courses/initial-values.tsx";
import type {SearchBarOptions} from "../../../pages/Courses/components/SearchBar.tsx";

export type CourseDetailProps = {
    course?: Course;
    states: Record<string, string>;
    active: boolean;
    onClose: (targetAction?: string | undefined) => void;
};

export type CourseFormProps = {
    formData: CourseFormDataOptions;
    categories: Category[];
    states: Record<string, string>;
    active: boolean;
    onClose: (targetAction?: string | undefined) => void;
};

export type CourseMaterialFormProps = {
    visible: boolean;
    courseId?: string;
    onCancel: () => void;
    onSuccess: () => void;
}

export type CourseResourceProps = {
    course?: Course;
    onDownload: (material: CourseItem) => void;
    active: boolean;
    onClose: (targetAction?: string | undefined) => void;
};

export type CourseTableProps = {
    courses: Course[];
    states: Record<string, string>;
    onResourceView: (course: Course) => void;
    onView: (course: Course) => void;
    onEdit: (course: Course) => void;
    onDelete: (course: Course) => void;
    tableProps: TableProps;
};

export type SearchBarProps = {
    categories: Category[];
    mentors: Mentor[];
    difficulties: Record<CourseDifficulty, string>;
    onChange: (options: SearchBarOptions) => void;
};