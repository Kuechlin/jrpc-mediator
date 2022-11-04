export interface IRequest<TResponse extends any = any> {
    response?: TResponse;
}

export type IResponse<TRequest extends IRequest<any>> =
    TRequest extends IRequest<infer TResponse> ? TResponse : null;

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

export enum ResultState {
    Failure = 0,
    Success = 1,
}

export type Result<T = any> =
    | {
          state: ResultState.Success;
          value: T;
      }
    | {
          state: ResultState.Failure;
          exception: JRpcException;
      };
