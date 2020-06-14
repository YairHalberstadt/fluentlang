using FluentLang.TestUtils;
using Xunit;
using Xunit.Abstractions;

namespace FluentLang.Compiler.Tests.Unit.Emit
{
	public class RegressionTests : TestBase
	{
		public RegressionTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
		{
		}

		[Fact, WorkItem("https://github.com/YairHalberstadt/fluentlang/issues/13")]
		public void StackOverflow1()
		{
			CreateAssembly(@"
export interface Func<TParam, TResult> {
    Invoke(param: TParam) : TResult;
}

namespace Sequences {
    export interface Sequence<T> {
        Decompose() : { Head() : T; Tail() : Sequence<T>; } | {};
    }
    
    export Empty<T>() : Sequence<T> {
        return {} + Decompose;
        Decompose(a : {}) : { Head() : T; Tail() : Sequence<T>; } | {} {
            return {};
        }
    }
    
    export Map<TSource, TResult>(source : Sequence<TSource>, func : Func<TSource, TResult>) : Sequence<TResult> {
        let decomposed = source.Decompose();
        return decomposed match {
            {} => Empty<TResult>();
            cons : { Head() : TSource; Tail() : Sequence<TSource>; } => MapAndCombine(cons);
        };
        MapAndCombine(cons : { Head() : TSource; Tail() : Sequence<TSource>; }) : Sequence<TResult> {
			return {} + Decompose;
			Decompose(a : {}) : { Head() : TResult; Tail() : Sequence<TResult>; } | {} {
				return {} + Head + Tail;
				Head(b: {}) : TResult { return func.Invoke(cons.Head()); }
				Tail(b : {}) : Sequence<TResult> { return Map<TSource, TResult>(cons.Tail(), func); }
			}
        }
    }
}")
				.VerifyDiagnostics()
				.VerifyEmit();
		}

		[Fact, WorkItem("https://github.com/YairHalberstadt/fluentlang/issues/20")]
		public void InternalErrors1()
		{
			CreateAssembly(@"interface Counter
{
    Increment() : Counter;
    Value() : int;
}

CreateCounter() : Counter
{
      return {} + Increment, Value;
      Increment(counter : Counter) : Counter
      {
          let value = counter.Value();
          return counter + Value;
          Value(this : {}) : int
          {
              return value + 1;
          }
      }

      Value(counter : {}) : int
      {
          return 0;
      }
}")
				.VerifyDiagnostics()
				.VerifyEmit();

		}
	}
}
