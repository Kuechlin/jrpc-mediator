import {
    UseMutationResult,
    useQueryClient,
    UseQueryResult,
} from '@tanstack/react-query';
import { CSSProperties, ReactNode, useState } from 'react';
import { getQueryKey } from '../src';
import { useJRpcClient, useJRpcCommand, useJRpcQuery } from './client';
import {
    DemoRequest,
    ErrorRequest,
    ListRequest,
    LoginRequest,
    SecretRequest,
} from './contract';

export function renderResult<TData, TError, TArgs = unknown, TCtx = unknown>(
    result:
        | UseMutationResult<TData, TError, TArgs, TCtx>
        | UseQueryResult<TData, TError>,
    render?: (val: TData) => ReactNode,
    renderError?: (err: TError) => ReactNode
) {
    return result.isLoading
        ? 'Loading...'
        : result.isError
        ? renderError
            ? renderError(result.error)
            : String(result.error)
        : result.data
        ? render
            ? render(result.data)
            : String(result.data)
        : 'Send request to see result';
}

const articleStyle: CSSProperties = {
    width: 100 / 5 + '%',
};

export function App() {
    const queryClient = useQueryClient();
    const axiosInstance = useJRpcClient().axiosInstance;

    const [length, setLength] = useState(10);
    const listQuery = useJRpcQuery(ListRequest, [length], {
        cacheTime: Infinity,
        staleTime: Infinity,
    });
    const secretCmd = useJRpcCommand(SecretRequest, {
        onSuccess() {
            queryClient.invalidateQueries(getQueryKey(ListRequest, 10));
        },
    });
    const demoCmd = useJRpcCommand(DemoRequest);
    const errorCmd = useJRpcCommand(ErrorRequest);
    const loginCmd = useJRpcCommand(LoginRequest, {
        onSuccess(data) {
            secretCmd.reset();
            axiosInstance.defaults.headers.post.Authorization = `Bearer ${data}`;
        },
    });

    const sendSecret = () => secretCmd.mutate({ text: 'secret text' });
    const sendDemo = () => demoCmd.mutate({ name: 'max' });
    const sendError = () => errorCmd.mutate({ message: 'some error' });
    const sendLogin = () => loginCmd.mutate({ name: 'admin', pass: 'root' });

    return (
        <div style={{ display: 'flex' }}>
            <article style={articleStyle}>
                <h3>login</h3>
                <button onClick={sendLogin}>Login</button>
                <hr />
                {renderResult(loginCmd, (d) => d.substring(0, 4) + '...')}
            </article>
            <article style={articleStyle}>
                <h3>send demo request</h3>
                <button onClick={sendDemo}>Send Demo</button>
                <hr />
                {renderResult(demoCmd)}
            </article>
            <article style={articleStyle}>
                <h3>send error</h3>
                <button onClick={sendError}>Send Error</button>
                <hr />
                {renderResult(errorCmd, undefined, (err) => (
                    <pre children={JSON.stringify(err, null, 2)} />
                ))}
            </article>
            <article style={articleStyle}>
                <h3>send secret request</h3>
                <button onClick={sendSecret}>Send Secret</button>
                <hr />
                {renderResult(secretCmd, (data) => (
                    <pre children={data} />
                ))}
            </article>
            <article style={articleStyle}>
                <h3>with react-query</h3>

                <input
                    type="number"
                    value={length}
                    onChange={(e) => setLength(parseInt(e.target.value))}
                />
                <hr />
                {renderResult(listQuery, (data) => (
                    <>
                        {data.map((num) => (
                            <div key={num}>{num}</div>
                        ))}
                    </>
                ))}
            </article>
        </div>
    );
}
