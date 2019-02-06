const baseAddress = process.env.REACT_APP_AUTH_HOST 
    ? process.env.REACT_APP_AUTH_HOST 
    : 'http://localhost:5005';

export default {
    authority: baseAddress + '/auth',
    client_id: 'js',
    post_logout_redirect_uri: baseAddress + '/index.html', 
    redirect_uri: baseAddress + '/index.html', 
    response_type: 'id_token token', 
    scope: 'openid profile Files.Read.All'
};