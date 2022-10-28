import { getMethod, IRequest } from '@jrpc-mediator/core';

export function getQueryKey<TArgs extends any[], TResponse>(
    queryType: { new (...args: TArgs): IRequest<TResponse> },
    ...args: TArgs
) {
    return [getMethod(queryType), ...args];
}
