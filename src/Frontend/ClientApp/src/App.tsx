import { Fabric } from 'office-ui-fabric-react/lib/Fabric'
import React from 'react';
// import Content from './components/Content'
import Footer from './components/Footer'
// import NavBar from './components/NavBar'
import SidebarMenu from './components/SidebarMenu'

import './App.css';
// import OidcSettings from './config/oidc-settings';
// import logo from "./logo.svg";
// import Authenticate from 'react-openidconnect';

// const Authenticated = (props: any) => {
//     if (props.user && props.user.profile && props.user.profile.name) {
//         return (<div>Hello {props.user.profile.name}</div>);
//     }

//     return null;
// };

// const NotAuthenticated = () => {
//     return <div>You are not authenticated, please click here to authenticate.</div>;
// };

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

        // this.userLoaded = this.userLoaded.bind(this); 
        // this.userUnLoaded = this.userUnLoaded.bind(this);
    }

    public userLoaded(user: any) {
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
                    <span>NavBar</span>
                </div>
                <div className="body">
                    <div className="content">
                        <span>Content</span>
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

    // <div className="App">
    //     <header className="App-header">
    //         <img src={logo} className="App-logo" alt="logo"/>
    //         <h1 className="App-title">React app with MSAL.js</h1>
    //     </header>

    //     <Authenticate 
    // OidcSettings={OidcSettings} 
    // userLoaded={this.userLoaded} 
    // userunLoaded={this.userUnLoaded} 
    // renderNotAuthenticated={NotAuthenticated}>

    //     <Authenticated user={this.state.user} />
    //     </Authenticate>
    // </div>
}

export default App;