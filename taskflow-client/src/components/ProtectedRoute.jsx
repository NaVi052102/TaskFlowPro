import React from 'react';
import { Navigate } from 'react-router-dom';

export default function ProtectedRoute({ children }) {
    // Check if JWT token exists in browser storage
    const token = localStorage.getItem('jwt_token');

    // If no token is found, redirect user to the login page
    if (!token) {
        return <Navigate to="/login" replace />;
    }

    // If token exists, render the protected child component (Dashboard)
    return children;
}