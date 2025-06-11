import { Button, Layout, Menu } from "antd";
import { LogoutOutlined } from "@ant-design/icons";

import { Outlet, useLocation, useNavigate } from "react-router-dom";
import { useAuth } from "../hooks";
import { menuItems } from "../constants/navigation";
import type { MenuItemType } from "antd/es/menu/interface";
import { applicationRole } from "../constants/role";

const { Header, Sider, Content, Footer } = Layout;

const MainLayout = () => {
  const navigate = useNavigate();
  const location = useLocation();
  const { user, removeToken, isMentorApproved } = useAuth();

  const roleMenuItems: MenuItemType[] = menuItems
    .filter((item) => item.role.includes(user?.role || "unauthorized"))
    .filter((item) => {
      if (
        user?.role === applicationRole.MENTOR &&
        !isMentorApproved &&
        item.isMentorApprovedRequired
      ) {
        return false;
      }
      return true;
    })
    .map((item) => ({
      key: item.key,
      icon: item.icon,
      label: item.label,
      onClick: () => navigate(item.link),
    }));

  const defaultSelectedKeys =
    location.pathname === "/"
      ? ["dashboard"]
      : [location.pathname.substring(1)];

  return (
    <Layout>
      <Sider
        breakpoint="lg"
        collapsedWidth="0"
        style={{
          position: "sticky",
        }}
        className="border-r border-gray-700 h-screen top-0"
      >
        <div>
          <div className="text-orange-500 text-center h-16 py-4 text-xl font-bold border-b border-gray-700">
            Mentor Connect
          </div>
          <Menu
            theme="dark"
            mode="inline"
            items={roleMenuItems}
            selectedKeys={defaultSelectedKeys}
          />
        </div>

        <div className="flex justify-evenly border-t border-gray-700 py-4">
          <Button
            title="Logout"
            icon={<LogoutOutlined />}
            onClick={() => {
              removeToken();
              navigate("/login");
            }}
          >
            Logout
          </Button>
        </div>
      </Sider>

      <Layout>
        <Header className="border-b border-gray-700 top-0"></Header>

        <Content className="flex-1 overflow-y-auto p-6 bg-gray-900">
          <Outlet />
        </Content>

        <Footer className="text-center border-t border-gray-700 h-16">
          ©{new Date().getFullYear()} Mentor Connect. All rights reserved.
        </Footer>
      </Layout>
    </Layout>
  );
};

export default MainLayout;
