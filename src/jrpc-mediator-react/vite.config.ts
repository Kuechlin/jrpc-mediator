import react from '@vitejs/plugin-react';
import { resolve } from 'path';
import { visualizer } from 'rollup-plugin-visualizer';
import { defineConfig } from 'vite';

// https://vitejs.dev/config/
export default defineConfig({
    plugins: [react(), visualizer()],
    build: {
        lib: {
            entry: resolve(__dirname, 'lib', 'index.ts'),
            name: '@jrpc-mediator/react',
            fileName: (format) => `jrpc-mediator-react.${format}.js`,
        },
        rollupOptions: {
            external: [
                'react',
                'react-dom',
                'react/jsx-runtime',
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
