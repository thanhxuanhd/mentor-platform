import { useEffect } from 'react';
import { useNavigate, useSearchParams } from 'react-router-dom';
import authService from '../../services/auth/authService';
import { useLocation } from 'react-router';

export default function OAuthCallback() {
  const [searchParams] = useSearchParams();
  const navigate = useNavigate();
  const { pathname } = useLocation();
  const pathParts = pathname.split('/');
  const provider = pathParts[pathParts.length - 1];

  useEffect(() => {
    const code = searchParams.get('code');

    if (!code) {
      console.error('Missing authorization code');
      return;
    }

    const sendCodeToBackend = async () => {
      try {
        console.log(provider);
        await authService.loginWithOAuth(code, provider);
        navigate('/');
      } catch (error) {
        console.error('OAuth callback failed', error);
        navigate('/login');
      }
    };

    sendCodeToBackend();
  }, []);

  return <p>Logging in via OAuth...</p>;
}
