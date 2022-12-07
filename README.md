# JRpc Mediator

Automatic [JSON-RPC](https://en.wikipedia.org/wiki/JSON-RPC) Endpoint for MediatR Request and Notifications

## Packages

-   **JRpcMediator:** Type Definitions

-   **JRpcMediator.Client:** HttpClient to send Request

-   **JRpcMediator.Server:** Server with JSON-RPC endpoint

-   **jrpc-mediator:** Typescript Client to use with react

## Examples

Server Launch Profiles:

-   Console (no auth)
-   IIS (win auth)

## Usage

### Tools
Install all tools present in package
```
dotnet tool restore
```
#### Generate Typescript models
```
dotnet jrpc generate [inputDll] [outputDir] [-l | to switch to camelCase instead of PascalCase]
```

### jrpc-mediator (React)

Use the built in JRPCProvider component to wrap your App. It also needs to be inside a tanstack/query QueryClientProvider
```tsx
// main.tsx
return(
    <QueryClientProvider {...props}>
        <JRPCProvider>
            <App />
        </JRPCProvider>
    </QueryClientProvider>
)
...
```

### 
## Development

Install Tools Localy

    dotnet tool restore

## Todo

-   [x] Support useQuery "select" option
-   [ ] empty args for queries and commands with no attributes
