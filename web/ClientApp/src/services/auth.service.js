"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var Msal = require("msal");
var AuthService = /** @class */ (function () {
    function AuthService() {
        var _this = this;
        this.login = function () {
            return _this.app.loginPopup(_this.applicationConfig.graphScopes).then((function (idToken) {
                debugger;
                var user = _this.app.getUser();
                if (user) {
                    return user;
                }
                else {
                    return null;
                }
            }), function () {
                debugger;
                return null;
            });
        };
        this.logout = function () {
            _this.app.logout();
        };
        this.getToken = function () {
            return _this.app.acquireTokenSilent(_this.applicationConfig.graphScopes).then(function (accessToken) {
                return accessToken;
            }, function (error) {
                return _this.app
                    .acquireTokenPopup(_this.applicationConfig.graphScopes)
                    .then(function (accessToken) {
                    return accessToken;
                }, function (err) {
                    console.error(err);
                });
            });
        };
        var PROD_REDIRECT_URI = 'https://localhost:44346/';
        var redirectUri = window.location.origin;
        if (window.location.hostname !== '127.0.0.1') {
            redirectUri = PROD_REDIRECT_URI;
        }
        this.applicationConfig = {
            clientID: 'fa6a41cc-4861-4d0e-8c90-08f16e8587c0',
            graphScopes: ['user.read']
        };
        this.app = new Msal.UserAgentApplication(this.applicationConfig.clientID, '', function () {
            // callback for login redirect
        }, {
            redirectUri: redirectUri
        });
    }
    return AuthService;
}());
exports.default = AuthService;
//# sourceMappingURL=auth.service.js.map