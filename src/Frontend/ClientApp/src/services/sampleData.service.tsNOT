export default class SampleData {
    private url: string;

    constructor() {
        this.url = 'https://localhost:44346/api/SampleData';
    }

    public getWeatherForecasts = (token: string, startIndex: number) => {
        const headers = new Headers({ Authorization: `Bearer ${token}` });
        const options = {
            headers
        };
        return fetch(`${this.url}/WeatherForecasts?startDateIndex=${startIndex}`, options)
            .then(response => response.json())
            .catch(response => {
                throw new Error(response.text());
            });
    };
}