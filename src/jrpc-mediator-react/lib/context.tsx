import { JRpcClient } from '@jrpc-mediator/core';
import { createContext, ReactNode, useMemo } from 'react';

export const JRpcContext = createContext<JRpcClient>(null as any);

export function JRpcProvider({
    children,
    url,
}: {
    children: ReactNode;
    url: string;
}) {
    const client = useMemo(() => new JRpcClient(url), []);

    return <JRpcContext.Provider value={client} children={children} />;
}
