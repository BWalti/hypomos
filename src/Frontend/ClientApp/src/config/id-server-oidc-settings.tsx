export default {
    authority: 'http://localhost:5005/auth',
    client_id: 'js',
    post_logout_redirect_uri: 'http://localhost:5005/index.html', 
    redirect_uri: 'http://localhost:5005/index.html', 
    response_type: 'id_token token', 
    scope: 'openid profile Files.Read.All'
};