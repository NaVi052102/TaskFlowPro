import React from 'react';
import ReactDOM from 'react-dom/client';
import App from './App.jsx';
import './index.css';
import { Toaster } from 'react-hot-toast'; // Import Toaster component

ReactDOM.createRoot(document.getElementById('root')).render(
  <React.StrictMode>
    <App />
    {/* Global toast notification container with dark theme styling */}
    <Toaster 
      position="bottom-right" 
      toastOptions={{
        style: {
          background: '#1e293b', // slate-800
          color: '#f8fafc', // slate-50
          border: '1px solid #334155', // slate-700
        },
      }}
    />
  </React.StrictMode>,
);