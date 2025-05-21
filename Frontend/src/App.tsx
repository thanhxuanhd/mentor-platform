import { ConfigProvider } from "antd";
import { CustomTheme } from "./theme/CustomTheme";
import { BrowserRouter } from "react-router-dom";
import AppRoutes from "./routes";
import { App as AntdApp } from "antd";
import { AuthProvider } from "./contexts/AuthContext";

function App() {
  window.addEventListener("storage", () => {
    if (!localStorage.getItem("token")) {
      window.location.href = "/";
    }
  });
  return (
    <ConfigProvider theme={CustomTheme}>
      <AntdApp>
        <BrowserRouter>
          <AuthProvider>
            <AppRoutes />
          </AuthProvider>
        </BrowserRouter>
      </AntdApp>
    </ConfigProvider>
  );
}

export default App;
