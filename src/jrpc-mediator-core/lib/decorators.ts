import 'reflect-metadata';

export function JRpcMethod(method: string) {
    return Reflect.metadata('method', method);
}

export function getMethod(obj: object) {
    if (typeof obj === 'function') {
        return Reflect.getMetadata('method', obj);
    } else if (!obj.constructor) {
        return null;
    } else {
        return (
            Reflect.getMetadata('method', obj.constructor) ||
            obj.constructor.name
        );
    }
}
