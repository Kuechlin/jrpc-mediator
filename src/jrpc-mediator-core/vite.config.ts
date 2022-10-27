import { resolve } from 'path';
import { defineConfig } from 'vite';

// https://vitejs.dev/config/
export default defineConfig({
    build: {
        lib: {
            entry: resolve(__dirname, 'lib/index.ts'),
            name: '@jrpc-mediator/core',
            fileName: 'jrpc-mediator-core',
        },
        rollupOptions: {
            external: ['axios'],
            output: {
                globals: {
                    axios: 'axios',
                },
            },
        },
    },
});
