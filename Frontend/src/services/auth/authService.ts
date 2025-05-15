import {axiosClient} from '../apiClient';
import type { LoginReq, SignUpReq, ResetPasswordReq } from '../../models';

interface AuthResponse {
  value: string;
  refreshToken: string;
}

export const authService = {
  login: async (loginData: LoginReq): Promise<AuthResponse> => {
    try {
      const response = await axiosClient.post<AuthResponse>('/Auth/sign-in', loginData);
      localStorage.setItem('token', response.data.value);
      localStorage.setItem('refreshToken', response.data.refreshToken);
      
      return response.data;
    } catch (error) {
      console.error('Login failed:', error);
      throw error;
    }
  },
  signUp: async (signUpData: SignUpReq): Promise<AuthResponse> => {
    try {
      const response = await axiosClient.post<AuthResponse>('/Auth/sign-up', signUpData);
      
      localStorage.setItem('token', response.data.value);
      localStorage.setItem('refreshToken', response.data.refreshToken);
      
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
  logout: () => {
    localStorage.removeItem('token');
    localStorage.removeItem('refreshToken');
    window.location.href = '/login';
  },
   
  isAuthenticated: (): boolean => {
    return !!localStorage.getItem('token');
  },

  getToken: (): string | null => {
    return localStorage.getItem('token');
  }
};

export default authService;