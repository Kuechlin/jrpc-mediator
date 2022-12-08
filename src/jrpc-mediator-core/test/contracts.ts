import { IRequest, JRpcMethod } from '../lib';

export class CreateTodoRequest implements IRequest<string> {
    static methodName = 'create-todo';

    response?: string;
    constructor(public title: string, public description: string) {}
}
JRpcMethod(CreateTodoRequest.methodName)(CreateTodoRequest);
