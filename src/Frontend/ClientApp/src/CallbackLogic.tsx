import * as React from "react";

import { UserManager } from 'oidc-client';

const OidcSettings = {
    authority: 'http://localhost:5000',
    client_id: 'js',
    post_logout_redirect_uri: 'http://localhost:5003/index.html', 
    redirect_uri: 'http://localhost:5003/index.html', 
    response_type: 'id_token token', 
    scope: 'openid profile'
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