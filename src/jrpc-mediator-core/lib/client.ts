import axios, { Axios, AxiosError, AxiosInstance } from 'axios';
import { getMethod } from './decorators';
import {
    INotification,
    IRequest,
    IResponse,
    JRpcError,
    JRpcResponse,
} from './types';

export class JRpcClient {
    private url: string;
    private axios: AxiosInstance;
    private currentId: number = 1;

    constructor(url: string) {
        this.url = url;
        this.axios = axios.create();
    }

    public configure = (setupAction: (axios: Axios) => void) => {
        setupAction(this.axios);
    };

    public send = async <TRequest extends IRequest<any>>(
        request: TRequest
    ): Promise<IResponse<TRequest>> => {
        const method = getMethod(request);
        if (!method) throw new Error('Method not found');
        try {
            const res = await this.axios.post<JRpcResponse>(this.url, {
                jsonrpc: '2.0',
                method,
                params: { ...request },
                id: this.currentId++,
            });

            if (res.data.error) {
                throw new JRpcError(res.data.error);
            } else {
                return res.data.result as any;
            }
        } catch (err) {
            if (err instanceof AxiosError && err.response) {
                throw new JRpcError(err.response.data?.error);
            } else {
                throw err;
            }
        }
    };

    public publish = async (request: INotification): Promise<void> => {
        const method = getMethod(request);
        if (!method) throw new Error('Method not found');

        await this.axios.post(this.url, {
            jsonrpc: '2.0',
            method,
            params: { ...request },
        });
    };
}
