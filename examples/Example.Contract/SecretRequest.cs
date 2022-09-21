﻿using JRpcMediator;
using MediatR;

namespace Example.Contract;

[JRpcMethod("secret")]
[JRpcAuthorize()]
public class SecretRequest : IRequest<string>
{
    public SecretRequest(string text)
    {
        Text = text;
    }

    public string Text { get; set; }
}