export type Mentor = {
  id: string;
  fullName: string;
  email: string;
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
  mentorId: string;
  mentorName: string;
  difficulty: string;
  dueDate: string;
  items: CourseResource[];
  tags: string[];
  status: string;
};

export type CourseResource = {
  id: string;
  title: string;
  description: string;
  resourceType: string;
  resourceUrl: string;
};

export type Feedback = {
  id: string;
  userId: string;
  rating: number;
  comment: string;
  createdAt: string;
};

export type CourseFormDataOptions = {
  id?: string;
  title: string;
  description: string;
  categoryId?: string;
  categoryName?: string;
  status: string;
  dueDate: string;
  difficulty: string;
  tags: string[];
};
