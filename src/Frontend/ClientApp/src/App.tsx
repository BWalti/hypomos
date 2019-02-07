import { Fabric } from 'office-ui-fabric-react/lib/Fabric'
import React from 'react';
// import Content from './components/Content'
import Footer from './components/Footer'
import NavBar from './components/NavBar'
import SidebarMenu from './components/SidebarMenu'

import './styles/App.css';

// import OidcSettings from './config/id-server-oidc-settings';
// import OidcSettings from './config/ms-oidc-settings';

import Authenticate from 'react-openidconnect';
import logo from "./logo.svg";

const Authenticated = (props: any) => {
    if (props.user && props.user.profile && props.user.profile.name) {
        return (<div>Hello {props.user.profile.name}</div>);
    }

    return null;
};

const NotAuthenticated = () => {
    return <div>You are not authenticated, please click here to authenticate.</div>;
};

class App extends React.Component {
    public state: any;

    constructor(props: any) {
        super(props);

        this.state = {
            apiCallFailed: false,
            loginFailed: false,
            user: null,
            userInfo: null
        };

        this.userLoaded = this.userLoaded.bind(this);
        this.userUnLoaded = this.userUnLoaded.bind(this);
    }

    public getOidcSettings() {
        const baseAddress = location.protocol + '//' + location.host;
        return {
            authority: baseAddress + '/auth',
            client_id: 'js',
            post_logout_redirect_uri: baseAddress + '/index.html', 
            redirect_uri: baseAddress + '/index.html', 
            response_type: 'id_token token', 
            scope: 'openid profile Files.Read.All'
        };
    }

    public userLoaded(user: any) {
        debugger;
        if (user) {
            this.setState({ user });
        }
    }

    public userUnLoaded() {
        this.setState({ user: undefined });
    }

    public render() {
        return (
            <Fabric className="App">
                <div className="header">
                    <NavBar />
                </div>
                <div className="body">
                    <div className="content">
                        <div className="App">
                            <header className="App-header">
                                <img src={logo} className="App-logo" alt="logo" />
                                <h1 className="App-title">React app with MSAL.js</h1>
                            </header>

                            <Authenticate
                                OidcSettings={this.getOidcSettings()}
                                userLoaded={this.userLoaded}
                                userunLoaded={this.userUnLoaded}
                                renderNotAuthenticated={NotAuthenticated}>

                                <Authenticated user={this.state.user} />
                            </Authenticate>
                        </div>
                    </div>
                    <div className="sidebar">
                        <SidebarMenu />
                    </div>
                </div>
                <div className="footer">
                    <Footer />
                </div>
            </Fabric>
        );
    }
}

export default App;