import {
    getMethod,
    IRequest,
    JRpcClient,
    JRpcError,
} from '@jrpc-mediator/core';
import {
    useMutation,
    UseMutationOptions,
    useQuery,
    UseQueryOptions,
} from '@tanstack/react-query';
import { createContext, ReactNode, useContext, useMemo } from 'react';
import { getQueryKey } from './utils';

export const createJRpcClient = (url: string) => {
    const JRpcContext = createContext<JRpcClient>(null as any);

    function JRpcProvider({ children }: { children: ReactNode }) {
        const client = useMemo(() => new JRpcClient(url), []);

        return <JRpcContext.Provider value={client} children={children} />;
    }

    function useJRpcClient() {
        return useContext(JRpcContext);
    }
    function useJRpcSend() {
        return useContext(JRpcContext).send;
    }
    function useJRpcPublish() {
        return useContext(JRpcContext).publish;
    }
    function useJRpcBatch() {
        return useContext(JRpcContext).batch;
    }

    function useJRpcQuery<TArgs extends any[], TResponse>(
        requestType: { new (...args: TArgs): IRequest<TResponse> },
        args: TArgs,
        options?: Omit<
            UseQueryOptions<TResponse, Error | JRpcError, TResponse, any[]>,
            'queryKey' | 'queryFn'
        >
    ) {
        const send = useJRpcSend();
        return useQuery(
            getQueryKey(requestType, ...args),
            ({ queryKey: [_method, ...args] }) =>
                send(new requestType(...(args as TArgs))),
            options
        );
    }

    function useJRpcMutation<
        TRequest extends IRequest<any>,
        TContext = unknown
    >(
        requestType: {
            new (...args: any[]): TRequest;
        },
        options?: Omit<
            UseMutationOptions<
                TRequest extends IRequest<infer TResponse> ? TResponse : null,
                Error | JRpcError,
                Omit<TRequest, 'response'>,
                TContext
            >,
            'mutationFn' | 'mutationKey'
        >
    ) {
        const send = useJRpcSend();

        function mutationFn(args: Omit<TRequest, 'response'>) {
            const cmd = new requestType();
            Object.assign(cmd, args);
            return send(cmd);
        }

        return useMutation({
            ...options,
            mutationFn,
            mutationKey: getMethod(requestType),
        });
    }

    return {
        JRpcProvider,
        useJRpcClient,
        useJRpcSend,
        useJRpcPublish,
        useJRpcQuery,
        useJRpcMutation,
        useJRpcBatch,
    };
};
