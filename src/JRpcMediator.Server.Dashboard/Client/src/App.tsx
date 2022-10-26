import { Title } from '@mantine/core';
import { useEffect, useState } from 'react';
import { JRpcTypes } from './models';

export default function App() {
    const [types, setTypes] = useState<JRpcTypes>({
        enums: {},
        requests: {},
        types: {},
    });
    useEffect(() => {
        // @ts-ignore
        fetch(BASE_URL + '/types')
            .then((res) => {
                if (res.ok) return res.json();
                else throw new Error(res.statusText);
            })
            .then(setTypes);
    }, []);

    return (
        <>
            <Title>JRpcDashboard</Title>
            <pre children={JSON.stringify(types, null, 2)} />
        </>
    );
}
