export type Mentor = {
  id: string;
  name: string;
};

export type Category = {
  id: string;
  name: string;
};

export type Course = {
  id: string;
  title: string;
  description: string;
  categoryId: string;
  categoryName: string;
  createdAt: string;
  updatedAt: string;
  status: string;
  enrolledStudents: number;
  completionRate: number;
  dueDate: string;
  difficulty: string;
  tags: string[];
  materials: CourseMaterial[];
  mentorId: string;
  mentorName?: string;
  feedback: Feedback[];
};

export type CourseMaterial = {
  id: string;
  name: string;
  mediaType: string;
  url: string;
  uploadedAt: string;
};

export type Feedback = {
  id: string;
  userId: string;
  rating: number;
  comment: string;
  createdAt: string;
};

export type CourseFormDataOptions = {
  title: string;
  description: string;
  categoryId: string;
  status: string;
  dueDate: string;
  difficulty: string;
  tags: string[];
};
