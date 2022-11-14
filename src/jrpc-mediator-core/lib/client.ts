import axios, { Axios, AxiosError, AxiosInstance } from 'axios';
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

    constructor(url: string, client: AxiosInstance = axios.create()) {
        this.url = url;
        this.axios = client;
    }

    public configure = (setupAction: (axios: Axios) => void) => {
        setupAction(this.axios);
    };

    public send = async <TRequest extends IRequest<any>>(
        request: TRequest
    ): Promise<IResponse<TRequest>> => {
        try {
            const res = await this.axios.post<JRpcResponse>(
                this.url,
                u.request(this.currentId++, request)
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
        await this.axios.post(this.url, u.notification(notification));
    };

    public batch = async (
        batch: Record<number, IRequest<any>>
    ): Promise<Record<number, Result>> => {
        const responses = await this.axios.post<JRpcResponse[]>(
            this.url,
            Object.entries(batch).map(([key, value]) =>
                u.request(Number(key), value)
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
