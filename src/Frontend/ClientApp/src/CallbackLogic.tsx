import * as React from "react";

import { UserManager } from 'oidc-client';

const OidcSettings = {
    authority: 'https://localhost:5000',
    client_id: 'js',
    post_logout_redirect_uri: 'id_token token', 
    redirect_uri: 'https://localhost:5003/callback.html', 
    response_type: 'openid profile api1', 
    scope: 'https://localhost:5003/index.html'
};

class CallbackLogic extends React.Component {
    constructor() {
        super({});

        new UserManager(OidcSettings).signinRedirectCallback().then(() => {
            window.location.href = "index.html";
        }).catch(e => {
            console.error(e);
        });
    }

    public render() {
        return (
            <div className="App" />
        );
    }
}

export default CallbackLogic;