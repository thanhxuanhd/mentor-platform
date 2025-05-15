import axios from 'axios';

let isRefreshing = false;
let failedQueue: {
  resolve: (token: string) => void;
  reject: (error: unknown) => void;
}[] = [];

const processQueue = (error: unknown, token: string | null = null) => {
  failedQueue.forEach((prom) => {
    if (error) {
      prom.reject(error);
    } else {
      prom.resolve(token as string);
    }
  });

  failedQueue = [];
};

const api = axios.create({
  baseURL: 'https://localhost:5000/api',
  headers: {
    'Content-Type': 'application/json',
  },
});

// Request interceptor for adding the auth token
api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('token');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

// Response interceptor for handling errors
api.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config;

    // If the error is not 401 or the request has already been retried, reject
    if (error.response?.status !== 401 || originalRequest._retry) {
      return Promise.reject(error);
    }

    // Mark the request as retried to prevent infinite loops
    originalRequest._retry = true;

    if (isRefreshing) {
      // If a refresh is already in progress, queue this request
      try {
        const token = await new Promise<string>((resolve, reject) => {
          failedQueue.push({ resolve, reject });
        });
        originalRequest.headers['Authorization'] = `Bearer ${token}`;
        return axios(originalRequest);
      } catch (err) {
        return Promise.reject(err);
      }
    }

    isRefreshing = true;

    try {
      // Try to refresh the token
      const refreshToken = localStorage.getItem('refreshToken');

      if (!refreshToken) {
        // No refresh token available, redirect to login
        localStorage.removeItem('token');
        window.location.href = '/login';
        return Promise.reject(error);
      }

      // Call the refresh token endpoint
      // Update the refreshToken function to use the new RefreshTokenRequest DTO

      const refreshAccessToken = async (): Promise<string | null> => {
        try {
          if (!refreshToken) return null;

          // Temporarily remove the Authorization header to avoid using the expired token
          delete api.defaults.headers.common['Authorization'];

          const response = await api.post('/auth/refresh-token', {
            refreshToken,
          });
          const { token: newToken, refreshToken: newRefreshToken } =
            response.data;

          localStorage.setItem('token', newToken);
          localStorage.setItem('refreshToken', newRefreshToken);
          api.defaults.headers.common['Authorization'] = `Bearer ${newToken}`;
          // setToken(newToken);  // Assuming these functions are defined elsewhere and handle token storage
          // setRefreshToken(newRefreshToken);
          return newToken;
        } catch (error) {
          console.error('Failed to refresh token:', error);
          // If refresh token is expired or invalid, log the user out
          localStorage.removeItem('token');
          localStorage.removeItem('refreshToken');
          window.location.href = '/login';
          return null;
        }
      };

      const accessToken = await refreshAccessToken();

      if (!accessToken) {
        return Promise.reject(error);
      }

      // Update localStorage and axios headers
      // localStorage.setItem("token", accessToken)
      api.defaults.headers.common['Authorization'] = `Bearer ${accessToken}`;
      originalRequest.headers['Authorization'] = `Bearer ${accessToken}`;

      // Process any queued requests
      processQueue(null, accessToken);

      // Retry the original request
      return axios(originalRequest);
    } catch (refreshError) {
      // If refresh fails, clear tokens and redirect to login
      processQueue(refreshError, null);
      localStorage.removeItem('token');
      localStorage.removeItem('refreshToken');
      window.location.href = '/login';
      return Promise.reject(refreshError);
    } finally {
      isRefreshing = false;
    }
  }
);

export default api;
