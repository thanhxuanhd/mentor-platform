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
  items: CourseItem[];
  tags: string[];
  status: string;
};

export type CourseItem = {
  id: string;
  title: string;
  description: string;
  mediaType: string;
  webAddress: string;
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
  categoryId: string;
  status: string;
  dueDate: string;
  difficulty: string;
  tags: string[];
};
