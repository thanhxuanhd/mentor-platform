// src/pages/NotFoundPage.tsx
import { Button } from "antd";
import { useNavigate } from "react-router-dom";

const NotFoundPage = () => {
  const navigate = useNavigate();

  return (
    <div className="flex h-100 items-center justify-center bg-gray-900">
      <div className="text-center">
        <h1 className="text-4xl font-bold mb-4 text-orange-500">404</h1>
        <p className="text-lg mb-6">
          The page you are looking for does not exist.
        </p>
        <Button type="primary" onClick={() => navigate("/")}>
          Back to Home
        </Button>
      </div>
    </div>
  );
};

export default NotFoundPage;
