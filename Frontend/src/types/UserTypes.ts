export interface User {
  id: number;
  name: string;
  email: string;
  role: "Admin" | "Mentor" | "Learner";
  joinedDate: string;
  status: "Pending" | "Active" | "Deactivated";
  lastActive: string;
}
