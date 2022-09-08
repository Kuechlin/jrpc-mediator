import React, { useState } from 'react';
import { useSend } from '../src';
import { DemoRequest, ErrorRequest } from './contract';
import { Query } from './Query';

export function App() {
    const send = useSend();
    const [results, setResults] = useState<Record<string, string>>({});
    const setResult = (key: string, promise: Promise<any>) => {
        const set = (val: any) =>
            setResults((last) => ({
                ...last,
                [key]: JSON.stringify(val, null, 2),
            }));
        promise.then(set).catch(set);
    };

    const sendDemo = () => setResult('demo', send(new DemoRequest('max')));

    const sendError = () =>
        setResult('error', send(new ErrorRequest('some error')));

    return (
        <div style={{ display: 'flex' }}>
            <article style={{ flexGrow: 1 }}>
                <h3>send demo request</h3>
                <button onClick={sendDemo}>Send Demo</button>
                <hr />
                <pre>{results.demo || 'Send request to see result'}</pre>
            </article>
            <article style={{ flexGrow: 1 }}>
                <h3>send error</h3>
                <button onClick={sendError}>Send Error</button>
                <hr />
                <pre>{results.error || 'Send request to see result'}</pre>
            </article>
            <article style={{ flexGrow: 1 }}>
                <h3>with react-query</h3>

                <Query />
            </article>
        </div>
    );
}
