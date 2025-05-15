import { Routes, Route } from "react-router-dom";
import MainLayout from "../components/MainLayout";
import DashboardPage from "../pages/Dashboard";
import UsersPage from "../pages/Users";
import CategoriesPage from "../pages/Categories";
import CoursesPage from "../pages/Courses";
import NotFoundPage from "../pages/NotFound";
import {Login, SignUp} from "../pages/Auth";
import ResetPassword from "../pages/Auth/ResetPassword";
//import { AuthRequired } from "../components/AuthRequired"

const AppRoutes = () => {
  return (
    <Routes>
      <Route path="login" element={<Login />} />
      <Route path="signup" element={<SignUp />} />
      <Route path="reset-password" element={<ResetPassword />} />
      <Route
        element={
          // <AuthRequired>
            <MainLayout />
          // </AuthRequired>
        }
      >
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
