import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import React from 'react';
import ReactDOM from 'react-dom';
import { App } from './App';
import { JRpcProvider } from './client';

const client = new QueryClient();

ReactDOM.render(
    <React.StrictMode>
        <QueryClientProvider client={client}>
            <JRpcProvider>
                <App />
            </JRpcProvider>
        </QueryClientProvider>
    </React.StrictMode>,
    document.getElementById('root') as HTMLElement
);
