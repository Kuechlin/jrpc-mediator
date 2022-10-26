import { createJRpcClient } from '@jrpc-mediator/react';

export const {
    JRpcProvider,
    useJRpcClient,
    useJRpcCommand,
    useJRpcPublish,
    useJRpcQuery,
    useJRpcSend,
} = createJRpcClient('/execute');
