import { getMethod, IRequest, JRpcError } from '@jrpc-mediator/core';
import {
    useMutation,
    UseMutationOptions,
    useQuery,
    UseQueryOptions,
} from '@tanstack/react-query';
import { useContext } from 'react';
import { JRpcContext } from './context';
import { getQueryKey } from './utils';

export function useJRpcClient() {
    return useContext(JRpcContext);
}
export function useJRpcSend() {
    return useContext(JRpcContext).send;
}
export function useJRpcPublish() {
    return useContext(JRpcContext).publish;
}

export function useJRpcQuery<TArgs extends any[], TResponse>(
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

export function useJRpcMutation<
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
