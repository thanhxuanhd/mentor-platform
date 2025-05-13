import { Layout, Menu } from "antd";
import {
  UserOutlined,
  AppstoreOutlined,
  BarChartOutlined,
  BookOutlined,
} from "@ant-design/icons";

import { Outlet, useNavigate } from "react-router-dom";

const { Header, Sider, Content, Footer } = Layout;

const siderStyle: React.CSSProperties = {
  overflow: "auto",
  height: "100vh",
  position: "sticky",
  insetInlineStart: 0,
  top: 0,
  bottom: 0,
  scrollbarWidth: "thin",
  scrollbarGutter: "stable",
};

const MainLayout = () => {
  const navigate = useNavigate();

  const menuItems = [
    {
      key: "dashboard",
      icon: <BarChartOutlined />,
      label: "Dashboard",
      onClick: () => navigate("/"),
    },
    {
      key: "users",
      icon: <UserOutlined />,
      label: "Users",
      onClick: () => navigate("/users"),
    },
    {
      key: "categories",
      icon: <AppstoreOutlined />,
      label: "Categories",
      onClick: () => navigate("/categories"),
    },
    {
      key: "courses",
      icon: <BookOutlined />,
      label: "Courses",
      onClick: () => navigate("/courses"),
    },
  ];

  return (
    <Layout>
      <Sider
        breakpoint="lg"
        collapsedWidth="0"
        onBreakpoint={(broken) => {
          console.log(broken);
        }}
        onCollapse={(collapsed, type) => {
          console.log(collapsed, type);
        }}
        style={siderStyle}
        className="border-r border-gray-700"
      >
        <div className="demo-logo-vertical" />
        <div className="text-orange-500 text-center py-4 text-xl font-bold">
          Mentor Connect
        </div>
        <Menu
          theme="dark"
          mode="inline"
          defaultSelectedKeys={["dashboard"]}
          items={menuItems}
        />
      </Sider>

      <Layout>
        <Header className="border-b border-gray-700"></Header>

        <Content className="p-4 bg-gray-900">
          <Outlet />
        </Content>

        <Footer className="text-center border-t border-gray-700">
          ©{new Date().getFullYear()} Mentor Connect. All rights reserved.
        </Footer>
      </Layout>
    </Layout>
  );
};

export default MainLayout;
