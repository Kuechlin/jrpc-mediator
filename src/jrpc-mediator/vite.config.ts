import react from '@vitejs/plugin-react';
import { resolve } from 'path';
import { visualizer } from 'rollup-plugin-visualizer';
import { defineConfig } from 'vite';

// https://vitejs.dev/config/
export default defineConfig({
    plugins: [react(), visualizer()],
    build: {
        lib: {
            entry: resolve(__dirname, 'src', 'index.ts'),
            name: 'jrpc-mediator',
            fileName: 'jrpc-mediator',
        },
        rollupOptions: {
            external: [
                'react',
                'react-dom',
                'axios',
                'reflect-metadata',
                '@tanstack/react-query',
            ],
        },
    },
});
