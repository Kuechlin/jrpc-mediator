import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import React from 'react';
import ReactDOM from 'react-dom/client';
import { JRpcProvider } from '../src';
import { App } from './App';

ReactDOM.createRoot(document.getElementById('root') as HTMLElement).render(
    <React.StrictMode>
        <QueryClientProvider client={new QueryClient()}>
            <JRpcProvider url="/execute">
                <App />
            </JRpcProvider>
        </QueryClientProvider>
    </React.StrictMode>
);
