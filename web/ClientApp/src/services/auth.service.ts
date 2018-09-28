import * as Msal from 'msal';

export default class AuthService {
    private app: Msal.UserAgentApplication;
    private applicationConfig: any;

    constructor() {
        const prodRedirectUri = 'https://localhost:44346/';

        let redirectUri = window.location.origin;
        if (window.location.hostname !== '127.0.0.1') {
            redirectUri = prodRedirectUri;
        }

        this.applicationConfig = {
            clientID: 'fa6a41cc-4861-4d0e-8c90-08f16e8587c0',
            graphScopes: ['user.read']
        };

        this.app = new Msal.UserAgentApplication(
            this.applicationConfig.clientID,
            '',
            () => {
                // callback for login redirect
            },
            {
                redirectUri
            }
        );
    }

    public login = () => {
        return this.app.loginPopup(this.applicationConfig.graphScopes).then(
            ((idToken: string) => {
                return this.app.getUser();
            }),
            () => {
                return null;
            }
        );
    };

    public logout = () => {
        this.app.logout();
    };

    public getToken = () => {
        return this.app.acquireTokenSilent(this.applicationConfig.graphScopes).then(
            accessToken => {
                return accessToken;
            },
            error => {
                return this.app
                    .acquireTokenPopup(this.applicationConfig.graphScopes)
                    .then(
                        accessToken => {
                            return accessToken;
                        },
                        err => {
                            console.error(err);
                        }
                    );
            }
        );
    };
}
