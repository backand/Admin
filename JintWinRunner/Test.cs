using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;
using Jint.Native.Number;
using Jint.Parser;
using Jint.Parser.Ast;
using Jint.Runtime;
using System.Net;

namespace Jint.Tests.Runtime
{
    public class EngineTests : IDisposable
    {
        private readonly Engine _engine;

        public EngineTests()
        {
            _engine = new Engine()
                .SetValue("log", new Action<object>(Console.WriteLine))
                ;
        }

        void IDisposable.Dispose()
        {
        }


        private void RunTest(string source)
        {
            _engine.Execute(source);
        }

        public void ShouldInterpretScriptFile(string file)
        {
            const string prefix = "Jint.Tests.Runtime.Scripts.";

            var assembly = Assembly.GetExecutingAssembly();
            var scriptPath = prefix + file;

            using (var stream = assembly.GetManifestResourceStream(scriptPath))
                if (stream != null)
                    using (var sr = new StreamReader(stream))
                    {
                        var source = sr.ReadToEnd();
                        RunTest(source);
                    }
        }

        public void ShouldInterpretLiterals(object expected, string source)
        {
            var engine = new Engine();
            var result = engine.Execute(source).GetCompletionValue().ToObject();

        }

        
        public void ShouldInterpretVariableDeclaration()
        {
            var engine = new Engine();
            var result = engine
                .Execute("var foo = 'bar'; foo;")
                .GetCompletionValue()
                .ToObject();

        }

        public void ShouldInterpretBinaryExpression(object expected, string source)
        {
            var engine = new Engine();
            var result = engine.Execute(source).GetCompletionValue().ToObject();

        }

