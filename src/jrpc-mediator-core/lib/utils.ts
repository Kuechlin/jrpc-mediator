import { getMethod } from "./decorators";
import {
    INotification,
    IRequest,
    JRpcException,
    JRpcRequest,
    Result,
    ResultState,
} from "./types";

const request = (id: number, obj: IRequest<any>): JRpcRequest => {
    const method = getMethod(obj);
    if (!method) throw new Error("Method not found");
    const { response, ...params } = obj;
    return {
        id,
        jsonrpc: "2.0",
        method: getMethod(obj),
        params,
    };
};
const notification = (obj: INotification): JRpcRequest => {
    const method = getMethod(obj);
    if (!method) throw new Error("Method not found");
    return {
        jsonrpc: "2.0",
        method: getMethod(obj),
        params: obj,
    };
};

const success = (value: object): Result => ({
    state: ResultState.Success,
    value,
});

const failure = (e: JRpcException): Result => ({
    state: ResultState.Failure,
    exception: e,
});

export const u = {
    request,
    notification,
    success,
    failure,
};
