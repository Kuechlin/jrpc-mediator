import React from 'react';
import ReactDOM from 'react-dom/client';
import { JRpcProvider } from './client';
import { App } from './App';
import { QueryClient, QueryClientProvider } from 'react-query';

const client = new QueryClient();

ReactDOM.createRoot(document.getElementById('root') as HTMLElement).render(
    <React.StrictMode>
        <QueryClientProvider client={client}>
            <JRpcProvider>
                <App />
            </JRpcProvider>
        </QueryClientProvider>
    </React.StrictMode>
);
