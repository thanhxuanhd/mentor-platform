// src/pages/NotFoundPage.tsx
import { Button } from "antd";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../../hooks";

const ForbiddenPage = () => {
  const navigate = useNavigate();
  const { removeToken } = useAuth();

  return (
    <div className="flex h-screen items-center justify-center bg-gray-900">
      <div className="text-center">
        <h1 className="text-4xl font-bold mb-4 text-orange-500">403</h1>
        <p className="text-lg mb-6">You are not allowed to access this page.</p>
        <Button
          type="primary"
          onClick={() => navigate("/")}
          className="mx-3 bg-gray-500! hover:bg-gray-400!"
        >
          Back to Home
        </Button>
        <Button
          type="primary"
          onClick={() => {
            removeToken();
            navigate("/login");
          }}
          className="mx-3"
        >
          Log out
        </Button>
      </div>
    </div>
  );
};

export default ForbiddenPage;
