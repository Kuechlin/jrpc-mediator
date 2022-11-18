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
