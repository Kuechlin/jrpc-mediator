import {
    useMutation,
    UseMutationOptions,
    useQuery,
    UseQueryOptions,
} from '@tanstack/react-query';
import axios, { Axios } from 'axios';
import { createContext, ReactNode, useCallback, useContext } from 'react';
import { INotification, IRequest, JRpcError, JRpcResponse } from './types';
import { createRequest, getMethod, getQueryKey, IdUtil } from './utils';

export type JRpcContext = {
    axiosInstance: Axios;
    send<TRequest extends IRequest<any>>(
        request: TRequest
    ): Promise<TRequest extends IRequest<infer TResponse> ? TResponse : null>;
    publish(request: INotification): Promise<void>;
};

export type JRpcOptions = {
    url: string;
    axiosInstance?: Axios;
};

export function createJRpcClient({
    url,
    axiosInstance = axios.create(),
}: JRpcOptions) {
    const JRpcContext = createContext<JRpcContext>(null as any);

    const useJRpcClient = () => useContext(JRpcContext);
    const useJRpcSend = () => useContext(JRpcContext).send;
    const useJRpcPublish = () => useContext(JRpcContext).publish;

    function JRpcProvider({ children }: { children: ReactNode }) {
        const send = useCallback<JRpcContext['send']>(async (request) => {
            const method = getMethod(request);
            if (!method) throw new Error('method not found');

            const res = await axiosInstance.post<JRpcResponse>(
                url,
                createRequest(IdUtil.nextId(), method, request)
            );

            if (!res.data) throw new Error('response is null');

            if (res.data.error) throw new JRpcError(res.data.error);

            return res.data.result as any;
        }, []);

        const publish = useCallback<JRpcContext['publish']>(async (request) => {
            const method = getMethod(request);
            if (!method) throw new Error('method not found');

            await axiosInstance.post(
                url,
                createRequest(IdUtil.nextId(), method, request)
            );
        }, []);

        return (
            <JRpcContext.Provider
                value={{
                    axiosInstance,
                    send,
                    publish,
                }}
                children={children}
            />
        );
    }

    function useJRpcQuery<TArgs extends any[], TResponse>(
        queryType: { new (...args: TArgs): IRequest<TResponse> },
        args: TArgs,
        options?: Omit<
            UseQueryOptions<TResponse, Error, TResponse, any[]>,
            'queryKey' | 'queryFn'
        >
    ) {
        const send = useJRpcSend();
        return useQuery(
            getQueryKey(queryType, ...args),
            ({ queryKey: [_method, ...args] }) =>
                send(new queryType(...(args as TArgs))),
            options
        );
    }

    function useJRpcCommand<TRequest extends IRequest<any>, TContext = unknown>(
        commandType: {
            new (): TRequest;
        },
        options?: Omit<
            UseMutationOptions<
                TRequest extends IRequest<infer TResponse> ? TResponse : null,
                Error,
                Omit<TRequest, 'response'>,
                TContext
            >,
            'mutationFn' | 'mutationKey'
        >
    ) {
        const send = useJRpcSend();

        function mutationFn(args: Omit<TRequest, 'response'>) {
            const cmd = new commandType();
            Object.assign(cmd, args);
            return send(cmd);
        }

        return useMutation({
            ...options,
            mutationFn,
            mutationKey: getMethod(commandType),
        });
    }

    return {
        JRpcProvider,
        useJRpcClient,
        useJRpcSend,
        useJRpcPublish,
        useJRpcQuery,
        useJRpcCommand,
    };
}
