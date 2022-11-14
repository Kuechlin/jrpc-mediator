/// <reference types="vitest" />

import { resolve } from 'path';
import { visualizer } from 'rollup-plugin-visualizer';
import { defineConfig } from 'vite';

// https://vitejs.dev/config/
export default defineConfig({
    plugins: [visualizer()],
    build: {
        lib: {
            entry: resolve(__dirname, 'lib/index.ts'),
            name: '@jrpc-mediator/core',
            fileName: (format) => `jrpc-mediator-core.${format}.js`,
        },
        rollupOptions: {
            external: ['axios', 'reflect-metadata'],
            output: {
                globals: {
                    axios: 'axios',
                },
            },
        },
    },
});
