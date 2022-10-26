import {
    Button,
    Card,
    Center,
    Divider,
    Group,
    PasswordInput,
    Stack,
    Text,
    TextInput,
    Title,
} from '@mantine/core';
import { useForm, zodResolver } from '@mantine/form';
import { z } from 'zod';

const schema = z.object({
    username: z.string({ required_error: 'Username is required' }),
    password: z.string({ required_error: 'Password is required' }),
});

export function LoginForm({
    loading,
    onSubmit,
}: {
    loading: boolean;
    onSubmit(values: z.infer<typeof schema>): void;
}) {
    const form = useForm({
        validate: zodResolver(schema),
        initialValues: {
            username: '',
            password: '',
        },
    });

    return (
        <Center m={24}>
            <Card style={{ width: '300px' }}>
                <form onSubmit={form.onSubmit(onSubmit)}>
                    <Stack>
                        <Title order={2}>Login</Title>
                        <Card.Section>
                            <Divider />
                        </Card.Section>
                        <TextInput
                            label="Username"
                            {...form.getInputProps('username')}
                        />
                        <PasswordInput
                            label="Password"
                            {...form.getInputProps('password')}
                        />
                        <Button
                            children="Login"
                            type="submit"
                            loading={loading}
                        />
                        <Group position="apart">
                            <Text size="xs" color="dimmed">
                                Reader: user 1234
                            </Text>
                            <Text size="xs" color="dimmed">
                                Writer: admin root
                            </Text>
                        </Group>
                    </Stack>
                </form>
            </Card>
        </Center>
    );
}
