import axios from 'axios';

// 1. Create a pre-configured Axios instance pointing to your .NET 9 backend API URL
const api = axios.create({
    baseURL: 'http://localhost:5001/api',
    headers: {
        'Content-Type': 'application/json'
    }
});

// 2. Request interceptor to automatically attach the JWT token from storage if available
api.interceptors.request.use(
    (config) => {
        // Retrieve the stored JWT token
        const token = localStorage.getItem('jwt_token');
        
        // If a token exists, add it to the Authorization header
        if (token) {
            config.headers.Authorization = 'Bearer ' + token;
        }
        return config;
    },
    (error) => {
        // Handle request errors
        return Promise.reject(error);
    }
);

// 3. Export the configured API client so it can be imported across your React components
export default api;