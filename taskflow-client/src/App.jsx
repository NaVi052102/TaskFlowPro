import React from 'react'; // Import React library
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom'; // Import router components and hooks
import LoginPage from './pages/LoginPage'; // Import login page component
import RegisterPage from './pages/RegisterPage'; // Import registration page component
import DashboardPage from './pages/DashboardPage'; // Import dashboard page component
import VerifyEmailPage from './pages/VerifyEmailPage'; // Import email verification page component
import ProtectedRoute from './components/ProtectedRoute'; // Import protected route guard component

export default function App() { // Export main App component
    return (
        <BrowserRouter>
            <Routes>
                {/* Default root path redirects to login */}
                <Route path="/" element={<Navigate to="/login" replace />} />
                
                {/* Public Authentication Routes */}
                <Route path="/login" element={<LoginPage />} />
                <Route path="/register" element={<RegisterPage />} />
                
                {/* Email Verification Route */}
                <Route path="/verify-email" element={<VerifyEmailPage />} />
                
                {/* Protected Dashboard Route */}
                <Route 
                    path="/dashboard" 
                    element={
                        <ProtectedRoute>
                            <DashboardPage />
                        </ProtectedRoute>
                    } 
                />
            </Routes>
        </BrowserRouter>
    );
}