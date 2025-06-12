import React from "react";
import { useAuth } from "../hooks";
import AdminDashboard from "../pages/Dashboard/AdminDashboard";
import { applicationRole } from "../constants/role";
import MentorDashboard from "../pages/Dashboard/MentorDashboard";
import LearnerDashboard from "../pages/Dashboard/LearnerDashboard";

const DashboardSelector: React.FC = () => {
  const { user } = useAuth();

  switch (user?.role) {
    case applicationRole.ADMIN:
      return <AdminDashboard />;
    case applicationRole.MENTOR:
      return <MentorDashboard />;
    case applicationRole.LEARNER:
      return <LearnerDashboard />;
    default:
      return <div>Unauthorized or invalid role</div>;
  }
};

export default DashboardSelector;