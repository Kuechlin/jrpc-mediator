import react from '@vitejs/plugin-react';
import { defineConfig } from 'vite';

// https://vitejs.dev/config/
export default defineConfig({
    plugins: [react()],
    server: {
        proxy: {
            '/execute': {
                target: 'http://localhost:5000',
                changeOrigin: true,
            },
        },
    },
});
