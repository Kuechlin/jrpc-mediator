import {
    Button,
    Card,
    Collapse,
    Group,
    Stack,
    Table,
    Text,
    Title,
} from '@mantine/core';
import { useToggle } from '@mantine/hooks';
import { ChevronDown } from 'tabler-icons-react';
import { RequestSchema } from '../models';

export function RequestView({ request }: { request: RequestSchema }) {
    const [open, toggle] = useToggle();

    return (
        <Card>
            <Card.Section>
                <Group
                    onClick={() => toggle()}
                    align="stretch"
                    position="apart"
                >
                    <Group>
                        <Button px={8} color="green">
                            <Text weight="bold">{request.method}</Text>
                        </Button>
                        <Title order={4}>{request.name}</Title>
                    </Group>
                    <Button color="gray" p={4} variant="subtle">
                        <ChevronDown
                            style={{
                                transform: `rotate(${open ? 180 : 0}deg)`,
                                transition: '0.25s',
                            }}
                        />
                    </Button>
                </Group>
                <Collapse in={open} p="xs">
                    <Card
                        sx={(theme) => ({
                            backgroundColor: theme.colors.dark[5],
                        })}
                    >
                        <Stack>
                            <Title order={3}>Parameters</Title>

                            <Table withBorder withColumnBorders>
                                <thead>
                                    <tr>
                                        <th>Name</th>
                                        <th>Value</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {Object.entries(request.properties).map(
                                        ([key, type]) => (
                                            <tr key={key}>
                                                <td>{key}</td>
                                                <td>{type}</td>
                                            </tr>
                                        )
                                    )}
                                </tbody>
                            </Table>
                        </Stack>
                    </Card>
                </Collapse>
            </Card.Section>
        </Card>
    );
}
