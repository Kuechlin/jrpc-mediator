import react from '@vitejs/plugin-react';
import { resolve } from 'path';
import { defineConfig } from 'vite';

// https://vitejs.dev/config/
export default defineConfig({
    plugins: [react()],
    base: '/base/',
    build: {
        outDir: resolve(
            __dirname,
            '..',
            'JRpcMediator.Server.Dashboard',
            'wwwroot'
        ),
        emptyOutDir: true,
    },
    server: {
        proxy: {
            '/base/types': {
                target: 'http://localhost:5000',
                changeOrigin: true,
                rewrite: (path) => path.replace('/base', '/execute'),
            },
        },
    },
});
