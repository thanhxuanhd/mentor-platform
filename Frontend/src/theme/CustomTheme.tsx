import { theme } from "antd";

export const CustomTheme = {
  algorithm: theme.darkAlgorithm,
  token: {
    colorPrimary: "#f97316",
    fontSize: 16,
    fontSizeLG: 15,
  },
  components: {
    Segmented: {
      itemSelectedColor: "#fff",
      itemSelectedBg: "#f97316",
      trackPadding: 4,
    },
    Table: {
      colorBgContainer: "#1f2937",
      colorTextHeading: "#fff",
      colorText: "#fff",
      colorBgElevated: "#374151",
    },
    Modal: {
      colorTextHeading: "#fff",
      colorText: "#fff",
      colorBgElevated: "#1f2937",
    },
  },
};
