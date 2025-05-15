import { ConfigProvider } from "antd";
import { CustomTheme } from "./theme/CustomTheme";
import { BrowserRouter } from "react-router-dom";
import AppRoutes from "./routes";

function App() {
   window.addEventListener("storage", () => {
    if (!localStorage.getItem("token")) {
      window.location.href = "/";
    }
  });
  return (
    <>
      <ConfigProvider theme={CustomTheme}>
        <BrowserRouter>
          <AppRoutes />
        </BrowserRouter>
      </ConfigProvider>
    </>
  );
}

export default App;
