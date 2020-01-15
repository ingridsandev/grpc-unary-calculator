using Calculator;
using Grpc.Core;
using System;
using System.IO;
using System.Threading.Tasks;

namespace client
{
    class Program
    {
        const string target = "127.0.0.1:50051";

        static void Main(string[] args)
        {
            try
            {
                var clientCert = File.ReadAllText("ssl/client.crt");
                var clientKey = File.ReadAllText("ssl/client.key");
                var caCrt = File.ReadAllText("ssl/ca.crt");

                var channelCredentials = new SslCredentials(caCrt, new KeyCertificatePair(clientCert, clientKey));

                Channel channel = new Channel(target, channelCredentials);

                channel.ConnectAsync().ContinueWith((task) =>
                {
                    if (task.Status == TaskStatus.RanToCompletion)
                        Console.WriteLine("The client connected successfully");
                });

                var client = new CalculatorService.CalculatorServiceClient(channel);

                Console.WriteLine("Which is the FIRST value for your operation? ");
                var firstValue = Console.ReadLine();

                Console.WriteLine("Which is the SECOND value for your operation? ");
                var secondValue = Console.ReadLine();

                Console.WriteLine("Inform the operation:");
                Console.WriteLine("  + = Addition ");
                Console.WriteLine("  - = Subtraction ");
                Console.WriteLine("  * = Multiplication ");
                Console.WriteLine("  / = Division ");

                var operation = Console.ReadLine();

                var response = client.Calculator(new CalculatorRequest()
                {
                    FirstValue = Convert.ToInt32(firstValue),
                    SecondValue = Convert.ToInt32(secondValue),
                    Operation = operation
                }, deadline: DateTime.UtcNow.AddMilliseconds(500));

                Console.WriteLine(response.Result);

                channel.ShutdownAsync().Wait();
                Console.ReadLine();
            }
            catch (RpcException e) when (e.StatusCode == StatusCode.DeadlineExceeded)
            {
                Console.WriteLine($"The communication exceeded the deadline");
            }
            catch (RpcException e)
            {
                Console.WriteLine($"StatusCode: {e.Status.StatusCode} | Detail: {e.Status.Detail}");
            }
            catch (Exception e)
            {
                Console.WriteLine("Something went wrong.");
            }
        }
    }
}
