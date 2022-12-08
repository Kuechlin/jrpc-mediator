import { describe, expect, it } from "vitest";
import { getMethod } from "../lib";
import { CreateTodoRequest } from "./contracts";

describe("jrpc decorators", () => {
    it("should get method name from type", () => {
        const method = getMethod(CreateTodoRequest);

        expect(method).toBe(CreateTodoRequest.methodName);
    });

    it("should get method from object", () => {
        const method = getMethod(new CreateTodoRequest("Test", "test todo"));

        expect(method).toBe(CreateTodoRequest.methodName);
    });
});
