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
    message: string;
    inner?: JRpcException;
};

export class JRpcError extends Error {
    public isJRpcError = true;
    public data: JRpcException;
    constructor(error: JRpcException) {
        super(error.message);
        this.data = error;
    }
}
