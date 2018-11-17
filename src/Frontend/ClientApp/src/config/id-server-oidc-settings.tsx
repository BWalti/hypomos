export default {
    authority: 'http://localhost:5000',
    client_id: 'js',
    post_logout_redirect_uri: 'http://localhost:5003/index.html', 
    redirect_uri: 'http://localhost:5003/index.html', 
    response_type: 'id_token token', 
    scope: 'openid profile'
};