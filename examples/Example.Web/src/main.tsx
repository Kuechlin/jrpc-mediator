import { MantineProvider } from '@mantine/core';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import React from 'react';
import ReactDOM from 'react-dom/client';
import 'reflect-metadata';
import App from './components/App';
import Authorize from './components/Authorize';
import { JRpcProvider } from './jrpc';

var client = new QueryClient();

ReactDOM.createRoot(document.getElementById('root') as HTMLElement).render(
    <React.StrictMode>
        <MantineProvider
            withGlobalStyles
            withNormalizeCSS
            theme={{ colorScheme: 'dark' }}
        >
            <QueryClientProvider client={client}>
                <JRpcProvider>
                    <Authorize>
                        <App />
                    </Authorize>
                </JRpcProvider>
            </QueryClientProvider>
        </MantineProvider>
    </React.StrictMode>
);
