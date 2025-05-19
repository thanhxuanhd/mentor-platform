import { Layout } from "antd";
import { Outlet } from "react-router-dom";

const { Header, Content } = Layout;

const AuthLayout = () => {
  return (
    <Layout>
      <Header className="flex align-center border-b border-gray-700 top-0">
        <div className="flex items-center justify-start gap-2">
          <img
            src="./assets/images/logo.svg"
            alt="Logo"
            className="h-10 w-10 sm:h-12 sm:w-12"
          />
          <p className="text-center text-md font-bold text-red-600 sm:text-xl">
            Rookies - Group 4
          </p>
        </div>
      </Header>

      <Content className="overflow-y-auto bg-gray-900">
        <Outlet />
      </Content>
    </Layout>
  );
};

export default AuthLayout;
