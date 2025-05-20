import { BarChartOutlined, UserOutlined, AppstoreOutlined, BookOutlined } from '@ant-design/icons';
import { applicationRole } from './role';

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
    role: [applicationRole.ADMIN, applicationRole.LEARNER],
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
    role: [applicationRole.ADMIN, applicationRole.LEARNER],
  },
  {
    key: "courses",
    icon: <BookOutlined />,
    label: "Courses",
    link: "courses",
    role: [applicationRole.LEARNER, applicationRole.ADMIN, applicationRole.MENTOR],
  },
];

