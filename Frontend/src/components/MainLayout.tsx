import { App, Button, Layout, Menu, Tooltip } from "antd";
import { SettingFilled, LogoutOutlined, BellOutlined } from "@ant-design/icons";

import { Outlet, useLocation, useNavigate } from "react-router-dom";
import { useAuth } from "../hooks";
import { menuItems } from "../constants/navigation";
import type { MenuItemType } from "antd/es/menu/interface";
import { applicationRole } from "../constants/role";
import UserProfileDropdown from "./ProfileCard";
import { useCallback, useEffect, useState } from "react";
import type { UserDetail } from "../types/UserTypes";
import { userService } from "../services/user/userService";
import type { NotificationProps } from "../types/Notification";
import Loading from "./Loading";
import { useUser } from "../hooks/useUser";

const { Header, Sider, Content, Footer } = Layout;

const MainLayout = () => {
  const [loading, setLoading] = useState<boolean>()
  const navigate = useNavigate();
  const location = useLocation();
  const [userDetails, setUserDetails] = useState<UserDetail | undefined>()
  const { user, removeToken, isMentorApproved } = useAuth();
  const [notify, setNotify] = useState<NotificationProps | null>(null);
  const { notification } = App.useApp();
  const { isProfileUpdated, setProfileUpdated } = useUser();

  useEffect(() => {
    if (notify) {
      notification[notify.type]({
        message: notify.message,
        description: notify.description,
        placement: "topRight",
        showProgress: true,
        duration: 3,
        pauseOnHover: true,
      });
      setNotify(null);
    }
  }, [notify, notification]);

  const fetchUserDetails = useCallback(async () => {
    try {
      setLoading(true);
      if (user?.id) {
        const response = await userService.getUserDetail(user.id);
        setUserDetails(response);
        setProfileUpdated(false); // Reset the flag after successful fetch
      }
    } catch (error: any) {
      setNotify({
        type: "error",
        message: "Failed to load user details",
        description: error?.response?.data?.error || "Error loading user details",
      });
    } finally {
      setLoading(false);
    }
  }, [user, setProfileUpdated]);

  useEffect(() => {
    if (user) {
      fetchUserDetails();
    }
  }, [user, isProfileUpdated, fetchUserDetails]);

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

  if (loading) {
    return <Loading />
  }

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
          <Tooltip title="Settings">
            <Button icon={<SettingFilled />} />
          </Tooltip>

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
        <Header className="flex justify-end border-b border-gray-700 top-0">
          {user &&
            <div className="flex items-center gap-3">
              <Button type="text" icon={<BellOutlined />} className="text-white hover:bg-slate-700 relative">
                <div className="absolute -top-1 -right-1 w-2 h-2 bg-red-500 rounded-full"></div>
              </Button>

              <UserProfileDropdown user={user} userDetail={userDetails} />
            </div>
          }
        </Header>

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
