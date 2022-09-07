import { useQuery } from '@tanstack/react-query';
import React, { useState } from 'react';
import { useSend } from '../src';
import { ListRequest } from './contract';

export function Query() {
    const send = useSend();
    const [length, setLength] = useState(10);
    const query = useQuery(['query', length], ({ queryKey }) =>
        send(new ListRequest(queryKey[1] as number))
    );

    return (
        <>
            <input
                type="number"
                value={length}
                onChange={(e) => setLength(parseInt(e.target.value))}
            />
            <hr />
            {query.isLoading
                ? 'Loading...'
                : query.isError
                ? 'Error!'
                : query.data
                ? query.data.map((num) => <div key={num}>{num}</div>)
                : null}
        </>
    );
}
