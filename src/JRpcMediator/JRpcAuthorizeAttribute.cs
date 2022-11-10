using System;

namespace JRpcMediator
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class JRpcAuthorizeAttribute : Attribute
    {
        public string? Policy { get; set; }
        public string? Role { get; set; }
        public string? Scheme { get; set; }
        public bool Challange { get; set; } = false;
    }
}