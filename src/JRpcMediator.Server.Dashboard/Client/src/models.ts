export interface JRpcTypes {
    types: Record<string, TypeModel>;
    enums: Record<string, EnumModel>;
    requests: Record<string, RequestModel>;
}

export interface EnumModel {
    name: string;
    values: Record<string, number>;
}

export interface TypeModel {
    name: string;
    properties: Record<string, string>;
}

export interface RequestModel extends TypeModel {
    method: string;
    returnType: string;
}
