import axios from "axios";

export const axiosClient = axios.create({
  headers: {
    Accept: "application/json",
    "Content-Type": "application/json",
  },
  baseURL: import.meta.env.VITE_BASE_URL_BE,
});

axiosClient.interceptors.request.use(
    (config) => {
      const token = localStorage.getItem("token");
      if (token) {
        config.headers.Authorization = `Bearer ${token}`;
      }
      return config;
    },
    (error) => Promise.reject(error),
);

axiosClient.interceptors.response.use(
    function (response) {
      return response;
    },
    function (error) {
      const res = error.response;
      if (res.status === 401) {
        window.location.href = "/login";
      }
      console.error("error: ", res);
      return Promise.reject(error);
    },
);