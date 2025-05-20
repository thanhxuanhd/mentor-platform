import { Routes, Route } from "react-router-dom";
import MainLayout from "../components/MainLayout";
import DashboardPage from "../pages/Dashboard";
import UsersPage from "../pages/Users";
import CategoriesPage from "../pages/Categories";
import CoursesPage from "../pages/Courses";
import NotFoundPage from "../pages/NotFound";
import { Login, SignUp } from "../pages/Auth";
import ResetPassword from "../pages/Auth/ResetPassword";
import OAuthCallback from "../pages/Auth/OAuthCallback";
import { AuthRequired } from "../components/AuthRequired";
import AuthLayout from "../components/AuthLayout";
import UserProfile from "../pages/Auth/UserProfile";

const AppRoutes = () => {
  return (
    <Routes>
      <Route element={<AuthLayout />}>
        <Route path="login" element={<Login />} />
        <Route path="signup" element={<SignUp />} />
        <Route path="signup/step2" element={<UserProfile />} />
        <Route path="reset-password" element={<ResetPassword />} />
        <Route path="auth/callback/:provider" element={<OAuthCallback />} />
      </Route>

      <Route
        element={
          <AuthRequired>
            <MainLayout />
          </AuthRequired>
        }
      >
        <Route path="/" element={<DashboardPage />} />
        <Route path="dashboard" element={<DashboardPage />} />
        <Route path="users" element={<UsersPage />} />
        <Route path="categories" element={<CategoriesPage />} />
        <Route path="courses" element={<CoursesPage />} />
      </Route>
      <Route path="*" element={<NotFoundPage />} />
    </Routes>
  );
};

export default AppRoutes;
