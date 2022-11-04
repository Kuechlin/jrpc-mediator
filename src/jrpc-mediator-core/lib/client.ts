import axios, { Axios, AxiosError, AxiosInstance } from 'axios';
import { getMethod } from './decorators';
import {
    INotification,
    IRequest,
    IResponse,
    JRpcError,
    JRpcResponse,
    Result,
} from './types';
import { u } from './utils';

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
            const res = await this.axios.post<JRpcResponse>(
                this.url,
                u.request({
                    method,
                    params: { ...request },
                    id: this.currentId++,
                })
            );

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

    public publish = async (notification: INotification): Promise<void> => {
        const method = getMethod(notification);
        if (!method) throw new Error('Method not found');

        await this.axios.post(
            this.url,
            u.request({
                method,
                params: { ...notification },
            })
        );
    };

    public batch = async (
        batch: Record<number, IRequest<any>>
    ): Promise<Record<number, Result>> => {
        const responses = await this.axios.post<JRpcResponse[]>(
            this.url,
            Object.entries(batch).map(([key, value]) =>
                u.request({
                    id: Number(key),
                    method: getMethod(value),
                    params: { ...value },
                })
            )
        );

        const results: Record<number, Result> = {};

        for (const response of responses.data) {
            if (!!response.error) {
                results[response.id] = u.failure(response.error);
            } else if (!!response.result) {
                results[response.id] = u.failure({
                    type: 'InvalidOperationException',
                    message: 'Invalid Response',
                });
            } else {
                results[response.id] = u.success(response.result);
            }
        }

        return results;
    };
}
