import React from "react";
import { Navigate } from "react-router-dom";
import { useAuth } from "../hooks";
import Loading from "../components/Loading";
import { applicationRole } from "../constants/role";

interface ProtectedRouteProps {
  requiredRole: string | string[];
  children?: React.ReactNode;
  checkMentorApplication?: boolean;
}

const ProtectedRoute: React.FC<ProtectedRouteProps> = ({
  requiredRole,
  children,
  checkMentorApplication,
}) => {
  const { user, isAuthenticated, loading, isMentorApproved } = useAuth();
  const currentPath = window.location.pathname;

  if (loading) {
    return <Loading />;
  }

  if (!isAuthenticated) {
    return <Navigate to="/login" state={{ from: currentPath }} replace />;
  }

  const hasAccess =
    user &&
    (Array.isArray(requiredRole)
      ? requiredRole.includes(user.role)
      : user.role === requiredRole);

  if (!hasAccess) {
    return <Navigate to="/forbidden" replace />;
  }

  if (
    user.role === applicationRole.MENTOR &&
    checkMentorApplication &&
    !isMentorApproved
  ) {
    return <Navigate to="my-applications" replace />;
  }

  return <>{children}</>;
};

export default ProtectedRoute;
