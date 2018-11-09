"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var GraphService = /** @class */ (function () {
    function GraphService() {
        var _this = this;
        this.getUserInfo = function (token) {
            var headers = new Headers({ Authorization: "Bearer " + token });
            var options = {
                headers: headers
            };
            return fetch(_this.graphUrl + "/me", options)
                .then(function (response) { return response.json(); })
                .catch(function (response) {
                throw new Error(response.text());
            });
        };
        this.graphUrl = 'https://graph.microsoft.com/v1.0/';
    }
    return GraphService;
}());
exports.default = GraphService;
//# sourceMappingURL=graph.service.js.map