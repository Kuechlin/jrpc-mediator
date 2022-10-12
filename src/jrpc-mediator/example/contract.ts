import { IRequest, JRpcMethod } from '../src';

@JRpcMethod('login')
export class LoginRequest implements IRequest<string> {
    response?: string;
    constructor(public name: string = '', public pass: string = '') {}
}

@JRpcMethod('demo')
export class DemoRequest implements IRequest<string> {
    response?: string;
    constructor(public name: string = '') {}
}

@JRpcMethod('error')
export class ErrorRequest implements IRequest<object> {
    response?: object;
    constructor(public message: string = '') {}
}

@JRpcMethod('list')
export class ListRequest implements IRequest<number[]> {
    response?: number[];
    constructor(public length: number = 0) {}
}

@JRpcMethod('secret')
export class SecretRequest implements IRequest<string> {
    response?: string;
    constructor(public text: string = '') {}
}
