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
    }
}
