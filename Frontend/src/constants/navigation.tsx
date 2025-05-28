import {
  BarChartOutlined,
  UserOutlined,
  AppstoreOutlined,
  BookOutlined,
  ProfileOutlined,
  ScheduleOutlined,
} from "@ant-design/icons";
import { applicationRole } from "./role";

interface MenuItemProps {
  key: string;
  icon: React.ReactNode;
  label: string;
  link: string;
  role: string[];
}

export const menuItems: MenuItemProps[] = [
  {
    key: "dashboard",
    icon: <BarChartOutlined />,
    label: "Dashboard",
    link: "dashboard",
    role: [
      applicationRole.ADMIN,
      applicationRole.LEARNER,
      applicationRole.MENTOR,
    ],
  },
  {
    key: "users",
    icon: <UserOutlined />,
    label: "Users",
    link: "users",
    role: [applicationRole.ADMIN, applicationRole.LEARNER],
  },
  {
    key: "categories",
    icon: <AppstoreOutlined />,
    label: "Categories",
    link: "categories",
    role: [applicationRole.ADMIN, applicationRole.LEARNER, applicationRole.MENTOR],
  },
  {
    key: "courses",
    icon: <BookOutlined />,
    label: "Courses",
    link: "courses",
    role: [
      applicationRole.LEARNER,
      applicationRole.ADMIN,
      applicationRole.MENTOR,
    ],
  },
  {
    key: "profile",
    icon: <ProfileOutlined />,
    label: "Profile",
    link: "profile",
    role: [applicationRole.LEARNER, applicationRole.MENTOR],
  },
  {
    key: "availability",
    icon: <ScheduleOutlined />,
    label: "Availability",
    link: "availability",
    role: [applicationRole.MENTOR],
  }
];
