import { createJRpcClient } from '@jrpc-mediator/react';

export const {
    JRpcProvider,
    useJRpcClient,
    useJRpcMutation,
    useJRpcPublish,
    useJRpcQuery,
    useJRpcSend,
} = createJRpcClient('/execute');
