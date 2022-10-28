import { ReactNode, useState } from 'react';
import { LoginRequest } from '../contracts';
import { useJRpcClient, useJRpcMutation } from '../jrpc';
import { LoginForm } from './LoginForm';

export default function Authorize({ children }: { children: ReactNode }) {
    const [authorized, setAuthorized] = useState(false);
    const client = useJRpcClient();

    const login = useJRpcMutation(LoginRequest, {
        onSuccess(token) {
            client.configure((axios) => {
                axios.defaults.headers.post[
                    'Authorization'
                ] = `Bearer ${token}`;
            });
            setAuthorized(true);
        },
    });

    if (authorized) return <>{children}</>;

    return <LoginForm loading={login.isLoading} onSubmit={login.mutate} />;
}
