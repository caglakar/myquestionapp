export const server =
  process.env.REACT_APP_ENV === 'production'
    ? 'https://qanda58-backend.azurewebsites.net/api/questions'
    : process.env.REACT_APP_ENV === 'staging'
    ? 'https://qanda58stage.azurewebsites.net/api/questions'
    : 'http://localhost:37892';

export const webAPIUrl = `${server}/api`;

export const authSettings = {
  domain: 'devcgkar.eu.auth0.com',
  client_id: 'uKvErxsDdYrgEiNY9SQnetvJnuFSkxxO',
  redirect_uri: decodeURIComponent(window.location.origin + '/signin-callback'),
  scope: 'openid profile QandAAPI email',
  audience: 'https://qanda',
};
