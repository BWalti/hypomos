﻿export default class GraphService {
    private graphUrl: string;

    constructor() {
        this.graphUrl = 'https://graph.microsoft.com/v1.0/';
    }

    public getUserInfo = (token: any) => {
        const headers = new Headers({ Authorization: `Bearer ${token}` });
        const options = {
            headers
        };
        return fetch(`${this.graphUrl}/me`, options)
            .then(response => response.json())
            .catch(response => {
                throw new Error(response.text());
            });
    };
}