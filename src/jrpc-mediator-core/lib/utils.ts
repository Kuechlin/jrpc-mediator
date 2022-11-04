import { JRpcException, JRpcRequest, Result, ResultState } from './types';

const request = (options: Omit<JRpcRequest, 'jsonrpc'>): JRpcRequest => ({
    jsonrpc: '2.0',
    ...options,
});

const success = (value: object): Result => ({
    state: ResultState.Success,
    value,
});

const failure = (e: JRpcException): Result => ({
    state: ResultState.Failure,
    exception: e,
});

export const u = {
    request,
    success,
    failure,
};
