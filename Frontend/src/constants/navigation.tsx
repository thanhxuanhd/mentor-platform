import {
  BarChartOutlined,
  UserOutlined,
  AppstoreOutlined,
  BookOutlined,
  ProfileOutlined,
  ScheduleOutlined,
  HddOutlined,
  MessageOutlined,
} from "@ant-design/icons";
import { applicationRole } from "./role";

interface MenuItemProps {
  key: string;
  icon: React.ReactNode;
  label: string;
  link: string;
  role: string[];
  isMentorApprovedRequired?: boolean;
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
    isMentorApprovedRequired: true,
  },
  {
    key: "messages",
    icon: <MessageOutlined />,
    label: "Messages",
    link: "messages",
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
    role: [applicationRole.ADMIN],
  },
  {
    key: "categories",
    icon: <AppstoreOutlined />,
    label: "Categories",
    link: "categories",
    role: [
      applicationRole.ADMIN,
      applicationRole.LEARNER,
      applicationRole.MENTOR,
    ],
    isMentorApprovedRequired: true,
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
    isMentorApprovedRequired: true,
  },
  {
    key: "profile",
    icon: <ProfileOutlined />,
    label: "Profile",
    link: "profile",
    role: [applicationRole.LEARNER, applicationRole.MENTOR],
  },
  {
    key: "applications",
    icon: <ProfileOutlined />,
    label: "Applications",
    link: "applications",
    role: [applicationRole.ADMIN],
  },
  {
    key: "my-applications",
    icon: <HddOutlined />,
    label: "My Applications",
    link: "my-applications",
    role: [applicationRole.MENTOR],
  },
  {
    key: "availability",
    icon: <ScheduleOutlined />,
    label: "Availability",
    link: "availability",
    role: [applicationRole.MENTOR],
  },
];
