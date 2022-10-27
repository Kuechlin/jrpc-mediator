import { getQueryKey } from '@jrpc-mediator/react';
import {
    Card,
    Center,
    Checkbox,
    Group,
    Stack,
    Text,
    Title,
} from '@mantine/core';
import { useQueryClient } from '@tanstack/react-query';
import { CreateTodoRequest, QueryTodosRequest, TodoState } from '../contracts';
import { useJRpcMutation, useJRpcQuery } from '../jrpc';
import { NewTodoModal } from './NewTodoModal';
import { renderResult } from './renderResult';

export default function App() {
    const queryClient = useQueryClient();
    const query = useJRpcQuery(QueryTodosRequest, [0, 25]);

    const createTodo = useJRpcMutation(CreateTodoRequest, {
        onSuccess() {
            queryClient.invalidateQueries(
                getQueryKey(QueryTodosRequest, 0, 25)
            );
        },
    });

    return (
        <Center>
            <Stack>
                <Title>JRpcMediator Todo Example</Title>
                <NewTodoModal
                    loading={createTodo.isLoading}
                    onSubmit={createTodo.mutateAsync}
                />
                {renderResult(query, (val) => (
                    <>
                        {val.map((todo) => (
                            <Card>
                                <Group align="baseline">
                                    <Checkbox
                                        checked={todo.state === TodoState.Done}
                                        indeterminate={
                                            todo.state === TodoState.InProgress
                                        }
                                    />
                                    <Title order={3}>{todo.name}</Title>
                                    <Text>{todo.description}</Text>
                                </Group>
                            </Card>
                        ))}
                    </>
                ))}
            </Stack>
        </Center>
    );
}
