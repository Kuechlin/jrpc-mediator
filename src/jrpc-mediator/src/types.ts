import 'reflect-metadata';

export interface IRequest<TResponse extends any> {
    response?: TResponse;
}

export interface INotification {}

export type JRpcRequest = {
    jsonrpc: '2.0';
    method: string;
    params: object;
    id?: number;
};

export type JRpcResponse = {
    jsonrpc: '2.0';
    result?: object;
    error?: JRpcException;
    id: number;
};

export type JRpcException = {
    type: string;
    msg: string;
    inner?: JRpcError;
};

export class JRpcError extends Error {
    public rpcError: JRpcException;
    constructor(error: JRpcException) {
        super(error.msg);
        this.rpcError = error;
    }
}
