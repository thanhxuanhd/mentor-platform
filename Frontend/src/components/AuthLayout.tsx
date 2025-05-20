import { Layout } from "antd";
import { Outlet } from "react-router-dom";
import LogoIcon from "./shared/LogoIcon";

const { Header, Content } = Layout;

const AuthLayout = () => {
  return (
    <Layout>
      <Header className="flex align-center border-b border-gray-700 w-full top-0">
        <div className="flex items-center justify-start gap-2">
          <LogoIcon style={{ fontSize: 32 }} />
          <p className="text-center text-md font-bold text-red-600 sm:text-xl">
            Rookies - Group 4
          </p>
        </div>
      </Header>

      <Content className="min-h-screen h-content overflow-y-auto bg-gray-900">
        <Outlet />
      </Content>
    </Layout>
  );
};

export default AuthLayout;
