const mapQueryString = (params: Record<string, string | undefined>): string => {
  return Object.entries(params)
    .filter(([_, value]) => value !== undefined && value !== null)
    .map(
      ([key, value]) =>
        `${encodeURIComponent(key)}=${encodeURIComponent(value!)}`,
    )
    .join("&");
};

const oauthConfig = {
  google: {
    baseUrl: import.meta.env.VITE_GOOGLE_AUTH_URL,
    params: {
      client_id: import.meta.env.VITE_GOOGLE_CLIENT_ID,
      redirect_uri: import.meta.env.VITE_GOOGLE_REDIRECT_URI,
      response_type: "code",
      scope: "openid email profile",
    },
  },
  github: {
    baseUrl: import.meta.env.VITE_GITHUB_AUTH_URL,
    params: {
      client_id: import.meta.env.VITE_GITHUB_CLIENT_ID,
      scope: "read:user user:email",
    },
  },
};

export const redirectOAuthHandler = (provider: "google" | "github") => {
  debugger;
  const { baseUrl, params } = oauthConfig[provider];
  debugger;
  const queryString = mapQueryString(params);
  const authUrl = `${baseUrl}?${queryString}`;
  window.location.href = authUrl;
};
