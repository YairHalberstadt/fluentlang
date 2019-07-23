using System;
using System.Collections.Generic;
using System.Text;

namespace FluentLang.Compiler.Tests.Unit.Generated
{
	public static class Example
	{
		public const string DEMO = @"
namespace Math {
    interface Counter {
        Count() : int;
        Increment() : Counter;
        Save() : Counter;
        Restore() : Counter;
    }

    Increment(this : Counter) : Counter {
        let count = this.Count() + 1;
        return this + Count;

        Count(this : Counter) : int {
            return count;
        }
    }

    CreateCounter() : Counter {
        return {} + Count + Increment + Save + Restore + StoredValue;

        Count(this : Counter) : int {
            return 0;
        }

        Save(this : Counter) : Counter {
            let current = this.Count();
            return this + StoredValue;

            StoredValue(this : Counter) : int {
                return current;
            }
        }

        Restore(this : SavableCounter) : Counter {
            let stored = this.StoredValue();

            return this + Count;

            Count(this : Counter) : int {
                return stored;
            }
        }

        StoredValue(this : Counter) : int {
            return 0;
        }

        interface SavableCounter {
            StoredValue() : int;
        } + Counter
    }
}

namespace Disambiguation {
    M(this : {}) : {} {
        return {};
    }

    M(this : { P() : int; }) : {} {
        return {};
    }

    M(this : {}) : int {
        return 0;
    }

    SomeMethod() : {} {
        return {} + M({}) : {} + M({ P() : int; }) : {} + M({}) : int;
    }

    M1(this : {}) : int {
        return 0;
    }

    namespace Inner {
        M1(this : {}) : int {
            return 1;
        }

        SomeMethod(chooseInner : bool) : {} {
            return if (chooseInner) {} + M1 else {} + Disambiguation.M1;
        }
    }

    SomeMethod(chooseInner : bool) : {} {
        return if (chooseInner) {} + Inner.M1 else {} + M1;
    }
}

namespace Exporting {
    export interface Adder {
        Add(a : int, b : int) : int;
    }

    export Sum3(a : int, b : int, c : int, adder : Adder) : int {
        let intermediate = adder.Add(a, b);
        return adder.Add(intermediate, c);
    }
}

namespace Mixins {
    interface PrintValue {
        Value() : int;
        PrintValue() : {};
    }

    GetPrintValue() : PrintValue {
        return {} + Value + PrintValue;

        Value(this : {}) : int {
            return 42;
        }

        PrintValue(this : PrintValue) : {} {
            let _ = System.Console.GetConsole().WriteLine(this.Value());
            return {};
        }
    }

    PrintMixedInValue() : {} {
        let originalPrintValue = GetPrintValue();
        let mixedinPrintValue = {} + mixin originalPrintValue;
        
        let _ = System.Console.GetConsole().WriteLine(originalPrintValue + Value); //prints 73
        let _ = System.Console.GetConsole().WriteLine(mixedinPrintValue + Value); //prints 73
        let _ = (originalPrintValue + Value).PrintValue(); //prints 73
        let _ = (mixedinPrintValue + Value).PrintValue(); //prints 42
        return {};

        Value(this : {}) : int {
            return 73;
        }
    }
}";
	}
}
