import { Button, Modal, Stack, TextInput } from '@mantine/core';
import { useForm, zodResolver } from '@mantine/form';
import { useToggle } from '@mantine/hooks';
import { z } from 'zod';

const schema = z.object({
    name: z.string({ required_error: 'Name is required' }),
    description: z.string().optional(),
});
type Values = z.infer<typeof schema>;

export function NewTodoModal({
    loading,
    onSubmit,
}: {
    loading: boolean;
    onSubmit(values: Values): Promise<any>;
}) {
    const [open, toggle] = useToggle();
    const form = useForm({
        validate: zodResolver(schema),
        initialValues: {
            name: '',
            description: '',
        },
    });

    const handleSubmit = async (values: Values) => {
        await onSubmit(values);
        form.reset();
        toggle();
    };

    return (
        <>
            <Button onClick={() => toggle()}>New Todo</Button>
            <Modal
                opened={open}
                title="New Todo"
                onClose={() => {
                    form.reset();
                    toggle();
                }}
            >
                <form onSubmit={form.onSubmit(handleSubmit)}>
                    <Stack>
                        <TextInput
                            label="Name"
                            {...form.getInputProps('name')}
                        />
                        <TextInput
                            label="Description"
                            {...form.getInputProps('description')}
                        />
                        <Button type="submit" loading={loading}>
                            Create Todo
                        </Button>
                    </Stack>
                </form>
            </Modal>
        </>
    );
}
