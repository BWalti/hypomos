export default class OidcSettings {
    public authority: 'https://localhost:5000';
    public client_id: 'js';
    public redirect_uri: 'https://localhost:5003/callback.html';
    public response_type: 'id_token token';
    public scope: 'openid profile api1';
    public post_logout_redirect_uri: 'https://localhost:5003/index.html';
}