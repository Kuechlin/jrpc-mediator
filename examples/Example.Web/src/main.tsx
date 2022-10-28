import { JRpcProvider } from '@jrpc-mediator/react';
import { MantineProvider } from '@mantine/core';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import React from 'react';
import ReactDOM from 'react-dom/client';
import 'reflect-metadata';
import App from './components/App';
import Authorize from './components/Authorize';

var client = new QueryClient();

ReactDOM.createRoot(document.getElementById('root') as HTMLElement).render(
    <React.StrictMode>
        <MantineProvider
            withGlobalStyles
            withNormalizeCSS
            theme={{ colorScheme: 'dark' }}
        >
            <QueryClientProvider client={client}>
                <JRpcProvider url="/execute">
                    <Authorize>
                        <App />
                    </Authorize>
                </JRpcProvider>
            </QueryClientProvider>
        </MantineProvider>
    </React.StrictMode>
);
