import React from 'react';
import { Navigate } from 'react-router-dom';
import { useAuth } from '../hooks';
import Loading from '../components/Loading';

interface ProtectedRouteProps {
  requiredRole: string | string[];
  children?: React.ReactNode;
}

const ProtectedRoute: React.FC<ProtectedRouteProps> = ({ requiredRole, children }) => {
  const { user, isAuthenticated, loading } = useAuth();
  const currentPath = window.location.pathname;

  if (loading) {
    return <Loading />
  }

  if (!isAuthenticated) {
    return <Navigate to="/login" state={{ from: currentPath }} replace />;
  }

  const hasAccess = user && (
    Array.isArray(requiredRole)
      ? requiredRole.includes(user.role)
      : user.role === requiredRole
  );

  if (!hasAccess) {
    return <Navigate to="/forbidden" replace />;
  }

  return (<>{children}</>);
};

export default ProtectedRoute;