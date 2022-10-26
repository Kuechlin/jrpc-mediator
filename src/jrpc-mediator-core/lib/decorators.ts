import 'reflect-metadata';

export function JRpcMethod(method: string) {
    return Reflect.metadata('method', method);
}

export function getMethod(obj: object) {
    if (!obj.constructor) return null;
    return (
        Reflect.getMetadata('method', obj.constructor) || obj.constructor.name
    );
}
