import { ConfigProvider } from "antd";
import { CustomTheme } from "./theme/CustomTheme";
import { BrowserRouter } from "react-router-dom";
import AppRoutes from "./routes";
import { App as AntdApp } from "antd";

function App() {
  window.addEventListener("storage", () => {
    if (!localStorage.getItem("token")) {
      window.location.href = "/";
    }
  });
  return (
    <>
      <ConfigProvider theme={CustomTheme}>
        <AntdApp>
          <BrowserRouter>
            <AppRoutes />
          </BrowserRouter>
        </AntdApp>
      </ConfigProvider>
    </>
  );
}

export default App;
