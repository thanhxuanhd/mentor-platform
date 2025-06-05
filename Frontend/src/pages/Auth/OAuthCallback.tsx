import { useEffect } from "react";
import { useNavigate, useSearchParams } from "react-router-dom";
import authService from "../../services/auth/authService";
import { useLocation } from "react-router";
import { useAuth } from "../../hooks";
import { userStatus } from "../../constants/userStatus";

export default function OAuthCallback() {
  const [searchParams] = useSearchParams();
  const navigate = useNavigate();
  const { pathname } = useLocation();
  const { setToken } = useAuth();
  const pathParts = pathname.split("/");
  const provider = pathParts[pathParts.length - 1];

  useEffect(() => {
    const code = searchParams.get("code");

    if (!code) {
      console.error("Missing authorization code");
      navigate("/login");
      return;
    }

    const sendCodeToBackend = async () => {
      try {
        const response = await authService.loginWithOAuth(code, provider);
        // setToken(response.token);
        // navigate("/");
        switch (response.userStatus) {
          case userStatus.ACTIVE:
            setToken(response.token);
            navigate("/");
            break;
          default:
            navigate("/profile-setup", { state: { ...response } });
            break;
        }
      } catch (error) {
        console.error("OAuth callback failed", error);
        navigate("/login");
      }
    };

    sendCodeToBackend();
  }, []);

  return <p>Logging in via OAuth...</p>;
}
