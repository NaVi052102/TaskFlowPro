import axios from 'axios';

const api = axios.create({
    baseURL: 'http://localhost:5028/api', // Changed from 5001 to 5028 to match your .NET backend
});

// Automatically attach JWT token to outgoing requests if available
api.interceptors.request.use((config) => {
    const token = localStorage.getItem('jwt_token');
    if (token) {
        config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
}, (error) => {
    return Promise.reject(error);
});

// Intercept incoming responses to catch expired or invalid tokens (401 Unauthorized)
api.interceptors.response.use(
    (response) => {
        return response;
    },
    (error) => {
        if (error.response && error.response.status === 401) {
            localStorage.removeItem('jwt_token');
            window.location.href = '/login';
        }
        return Promise.reject(error);
    }
);

export default api;