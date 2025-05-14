import { Routes, Route } from "react-router-dom";
import MainLayout from "../components/MainLayout";
import DashboardPage from "../pages/Dashboard";
import UsersPage from "../pages/Users";
import CategoriesPage from "../pages/Categories";
import CoursesPage from "../pages/Courses";
import NotFoundPage from "../pages/NotFound";

const AppRoutes = () => {
  return (
    <Routes>
      <Route element={<MainLayout />}>
        <Route path="/" element={<DashboardPage />} />
        <Route path="users" element={<UsersPage />} />
        <Route path="categories" element={<CategoriesPage />} />
        <Route path="courses" element={<CoursesPage />} />
        <Route path="*" element={<NotFoundPage />} />
      </Route>
    </Routes>
  );
};

export default AppRoutes;
