import { UserManager } from 'oidc-client';

import OidcSettings from './OidcSettings';

class CallbackLogic extends React.Component {
    public contextTypes : React.ReactPropTypes;

    constructor() {
        super({});

        var um = new UserManager(new OidcSettings());

        um.signinRedirectCallback().then(() => {
            window.location = "index.html";
        }).catch(e => {
            console.error(e);
        });
    }

    public render() {
        return (
            <div className="App">
            </div>
        );
    }
}

export default CallbackLogic;