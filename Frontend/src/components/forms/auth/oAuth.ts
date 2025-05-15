const mapQueryString = (params: Record<string, string | undefined>): string => {
  return Object.entries(params)
    .filter(([_, value]) => value !== undefined && value !== null)
    .map(
      ([key, value]) =>
        `${encodeURIComponent(key)}=${encodeURIComponent(value!)}`
    )
    .join('&');
};

const oauthConfig = {
  google: {
    baseUrl: 'https://accounts.google.com/o/oauth2/v2/auth',
    params: {
      client_id: process.env.GOOGLE_CLIENT_ID || '',
      redirect_uri: process.env.GOOGLE_REDIRECT_URI,
      response_type: 'code',
      scope: 'openid email profile',
    },
  },
  github: {
    baseUrl: 'https://github.com/login/oauth/authorize',
    params: {
      client_id: process.env.GITHUB_CLIENT_ID || '',
      scope: 'read:user user:email',
    },
  },
};

export const redirectOAuthHandler = (provider: 'google' | 'github') => {
  return () => {
    const { baseUrl, params } = oauthConfig[provider];
    const queryString = mapQueryString(params);
    const authUrl = `${baseUrl}?${queryString}`;
    window.location.href = authUrl;
  };
};
