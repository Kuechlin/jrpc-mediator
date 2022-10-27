import react from '@vitejs/plugin-react';
import { resolve } from 'path';
import { defineConfig } from 'vite';

// https://vitejs.dev/config/
export default defineConfig({
    plugins: [react()],
    build: {
        lib: {
            entry: resolve(__dirname, 'lib', 'index.tsx'),
            name: '@jrpc-mediator/react',
            fileName: 'jrpc-mediator-react',
        },
        rollupOptions: {
            external: [
                'react',
                'react-dom',
                'axios',
                '@tanstack/react-query',
                '@jrpc-mediator/core',
            ],
            output: {
                globals: {
                    react: 'React',
                },
            },
        },
    },
});
