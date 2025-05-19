import { axiosClient } from '../apiClient';
import type { LoginReq, SignUpReq, ResetPasswordReq } from '../../models';

interface AuthResponse {
  value: string;
  refreshToken: string;
}

export const authService = {
  login: async (loginData: LoginReq): Promise<AuthResponse> => {
    try {
      const response = await axiosClient.post<AuthResponse>(
        '/Auth/sign-in',
        loginData
      );
      return response.data;
    } catch (error) {
      console.error('Login failed:', error);
      throw error;
    }
  },
  
  signUp: async (signUpData: SignUpReq): Promise<AuthResponse> => {
    try {
      const response = await axiosClient.post<AuthResponse>(
        '/Auth/sign-up',
        signUpData
      );
      return response.data;
    } catch (error) {
      console.error('Sign up failed:', error);
      throw error;
    }
  },
  resetPassword: async (data: ResetPasswordReq): Promise<void> => {
    try {
      await axiosClient.post('/Auth/reset-password', data);
    } catch (error) {
      console.error('Reset password failed:', error);
      throw error;
    }
  },

  loginWithOAuth: async (
    token: string,
    provider: string
  ): Promise<AuthResponse> => {
    try {
      const response = await axiosClient.post(`/Auth/${provider}`, {
        token: token,
      });
      return response.data;
    } catch (error) {
      console.error('Login failed:', error);
      throw error;
    }
  },
};

export default authService;