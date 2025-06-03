export interface CreateAndEditCourse {
    title: string;
    category: string;
    difficulty: string;
    dueDate: string;
    tags: string[];
    description: string
    expectedMessage: string;
}

export interface CourseView {
    keyword: string;
    category: string;
    difficulty: string;
    mentor: string;
    status: string;
    expectedMessage: string;
}
