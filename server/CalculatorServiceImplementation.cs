using Calculator;
using Grpc.Core;
using System.Threading.Tasks;
using static Calculator.CalculatorService;

namespace server
{
    public class CalculatorServiceImplementation : CalculatorServiceBase
    {
        public override Task<CalculatorResponse> Calculator(CalculatorRequest request, ServerCallContext context)
        {
            try
            {
                var result = 0;

                switch (request.Operation)
                {
                    case "+":
                        result = request.FirstValue + request.SecondValue;
                        break;
                    case "-":
                        result = request.FirstValue - request.SecondValue;
                        break;
                    case "*":
                        result = request.FirstValue * request.SecondValue;
                        break;
                    case "/":
                        result = request.FirstValue / request.SecondValue;
                        break;
                }

                return Task.FromResult(new CalculatorResponse() { Result = result, Request = request });
            }
            catch (System.Exception e)
            {
                throw new RpcException(new Status(StatusCode.Internal, $"Something went wrong - Exception: {e}"));
            }
        }
    }
}