        public void ShouldEvaluateHasOwnProperty()
        {
            RunTest(@"
                var x = {};
                x.Bar = 42;
                assert(x.hasOwnProperty('Bar'));
            ");
        }

        public void FunctionConstructorsShouldCreateNewObjects()
        {
            RunTest(@"
                var Vehicle = function () {};
                var vehicle = new Vehicle();
                assert(vehicle != undefined);
            ");
        }

        public void NewObjectsInheritFunctionConstructorProperties()
        {
            RunTest(@"
                var Vehicle = function () {};
                var vehicle = new Vehicle();
                Vehicle.prototype.wheelCount = 4;
                assert(vehicle.wheelCount == 4);
                assert((new Vehicle()).wheelCount == 4);
            ");
        }

        public void PrototypeFunctionIsInherited()
        {
            RunTest(@"
                function Body(mass){
                   this.mass = mass;
                }
                Body.prototype.offsetMass = function(dm) {
                   this.mass += dm;
                   return this;
                }
                var b = new Body(36);
                b.offsetMass(6);
                assert(b.mass == 42);
            ");

        }

        public void FunctionConstructorCall()
        {
            RunTest(@"
                function Body(mass){
                   this.mass = mass;
                }
                
                var john = new Body(36);
                assert(john.mass == 36);
            ");
        }

        public void NewObjectsShouldUsePrivateProperties()
        {
            RunTest(@"
                var Vehicle = function (color) {
                    this.color = color;
                };
                var vehicle = new Vehicle('tan');
                assert(vehicle.color == 'tan');
            ");
        }

        public void FunctionConstructorsShouldDefinePrototypeChain()
        {
            RunTest(@"
                function Vehicle() {};
                var vehicle = new Vehicle();
                assert(vehicle.hasOwnProperty('constructor') == false);
            ");
        }

        public void NewObjectsConstructorIsObject()
        {
            RunTest(@"
                var o = new Object();
                assert(o.constructor == Object);
            ");
        }

        public void NewObjectsIntanceOfConstructorObject()
        {
            RunTest(@"
                var o = new Object();
                assert(o instanceof Object);
            ");
        }

        public void NewObjectsConstructorShouldBeConstructorObject()
        {
            RunTest(@"
                var Vehicle = function () {};
                var vehicle = new Vehicle();
                assert(vehicle.constructor == Vehicle);
            ");
        }

        public void NewObjectsIntanceOfConstructorFunction()
        {
            RunTest(@"
                var Vehicle = function () {};
                var vehicle = new Vehicle();
                assert(vehicle instanceof Vehicle);
            ");
        }

        public void ShouldEvaluateForLoops()
        {
            RunTest(@"
                var foo = 0;
                for (var i = 0; i < 5; i++) {
                    foo += i;
                }
                assert(foo == 10);
            ");
        }

        public void ShouldEvaluateRecursiveFunctions()
        {
            RunTest(@"
                function fib(n) {
                    if (n < 2) {
                        return n;
                    }
                    return fib(n - 1) + fib(n - 2);
                }
                var result = fib(6);
                assert(result == 8);
            ");
        }

        public void ShouldAccessObjectProperties()
        {
            RunTest(@"
                var o = {};
                o.Foo = 'bar';
                o.Baz = 42;
                o.Blah = o.Foo + o.Baz;
                assert(o.Blah == 'bar42');
            ");
        }


        public void ShouldConstructArray()
        {
            RunTest(@"
                var o = [];
                assert(o.length == 0);
            ");
        }

        public void ArrayPushShouldIncrementLength()
        {
            RunTest(@"
                var o = [];
                o.push(1);
                assert(o.length == 1);
            ");
        }

        public void ArrayFunctionInitializesLength()
        {
            RunTest(@"
                assert(Array(3).length == 3);
                assert(Array('3').length == 1);
            ");
        }

        public void ArrayIndexerIsAssigned()
        {
            RunTest(@"
                var n = 8;
                var o = Array(n);
                for (var i = 0; i < n; i++) o[i] = i;
                assert(o[0] == 0);
                assert(o[7] == 7);
            ");
        }

        public void ArrayPopShouldDecrementLength()
        {
            RunTest(@"
                var o = [42, 'foo'];
                var pop = o.pop();
                assert(o.length == 1);
                assert(pop == 'foo');
            ");
        }

        public void ArrayConstructor()
        {
            RunTest(@"
                var o = [];
                assert(o.constructor == Array);
            ");
        }

        public void DateConstructor()
        {
            RunTest(@"
                var o = new Date();
                assert(o.constructor == Date);
                assert(o.hasOwnProperty('constructor') == false);
            ");
        }

        public void ShouldConvertDateToNumber()
        {
            RunTest(@"
                assert(Number(new Date(0)) === 0);
            ");
        }

        public void DatePrimitiveValueShouldBeNaN()
        {
            RunTest(@"
                assert(isNaN(Date.prototype.valueOf()));
            ");
        }

        public void MathObjectIsDefined()
        {
            RunTest(@"
                var o = Math.abs(-1)
                assert(o == 1);
            ");
        }

        public void VoidShouldReturnUndefined()
        {
            RunTest(@"
                assert(void 0 === undefined);
                var x = '1';
                assert(void x === undefined);
                x = 'x'; 
                assert (isNaN(void x) === true);
                x = new String('-1');
                assert (void x === undefined);
            ");
        }

        public void TypeofObjectShouldReturnString()
        {
            RunTest(@"
                assert(typeof x === 'undefined');
                assert(typeof 0 === 'number');
                var x = 0;
                assert (typeof x === 'number');
                var x = new Object();
                assert (typeof x === 'object');
            ");
        }

        public void MathAbsReturnsAbsolute()
        {
            RunTest(@"
                assert(1 == Math.abs(-1));
            ");
        }

        public void NaNIsNan()
        {
            RunTest(@"
                var x = NaN; 
                assert(isNaN(NaN));
                assert(isNaN(Math.abs(x)));
            ");
        }

        public void ToNumberHandlesStringObject()
        {
            RunTest(@"
                x = new String('1');
                x *= undefined;
                assert(isNaN(x));
            ");
        }

        
        public void FunctionScopesAreChained()
        {
            RunTest(@"
                var x = 0;
                function f1(){
                  function f2(){
                    return x;
                  };
                  return f2();
  
                  var x = 1;
                }
                assert(f1() === undefined);
            ");
        }

        
        public void EvalFunctionParseAndExecuteCode()
        {
            RunTest(@"
                var x = 0;
                eval('assert(x == 0)');
            ");
        }

        
        public void ForInStatement()
        {
            RunTest(@"
                var x, y, str = '';
                for(var z in this) {
                    str += z;
                }
                
                assert(str == 'xystrz');
            ");
        }

        
        public void WithStatement()
        {
            RunTest(@"
                with (Math) {
                  assert(cos(0) == 1);
                }
            ");
        }

        
        public void ObjectExpression()
        {
            RunTest(@"
                var o = { x: 1 };
                assert(o.x == 1);
            ");
        }

        
        public void StringFunctionCreatesString()
        {
            RunTest(@"
                assert(String(NaN) === 'NaN');
            ");
        }

        
        public void ScopeChainInWithStatement()
        {
            RunTest(@"
                var x = 0;
                var myObj = {x : 'obj'};
                function f1(){
                  var x = 1;
                  function f2(){
                    with(myObj){
                      return x;
                    }
                  };
                  return f2();
                }
                assert(f1() === 'obj');
            ");
        }

        
        public void TryCatchBlockStatement()
        {
            RunTest(@"
                var x, y, z;
                try {
                    x = 1;
                    throw new TypeError();
                    x = 2;
                }
                catch(e) {
                    assert(x == 1);
                    assert(e instanceof TypeError);
                    y = 1;
                }
                finally {
                    assert(x == 1);
                    z = 1;
                }
                
                assert(x == 1);
                assert(y == 1);
                assert(z == 1);
            ");
        }

        
        public void FunctionsCanBeAssigned()
        {
            RunTest(@"
                var sin = Math.sin;
                assert(sin(0) == 0);
            ");
        }

        
        public void FunctionArgumentsIsDefined()
        {
            RunTest(@"
                function f() {
                    assert(arguments.length > 0);
                }
                f(42);
            ");
        }

        
        public void PrimitiveValueFunctions()
        {
            RunTest(@"
                var s = (1).toString();
                assert(s == '1');
            ");
        }

        public void OperatorsPrecedence(object expected, string source)
        {
            var engine = new Engine();
            var result = engine.Execute(source).GetCompletionValue().ToObject();

        }

        
        public void FunctionPrototypeShouldHaveApplyMethod()
        {
            RunTest(@"
                var numbers = [5, 6, 2, 3, 7];
                var max = Math.max.apply(null, numbers);
                assert(max == 7);
            ");
        }

        public void ShouldEvaluateParseInt(object expected, string source)
        {
            var engine = new Engine();
            var result = engine.Execute(source).GetCompletionValue().ToObject();

        }

        
        public void ShouldNotExecuteDebuggerStatement()
        {
            new Engine().Execute("debugger");
        }

        
        public void ShouldThrowStatementCountOverflow()
        {
        }

        
        public void ShouldThrowTimeout()
        {
         }


        
        public void CanDiscardRecursion()
        {
            var script = @"var factorial = function(n) {
                if (n>1) {
                    return n * factorial(n - 1);
                }
            };
            var result = factorial(500);
            ";

        }

        
        public void ShouldDiscardHiddenRecursion()
        {
            var script = @"var renamedFunc;
            var exec = function(callback) {
                renamedFunc = callback;
                callback();
            };
            var result = exec(function() {
                renamedFunc();
            });
            ";

        }

        
        public void ShouldRecognizeAndDiscardChainedRecursion()
        {
            var script = @" var funcRoot, funcA, funcB, funcC, funcD;
            var funcRoot = function() {
                funcA();
            };
 
            var funcA = function() {
                funcB();
            };
            var funcB = function() {
                funcC();
            };
            var funcC = function() {
                funcD();
            };
            var funcD = function() {
                funcRoot();
            };
            funcRoot();
            ";

        }

        
        public void ShouldProvideCallChainWhenDiscardRecursion()
        {
            var script = @" var funcRoot, funcA, funcB, funcC, funcD;
            var funcRoot = function() {
                funcA();
            };
 
            var funcA = function() {
                funcB();
            };
            var funcB = function() {
                funcC();
            };
            var funcC = function() {
                funcD();
            };
            var funcD = function() {
                funcRoot();
            };
            funcRoot();
            ";

            RecursionDepthOverflowException exception = null;

            try
            {
                new Engine(cfg => cfg.LimitRecursion()).Execute(script);
            }
            catch (RecursionDepthOverflowException ex)
            {
                exception = ex;
            }

       }

        
        public void ShouldAllowShallowRecursion()
        {
            var script = @"var factorial = function(n) {
                if (n>1) {
                    return n * factorial(n - 1);
                }
            };
            var result = factorial(8);
            ";

            new Engine(cfg => cfg.LimitRecursion(20)).Execute(script);
        }

        
        public void ShouldDiscardDeepRecursion()
        {
            var script = @"var factorial = function(n) {
                if (n>1) {
                    return n * factorial(n - 1);
                }
            };
            var result = factorial(38);
            ";

        }

        
        public void ShouldConvertDoubleToStringWithoutLosingPrecision()
        {
            RunTest(@"
                assert(String(14.915832707045631) === '14.915832707045631');
                assert(String(-14.915832707045631) === '-14.915832707045631');
                assert(String(0.5) === '0.5');
                assert(String(0.00000001) === '1e-8');
                assert(String(0.000001) === '0.000001');
                assert(String(-1.0) === '-1');
                assert(String(30.0) === '30');
                assert(String(0.2388906159889881) === '0.2388906159889881');
            ");
        }

        
        public void ShouldWriteNumbersUsingBases()
        {
            RunTest(@"
                assert(15.0.toString() === '15');
                assert(15.0.toString(2) === '1111');
                assert(15.0.toString(8) === '17');
                assert(15.0.toString(16) === 'f');
                assert(15.0.toString(17) === 'f');
                assert(15.0.toString(36) === 'f');
                assert(15.1.toString(36) === 'f.3llllllllkau6snqkpygsc3di');
            ");
        }

        
        public void ShouldNotAlterSlashesInRegex()
        {
            RunTest(@"
                assert(new RegExp('/').toString() === '///');
            ");
        }

        
        public void ShouldHandleEscapedSlashesInRegex()
        {
            RunTest(@"
                var regex = /[a-z]\/[a-z]/;
                assert(regex.test('a/b') === true);
                assert(regex.test('a\\/b') === false);
            ");
        }

        
        public void ShouldComputeFractionInBase()
        {
        }

        
        public void ShouldInvokeAFunctionValue()
        {
            RunTest(@"
                function add(x, y) { return x + y; }
            ");

            var add = _engine.GetValue("add");

        }


        
        public void ShouldNotInvokeNonFunctionValue()
        {
            RunTest(@"
                var x= 10;
            ");

            var x = _engine.GetValue("x");

        }

        public void ShouldConvertNumbersToDifferentBase(string expected, long number, int radix)
        {
            var result = NumberPrototype.ToBase(number, radix);
        }

        
        public void JsonParserShouldParseNegativeNumber()
        {
            RunTest(@"
                var a = JSON.parse('{ ""x"":-1 }');
                assert(a.x === -1);
                var b = JSON.parse('{ ""x"": -1 }');
                assert(b.x === -1);
            ");
        }

        
        public void JsonParserShouldDetectInvalidNegativeNumberSyntax()
        {
            RunTest(@"
                try {
                    JSON.parse('{ ""x"": -.1 }'); // Not allowed
                    assert(false);
                }
                catch(ex) {
                    assert(ex instanceof SyntaxError);
                }
            ");

            RunTest(@"
                try {
                    JSON.parse('{ ""x"": - 1 }'); // Not allowed
                    assert(false);
                }
                catch(ex) {
                    assert(ex instanceof SyntaxError);
                }
            ");
        }

        
        public void ShouldBeCultureInvariant()
        {
            // decimals in french are separated by commas
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("fr-FR");

            var engine = new Engine();

            var result = engine.Execute("1.2 + 2.1").GetCompletionValue().AsNumber();

            result = engine.Execute("JSON.parse('{\"x\" : 3.3}').x").GetCompletionValue().AsNumber();
        }

        
        public void ShouldGetTheLastSyntaxNode()
        {
            var engine = new Engine();
            engine.Execute("1.2");

            var result = engine.GetLastSyntaxNode();
        }

        
        public void ShouldGetParseErrorLocation()
        {
            var engine = new Engine();
            try
            {
                engine.Execute("1.2+ new", new ParserOptions { Source = "jQuery.js" });
            }
            catch (ParserException e)
            {
            }
        }

        
        public void ParseShouldReturnNumber()
        {
            var engine = new Engine();

            var result = engine.Execute("Date.parse('1970-01-01');").GetCompletionValue().AsNumber();
        }

        
        public void UtcShouldUseUtc()
        {
            const string customName = "Custom Time";
            var customTimeZone = TimeZoneInfo.CreateCustomTimeZone(customName, new TimeSpan(7, 11, 0), customName, customName, customName, null, false);
            var engine = new Engine(cfg => cfg.LocalTimeZone(customTimeZone));

            var result = engine.Execute("Date.UTC(1970,0,1)").GetCompletionValue().AsNumber();
        }

        
        public void ShouldUseLocalTimeZoneOverride()
        {
            const string customName = "Custom Time";
            var customTimeZone = TimeZoneInfo.CreateCustomTimeZone(customName, new TimeSpan(0, 11, 0), customName, customName, customName, null, false);

            var engine = new Engine(cfg => cfg.LocalTimeZone(customTimeZone));

            var epochGetLocalMinutes = engine.Execute("var d = new Date(0); d.getMinutes();").GetCompletionValue().AsNumber();

            var localEpochGetUtcMinutes = engine.Execute("var d = new Date(1970,0,1); d.getUTCMinutes();").GetCompletionValue().AsNumber();

            var parseLocalEpoch = engine.Execute("Date.parse('January 1, 1970');").GetCompletionValue().AsNumber();

            var epochToLocalString = engine.Execute("var d = new Date(0); d.toString();").GetCompletionValue().AsString();
        }

         public void ShouldParseAsUtc(string date)
        {
            const string customName = "Custom Time";
            var customTimeZone = TimeZoneInfo.CreateCustomTimeZone(customName, new TimeSpan(7, 11, 0), customName, customName, customName, null, false);
            var engine = new Engine(cfg => cfg.LocalTimeZone(customTimeZone));

            engine.SetValue("d", date);
            var result = engine.Execute("Date.parse(d);").GetCompletionValue().AsNumber();

        }

        public void ShouldParseAsLocalTime(string date)
        {
            const string customName = "Custom Time";
            var customTimeZone = TimeZoneInfo.CreateCustomTimeZone(customName, new TimeSpan(0, 11, 0), customName, customName, customName, null, false);
            var engine = new Engine(cfg => cfg.LocalTimeZone(customTimeZone)).SetValue("d", date);

            var result = engine.Execute("Date.parse(d);").GetCompletionValue().AsNumber();

        }

        
        public void EmptyStringShouldMatchRegex()
        {
            RunTest(@"
                var regex = /^(?:$)/g;
                assert(''.match(regex) instanceof Array);
            ");
        }

        
        public void ShouldExecuteHandlebars()
        {
            var url = "http://cdnjs.cloudflare.com/ajax/libs/handlebars.js/2.0.0/handlebars.js";
            var content = new WebClient().DownloadString(url);

            RunTest(content);

            RunTest(@"
                var source = 'Hello {{name}}';
                var template = Handlebars.compile(source);
                var context = {name: 'Paul'};
                var html = template(context);
                assert('Hello Paul' == html);
            ");
        }

        
        public void DateParseReturnsNaN()
        {
            RunTest(@"
                var d = Date.parse('not a date');
                assert(isNaN(d));
            ");
        }

        
        public void ShouldIgnoreHtmlComments()
        {
            RunTest(@"
                var d = Date.parse('not a date'); <!-- a comment -->
                assert(isNaN(d));
            ");
        }

        
        public void DateShouldAllowEntireDotNetDateRange()
        {
            var engine = new Engine();

            var minValue = engine.Execute("new Date('0001-01-01T00:00:00.000')").GetCompletionValue().ToObject();

            var maxValue = engine.Execute("new Date('9999-12-31T23:59:59.999')").GetCompletionValue().ToObject();
        }

        
        public void ShouldConstructNewArrayWithInteger()
        {
            RunTest(@"
                var a = new Array(3);
                assert(a.length === 3);
                assert(a[0] == undefined);
                assert(a[1] == undefined);
                assert(a[2] == undefined);
            ");
        }

        
        public void ShouldConstructNewArrayWithString()
        {
            RunTest(@"
                var a = new Array('foo');
                assert(a.length === 1);
                assert(a[0] === 'foo');
            ");
        }

        
        public void ShouldThrowRangeExceptionWhenConstructedWithNonInteger()
        {
            RunTest(@"
                var result = false;
                try {
                    var a = new Array(3.4);
                }
                catch(e) {
                    result = e instanceof RangeError;
                }
                assert(result);                
            ");
        }

        
        public void ShouldInitializeArrayWithSingleIngegerValue()
        {
            RunTest(@"
                var a = [3];
                assert(a.length === 1);
                assert(a[0] === 3);
            ");
        }

        
        public void ShouldInitializeJsonObjectArrayWithSingleIntegerValue()
        {
            RunTest(@"
                var x = JSON.parse('{ ""a"": [3] }');
                assert(x.a.length === 1);
                assert(x.a[0] === 3);
            ");
        }

        
        public void ShouldInitializeJsonArrayWithSingleIntegerValue()
        {
            RunTest(@"
                var a = JSON.parse('[3]');
                assert(a.length === 1);
                assert(a[0] === 3);
            ");
        }
    }
}