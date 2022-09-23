import axios from 'axios';
import { createJRpcClient } from '../src';

export const {
    JRpcProvider,
    useJRpcClient,
    useJRpcPublish,
    useJRpcQuery,
    useJRpcCommand,
    useJRpcSend,
} = createJRpcClient({
    url: '/execute',
    axiosInstance: axios.create({
        withCredentials: true,
    }),
});
