import { IRequest, JRpcMethod } from '../src';

@JRpcMethod('demo')
export class DemoRequest implements IRequest<string> {
    response?: string;
    constructor(public Name: string) {}
}

@JRpcMethod('error')
export class ErrorRequest implements IRequest<object> {
    response?: object;
    constructor(public Message: string) {}
}

@JRpcMethod('list')
export class ListRequest implements IRequest<number[]> {
    response?: number[];
    constructor(public Length: number) {}
}
