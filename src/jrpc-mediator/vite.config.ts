import { resolve } from 'path';
import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';
import { visualizer } from 'rollup-plugin-visualizer';

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
            external: ['react', 'axios', 'reflect-metadata'],
        },
    },
});
