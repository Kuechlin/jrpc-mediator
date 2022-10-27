import { AppShell, Header, Stack, Title } from '@mantine/core';
import axios from 'axios';
import { useEffect, useState } from 'react';
import { JRpcTypes } from '../models';
import { RequestView } from './RequestView';

export default function App() {
    const [types, setTypes] = useState<JRpcTypes>({
        enums: {},
        requests: {},
        types: {},
    });
    useEffect(() => {
        axios
            .get<JRpcTypes>(BASE_URL + '/types')
            .then((res) => setTypes(res.data));
    }, []);

    return (
        <AppShell
            header={
                <Header
                    height={48}
                    px={16}
                    sx={(theme) => ({
                        backgroundColor: theme.colors.dark[6],
                    })}
                >
                    <Title>JRpcDashboard</Title>
                </Header>
            }
        >
            <Stack>
                {Object.entries(types.requests).map(([key, request]) => (
                    <RequestView key={key} request={request} />
                ))}
            </Stack>
        </AppShell>
    );
}
