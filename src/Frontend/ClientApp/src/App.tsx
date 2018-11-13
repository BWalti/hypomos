import * as React from 'react';
import Authenticate from 'react-openidconnect';

import "./App.css";

import logo from "./logo.svg";

const OidcSettings = {
    authority: 'http://localhost:5000',
    client_id: 'js',
    post_logout_redirect_uri: 'http://localhost:5003/index.html', 
    redirect_uri: 'http://localhost:5003/index.html', 
    response_type: 'id_token token', 
    scope: 'openid profile'
};

const Authenticated = (user: any) => { 
    if (user !== undefined && user.user !== undefined) {
        return (<div>Hello dear {user.user} you are authenticated.</div>);
    }

    return null;
};

class App extends React.Component {
    public state: any;

    constructor() {
        super({});

        this.state = {
            apiCallFailed: false,
            loginFailed: false,
            user: null,
            userInfo: null
        };

        this.userLoaded = this.userLoaded.bind(this); 
        this.userUnLoaded = this.userUnLoaded.bind(this);
    }

    public userLoaded(user: any) {
        if (user) {
            debugger;
        }
    } 
  
    public userUnLoaded() {
        this.setState({ "user": undefined });
    } 
 
    public NotAuthenticated() {
        return <div>You are not authenticated, please click here to authenticate.</div>;
    }

    public render() {
        return (
            <div className="App">
                <header className="App-header">
                    <img src={logo} className="App-logo" alt="logo"/>
                    <h1 className="App-title">React app with MSAL.js</h1>
                </header>

                <Authenticate 
                    OidcSettings={OidcSettings} 
                    userLoaded={this.userLoaded} 
                    userunLoaded={this.userUnLoaded} 
                    renderNotAuthenticated={this.NotAuthenticated}>

                    <Authenticated user={this.state.user} />
                </Authenticate>
            </div>
        );
    }
}

export default App;