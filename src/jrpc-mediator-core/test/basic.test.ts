import axios, { AxiosInstance } from 'axios';
import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { IRequest, JRpcClient, JRpcMethod } from '../lib';

@JRpcMethod('create-todo')
class CreateTodoRequest implements IRequest<string> {
    response?: string;
    constructor(public title: string, public description: string) {}
}

describe('jrpc client', () => {
    let axiosInstance: AxiosInstance;
    let client: JRpcClient;
    beforeEach(() => {
        axiosInstance = axios;
        client = new JRpcClient('/jrpc', axios);
    });
    afterEach(() => {
        vi.clearAllMocks();
    });

    it('schould post', async () => {
        await client.send(new CreateTodoRequest('test', 'hallo welt'));

        expect(axiosInstance.post).toBeCalledWith('/jrpc', {
            jsonrpc: '2.0',
            method: 'create-todo',
            params: {
                title: 'test',
                description: 'hallo welt',
            },
            id: 1,
        });
    });
});

vi.mock('axios', () => ({
    default: {
        post: vi.fn(() => ({
            data: {
                jsonrpc: '2.0',
                result: {},
                id: 1,
            },
        })),
    },
}));
