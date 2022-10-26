import { Center, Loader } from '@mantine/core';
import { UseQueryResult } from '@tanstack/react-query';
import { ReactNode } from 'react';

export function renderResult<TData, TError>(
    result: UseQueryResult<TData, TError>,
    render: (val: TData) => ReactNode
) {
    switch (result.status) {
        case 'loading':
            return (
                <Center>
                    <Loader variant="bars" />
                </Center>
            );
        case 'error':
            return (
                <div style={{ color: 'red' }}>
                    <h2>Error</h2>
                    <pre children={JSON.stringify(result.error, null, 2)} />
                </div>
            );
        default:
            return render(result.data);
    }
}
