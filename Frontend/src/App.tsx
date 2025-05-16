import { ConfigProvider } from "antd";
import { CustomTheme } from "./theme/CustomTheme";
import { BrowserRouter } from "react-router-dom";
import AppRoutes from "./routes";

function App() {
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
