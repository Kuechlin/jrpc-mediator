/////////////////////////////////////
// automatically generated content //
/////////////////////////////////////
import { IRequest, JRpcMethod } from '@jrpc-mediator/core';

export type TodoModel = {
	id: number;
	name: string;
	description: string;
	state: TodoState;
}

export type Unit = {
}

export enum TodoState {
	New = 1,
	InProgress = 2,
	Done = 3,
}

@JRpcMethod('create/todo')
export class CreateTodoRequest implements IRequest<TodoModel> {
	response?: TodoModel;
	constructor(
		public name: string,
		public description: string
	) {}
}

@JRpcMethod('delete/todo')
export class DeleteTodoRequest implements IRequest<Unit> {
	response?: Unit;
	constructor(
		public id: number
	) {}
}

@JRpcMethod('error')
export class ErrorRequest implements IRequest<string> {
	response?: string;
	constructor(
		public message: string
	) {}
}

@JRpcMethod('get/todo')
export class GetTodoRequest implements IRequest<TodoModel> {
	response?: TodoModel;
	constructor(
		public id: number
	) {}
}

@JRpcMethod('login')
export class LoginRequest implements IRequest<string> {
	response?: string;
	constructor(
		public username: string,
		public password: string
	) {}
}

@JRpcMethod('query/todo')
export class QueryTodosRequest implements IRequest<TodoModel[]> {
	response?: TodoModel[];
	constructor(
		public skip: number,
		public take: number
	) {}
}

@JRpcMethod('update/todo')
export class UpdateTodoRequest implements IRequest<TodoModel> {
	response?: TodoModel;
	constructor(
		public model: TodoModel
	) {}
}