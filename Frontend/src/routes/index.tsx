import { Routes, Route } from 'react-router-dom';
import MainLayout from '../components/MainLayout';
import DashboardPage from '../pages/Dashboard';
import UsersPage from '../pages/Users';
import CategoriesPage from '../pages/Categories';
import CoursesPage from '../pages/Courses';
import NotFoundPage from '../pages/NotFound';
import { Login, SignUp } from '../pages/Auth';
import ResetPassword from '../pages/Auth/ResetPassword';
import ForgotPassword from '../pages/Auth/ForgotPassword';
import OAuthCallback from '../pages/Auth/OAuthCallback';
import ProtectedRoute from './ProtectedRoute';
import { applicationRole } from '../constants/role';
import Test from '../components/Test';

const AppRoutes = () => {
  return (
    <Routes>
      <Route path="login" element={<Login />} />
      <Route path="signup" element={<SignUp />} />
      <Route path="reset-password" element={<ResetPassword />} />
      <Route path="forgot-password" element={<ForgotPassword />} />
      <Route path="auth/callback/:provider" element={<OAuthCallback />} />
      <Route path="step2" element={<Test />} />
      <Route
        element={
          <ProtectedRoute requiredRole={[applicationRole.ADMIN, applicationRole.LEARNER]}>
            <MainLayout />
          </ProtectedRoute>
        }
      >
        <Route path="/" element={<DashboardPage />} />
        <Route path="dashboard" element={<DashboardPage />} />
        <Route path="users" element={<UsersPage />} />
        <Route path="categories" element={<CategoriesPage />} />
        <Route path="courses" element={<CoursesPage />} />
      </Route>
      <Route
        element={
          <MainLayout />
        }
      >
        <Route path="*" element={<NotFoundPage />} />
      </Route>
    </Routes>
  );
};

export default AppRoutes;
