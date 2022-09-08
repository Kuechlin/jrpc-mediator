import {
    createRequest,
    getMethod,
    INotification,
    IRequest,
    JRpcError,
    JRpcResponse,
} from './types';
import { useJRpcContext } from './context';

class IdUtil {
    static #current: number = 1;
    static nextId() {
        return IdUtil.#current++;
    }
}

export function useSend() {
    const client = useJRpcContext();

    return async function <TRequest extends IRequest<any>>(
        request: TRequest
    ): Promise<TRequest extends IRequest<infer TResponse> ? TResponse : null> {
        const method = getMethod(request);
        if (!method) throw new Error('method not found');

        const res = await client.post<JRpcResponse>(
            '',
            createRequest(IdUtil.nextId(), method, request)
        );

        if (!res.data) throw new Error('response is null');

        if (res.data.error) throw new JRpcError(res.data.error);

        return res.data.result as any;
    };
}

export function usePublish() {
    const client = useJRpcContext();

    return async function (request: INotification): Promise<void> {
        const method = getMethod(request);
        if (!method) throw new Error('method not found');

        await client.post('', createRequest(IdUtil.nextId(), method, request));
    };
}
