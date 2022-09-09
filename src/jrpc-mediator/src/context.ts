import axios, { Axios } from 'axios';
import { createContext, createElement, useContext } from 'react';

type JRpcContext = {
    url: string;
    client: Axios;
};

const JRpcContext = createContext<JRpcContext>(null as any);

export function useJRpcContext() {
    return useContext(JRpcContext);
}

export type JRpcProviderProps = {
    url: string;
    client?: Axios;
    children: any;
};

export function JRpcProvider({ url, client, children }: JRpcProviderProps) {
    return createElement(JRpcContext.Provider, {
        value: {
            url,
            client: client ?? axios.create(),
        },
        children,
    });
}
