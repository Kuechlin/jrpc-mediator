import { IRequest, JRpcRequest } from './types';

export function JRpcMethod(method: string) {
    return Reflect.metadata('method', method);
}

export function getMethod(obj: object) {
    if (!obj.constructor) return null;
    return (
        Reflect.getMetadata('method', obj.constructor) || obj.constructor.name
    );
}
export function getQueryKey<TArgs extends any[], TResponse>(
    queryType: { new (...args: TArgs): IRequest<TResponse> },
    ...args: TArgs
) {
    return [getMethod(queryType), ...args];
}

export class IdUtil {
    static #current: number = 1;
    static nextId() {
        return IdUtil.#current++;
    }
}

export function createRequest(
    id: number,
    method: string,
    params: object
): JRpcRequest {
    return {
        jsonrpc: '2.0',
        method,
        params: { ...params },
        id,
    };
}
