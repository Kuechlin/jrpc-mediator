import axios, { Axios } from 'axios';
import { createContext, createElement, useContext } from 'react';

const JRpcContext = createContext(new Axios());

export function useJRpcContext() {
    return useContext(JRpcContext);
}

export type JRpcProviderProps = {
    url: string;
    children: any;
};

export function JRpcProvider({ url, children }: JRpcProviderProps) {
    return createElement(JRpcContext.Provider, {
        value: axios.create({
            baseURL: url,
        }),
        children,
    });
}
