/////////////////////////////////////
// automatically generated content //
/////////////////////////////////////
import { IRequest, JRpcMethod } from '@jrpc-mediator/core';

export interface IEntity {
	id: number
}
export interface TodoModel extends IEntity {
	name: string;
	description: string;
	state: TodoState;
}

export interface Unit {
}

export interface Result {
	state: ResultState;
	value: Object;
	exception: Exception;
}

export interface Object {
}

export interface Exception {
	helpLink: string;
	source: string;
	hResult: number;
}

export enum TodoState {
	New = 1,
	InProgress = 2,
	Done = 3,
}

export enum ResultState {
	Failure = 0,
	Success = 1,
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

@JRpcMethod('result')
export class ResultRequest implements IRequest<Result> {
	response?: Result;
	constructor(
		public name: string,
		public value: string,
		public shouldThrowError: boolean
	) {}
}

@JRpcMethod('update/todo')
export class UpdateTodoRequest implements IRequest<TodoModel> {
	response?: TodoModel;
	constructor(
		public model: TodoModel
	) {}
}