import React, { useEffect, useState } from 'react'; // Import React, state, and effect hooks
import { useSearchParams, useNavigate } from 'react-router-dom'; // Import router hooks for reading URL query params and navigation
import api from '../services/api'; // Import configured Axios API instance
import { CheckCircle2, XCircle, Loader2 } from 'lucide-react'; // Import status icons
import toast from 'react-hot-toast'; // Import hot-toast notifications

export default function VerifyEmailPage() {
    const [searchParams] = useSearchParams(); // Hook to read query parameters from URL
    const navigate = useNavigate(); // Hook to redirect user after verification

    const [status, setStatus] = useState('loading'); // State hook for verification status ('loading', 'success', 'error')
    const [errorMessage, setErrorMessage] = useState(''); // State hook for storing error message

    const token = searchParams.get('token'); // Extract token query parameter from URL
    const email = searchParams.get('email'); // Extract email query parameter from URL

    useEffect(() => {
        const verifyAccount = async () => {
            if (!token || !email) {
                setStatus('error');
                setErrorMessage('Invalid verification link parameters.');
                return;
            }

            try {
                // Send POST or GET request to your backend verification endpoint
                await api.post('/users/verify-email', { email, token });
                setStatus('success');
                toast.success('Email verified successfully!');
            } catch (err) {
                console.error('Verification failed', err);
                setStatus('error');
                setErrorMessage(err.response?.data?.message || 'Email verification failed or link expired.');
            }
        };

        verifyAccount();
    }, [token, email]);

    return (
        <div className="min-h-screen bg-slate-900 text-slate-100 flex items-center justify-center p-6">
            <div className="bg-slate-800 border border-slate-700/60 p-8 rounded-2xl shadow-xl max-w-md w-full text-center">
                {status === 'loading' && (
                    <div className="flex flex-col items-center py-6">
                        <Loader2 className="w-12 h-12 text-indigo-400 animate-spin mb-4" />
                        <h2 className="text-xl font-bold text-white">Verifying your email...</h2>
                        <p className="text-slate-400 text-sm mt-1">Please wait while we confirm your real Gmail account.</p>
                    </div>
                )}

                {status === 'success' && (
                    <div className="flex flex-col items-center py-6">
                        <CheckCircle2 className="w-12 h-12 text-emerald-400 mb-4" />
                        <h2 className="text-xl font-bold text-white">Email Verified!</h2>
                        <p className="text-slate-400 text-sm mt-1 mb-6">Your account has been successfully verified. You can now log in to TaskFlow Pro.</p>
                        <button 
                            onClick={() => navigate('/login')}
                            className="w-full bg-indigo-600 hover:bg-indigo-500 text-white font-medium py-2.5 rounded-xl transition-colors"
                        >
                            Go to Login
                        </button>
                    </div>
                )}

                {status === 'error' && (
                    <div className="flex flex-col items-center py-6">
                        <XCircle className="w-12 h-12 text-red-400 mb-4" />
                        <h2 className="text-xl font-bold text-white">Verification Failed</h2>
                        <p className="text-red-400 text-sm mt-1 mb-6">{errorMessage}</p>
                        <button 
                            onClick={() => navigate('/login')}
                            className="w-full bg-slate-700 hover:bg-slate-600 text-slate-200 font-medium py-2.5 rounded-xl transition-colors"
                        >
                            Return to Login
                        </button>
                    </div>
                )}
            </div>
        </div>
    );
}