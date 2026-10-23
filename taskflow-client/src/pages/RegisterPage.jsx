import React, { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import api from '../services/api';
import toast from 'react-hot-toast';
import { MailCheck, Loader2 } from 'lucide-react';

export default function RegisterPage() {
    const [username, setUsername] = useState('');
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [loading, setLoading] = useState(false);
    const [isRegistered, setIsRegistered] = useState(false); // Track registration success state

    const handleRegister = async (e) => {
        e.preventDefault();
        setLoading(true);

        try {
            // Call your backend registration endpoint
            await api.post('/users/register', { username, email, password });
            
            // Set registered state to true instead of redirecting to login
            setIsRegistered(true);
            toast.success('Registration successful! Please check your email.');
        } catch (err) {
            console.error('Registration failed', err);
            toast.error(err.response?.data?.message || 'Failed to register account.');
        } finally {
            setLoading(false);
        }
    };

    // If registration succeeded, show the "Check your email" confirmation screen
    if (isRegistered) {
        return (
            <div className="min-h-screen bg-slate-900 text-slate-100 flex items-center justify-center p-6">
                <div className="bg-slate-800 border border-slate-700/60 p-8 rounded-2xl shadow-xl max-w-md w-full text-center">
                    <div className="flex flex-col items-center py-4">
                        <MailCheck className="w-16 h-16 text-emerald-400 mb-4 animate-bounce" />
                        <h2 className="text-2xl font-bold text-white mb-2">Check Your Inbox!</h2>
                        <p className="text-slate-300 text-sm mb-6">
                            We've sent a verification link to <span className="font-semibold text-indigo-400">{email}</span>. Please click the link inside to verify your account before logging in.
                        </p>
                        <Link 
                            to="/login"
                            className="w-full bg-slate-700 hover:bg-slate-600 text-slate-200 font-medium py-2.5 rounded-xl transition-colors block text-center"
                        >
                            Return to Login
                        </Link>
                    </div>
                </div>
            </div>
        );
    }

    // Otherwise, render your normal registration form
    return (
        <div className="min-h-screen bg-slate-900 text-slate-100 flex items-center justify-center p-6">
            <div className="bg-slate-800 border border-slate-700/60 p-8 rounded-2xl shadow-xl max-w-md w-full">
                <h2 className="text-2xl font-bold text-white mb-6 text-center">Create TaskFlow Pro Account</h2>
                
                <form onSubmit={handleRegister} className="space-y-4">
                    <div>
                        <label className="block text-xs font-medium text-slate-400 mb-1">Username</label>
                        <input 
                            type="text" 
                            value={username} 
                            onChange={(e) => setUsername(e.target.value)} 
                            required
                            className="w-full bg-slate-900 border border-slate-700 rounded-xl px-4 py-2.5 text-white focus:outline-none focus:border-indigo-500"
                            placeholder="Enter username"
                        />
                    </div>

                    <div>
                        <label className="block text-xs font-medium text-slate-400 mb-1">Gmail Address</label>
                        <input 
                            type="email" 
                            value={email} 
                            onChange={(e) => setEmail(e.target.value)} 
                            required
                            className="w-full bg-slate-900 border border-slate-700 rounded-xl px-4 py-2.5 text-white focus:outline-none focus:border-indigo-500"
                            placeholder="name@gmail.com"
                        />
                    </div>

                    <div>
                        <label className="block text-xs font-medium text-slate-400 mb-1">Password</label>
                        <input 
                            type="password" 
                            value={password} 
                            onChange={(e) => setPassword(e.target.value)} 
                            required
                            className="w-full bg-slate-900 border border-slate-700 rounded-xl px-4 py-2.5 text-white focus:outline-none focus:border-indigo-500"
                            placeholder="••••••••"
                        />
                    </div>

                    <button 
                        type="submit" 
                        disabled={loading}
                        className="w-full bg-indigo-600 hover:bg-indigo-500 text-white font-medium py-2.5 rounded-xl transition-colors flex items-center justify-center mt-2"
                    >
                        {loading ? <Loader2 className="w-5 h-5 animate-spin" /> : 'Register Account'}
                    </button>
                </form>

                <p className="text-center text-sm text-slate-400 mt-6">
                    Already have an account? <Link to="/login" className="text-indigo-400 hover:underline">Log in</Link>
                </p>
            </div>
        </div>
    );
}