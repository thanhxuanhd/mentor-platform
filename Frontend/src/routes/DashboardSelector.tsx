import React from "react";
import { useAuth } from "../hooks";
import AdminDashboard from "../pages/Dashboard/AdminDashboard";
import { applicationRole } from "../constants/role";
import DashboardPage from "../pages/Dashboard";

const DashboardSelector: React.FC = () => {
  const { user } = useAuth();

  switch (user?.role) {
    case applicationRole.ADMIN:
      return <AdminDashboard />;
    case applicationRole.MENTOR:
      return <DashboardPage />;
    case applicationRole.LEARNER:
      return <DashboardPage />;
    default:
      return <div>Unauthorized or invalid role</div>;
  }
};

export default DashboardSelector;