using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Tracing;
using static JRpcMediator.Utils.JRpcUtils;

namespace JRpcMediator
{
    public enum ResultState : byte
    {
        Failure,
        Success
    }

    public interface IResult
    {
        ResultState State { get; set; }
        object? Value { get; set; }
        Exception? Exception { get; set; }
        bool IsSuccess { get; }
        bool IsFailure { get; }
    }

    public static class ResultFactory
    {
        public static IResult Create(Type type, Exception exception)
        {
            if (IsResult(type))
            {
                return (IResult)Activator.CreateInstance(type, exception);
            }
            else
            {
                return (IResult)Activator.CreateInstance(typeof(Result<>).MakeGenericType(type), exception);
            }
        }
        public static IResult Create(Type type, object value)
        {
            if (IsResult(type))
            {
                return (IResult)Activator.CreateInstance(type, value);
            }
            else
            {
                return (IResult)Activator.CreateInstance(typeof(Result<>).MakeGenericType(type), value);
            }
        }
        public static T Create<T>(Exception e)
        {
            return (T)Activator.CreateInstance(typeof(T), e);
        }
        public static T Create<T>(object? value)
        {
            return (T)Activator.CreateInstance(typeof(T), value);
        }
    }

    public sealed class Result<TValue> : IResult
    {
        public ResultState State { get; set; }
        public object? Value { get; set; }
        public Exception? Exception { get; set; }

        public bool IsSuccess => State == ResultState.Success;
        public bool IsFailure => State == ResultState.Failure;

        public Result(TValue value)
        {
            State = ResultState.Success;
            Value = value;
            Exception = null;
        }

        public Result(Exception exception)
        {
            State = ResultState.Failure;
            Exception = exception;
            Value = null;
        }

        public static implicit operator Result<TValue>(TValue value) => new Result<TValue>(value);

        public TValue? GetValue() =>
            IsFailure
                ? default
                : (TValue)Value!;

        public bool TryGetValue([NotNullWhen(true)] out TValue? value)
        {
            value = GetValue();
            return IsSuccess;
        }

        public Result<TResult> Map<TResult>(Func<TValue, TResult> f) =>
            IsFailure
                ? new Result<TResult>(Exception!)
                : new Result<TResult>(f(GetValue()!));

        public TValue IfFail(TValue defaultValue) =>
            IsFailure
                ? defaultValue
                : GetValue()!;

        public void IfFail(Action<Exception> f)
        {
            if (IsFailure) f(Exception!);
        }

        public void IfSucc(Action<TValue> f)
        {
            if (IsSuccess) f(GetValue()!);
        }
    }
}