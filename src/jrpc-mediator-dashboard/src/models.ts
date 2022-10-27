export interface JRpcTypes {
    types: Record<string, TypeSchema>;
    enums: Record<string, EnumSchema>;
    requests: Record<string, RequestSchema>;
}

export interface EnumSchema {
    name: string;
    values: Record<string, number>;
}

export interface TypeSchema {
    name: string;
    properties: Record<string, string>;
}

export interface RequestSchema extends TypeSchema {
    method: string;
    returnType: string;
}
