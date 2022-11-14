using JRpcMediator;
using MediatR;
using System.Collections.Generic;

namespace Example.Contract
{
    [JRpcMethod("result")]
    public class ResultRequest : IRequest<Result<Dictionary<string, string>>>
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public bool ShouldThrowError { get; set; }

        public ResultRequest(string name, string value, bool shouldThrowError = false)
        {
            Name = name;
            Value = value;
            ShouldThrowError = shouldThrowError;
        }
    }
}