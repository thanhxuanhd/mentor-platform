import { Routes, Route } from "react-router-dom";
import MainLayout from "../components/MainLayout";
import DashboardPage from "../pages/Dashboard";
import UsersPage from "../pages/Users";
import CategoriesPage from "../pages/Categories";
import CoursesPage from "../pages/Courses";
import NotFoundPage from "../pages/NotFound";
import { Login, SignUp } from "../pages/Auth";
import ResetPassword from "../pages/Auth/ResetPassword";
import ForgotPassword from "../pages/Auth/ForgotPassword";
import OAuthCallback from "../pages/Auth/OAuthCallback";
import AuthLayout from "../components/AuthLayout";
import ProtectedRoute from "./ProtectedRoute";
import { applicationRole } from "../constants/role";
import ForbiddenPage from "../pages/Forbidden";
import Profile from '../pages/UserProfile/components/Profile'
import EditProfile from '../pages/UserProfile/components/EditProfile'

const AppRoutes = () => {
  return (
    <Routes>
      <Route element={<AuthLayout />}>
        <Route path="login" element={<Login />} />
        <Route path="signup" element={<SignUp />} />
        <Route path="profile-setup" element={<ProfileSetup />} />
        <Route path="reset-password" element={<ResetPassword />} />
        <Route path="forgot-password" element={<ForgotPassword />} />
        <Route path="auth/callback/:provider" element={<OAuthCallback />} />
        <Route path="*" element={<NotFoundPage />} />
        <Route path="forbidden" element={<ForbiddenPage />} />
      </Route>    
      <Route
        element={
          <ProtectedRoute
            requiredRole={[
              applicationRole.ADMIN,
              applicationRole.LEARNER,
              applicationRole.MENTOR,
            ]}
          >
            <MainLayout />
          </ProtectedRoute>
        }
      >
        <Route path="/" element={<DashboardPage />} />
        <Route path="dashboard" element={<DashboardPage />} />
        <Route path="courses" element={<CoursesPage />} />
      </Route>
      <Route
        element={
          <ProtectedRoute
            requiredRole={[applicationRole.ADMIN, applicationRole.LEARNER]}
          >
            <MainLayout />
          </ProtectedRoute>
        }
      >
        <Route path="users" element={<UsersPage />} />
        <Route path="categories" element={<CategoriesPage />} />
      </Route>
      <Route
        element={
          <ProtectedRoute
            requiredRole={[applicationRole.ADMIN, applicationRole.MENTOR, applicationRole.LEARNER]}
          >
            <MainLayout />
          </ProtectedRoute>
        }
      >
        <Route path="profile" element={<Profile />} />
        <Route path="profile/edit" element={<EditProfile />} />
      </Route>
    </Routes>
  );
};

export default AppRoutes;