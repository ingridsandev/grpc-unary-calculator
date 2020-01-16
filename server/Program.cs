using Calculator;
using Google.Protobuf.Reflection;
using Grpc.Core;
using Grpc.Reflection;
using Grpc.Reflection.V1Alpha;
using System;
using System.Collections.Generic;
using System.IO;

namespace server
{
    class Program
    {
        const int Port = 50051;

        static void Main(string[] args)
        {
            Server server = null;

            try
            {
                var serverCert = File.ReadAllText("ssl/server.crt");
                var serverKey = File.ReadAllText("ssl/server.key");
                var caCrt = File.ReadAllText("ssl/ca.crt");

                var credentials = new SslServerCredentials(new List<KeyCertificatePair>()
                {
                    new KeyCertificatePair(serverCert, serverKey)
                }, caCrt, true);

                server = new Server()
                {
                    Services = 
                    { 
                        CalculatorService.BindService(new CalculatorServiceImplementation()),
                        ServerReflection.BindService(new ReflectionServiceImpl(new List<ServiceDescriptor>() { CalculatorService.Descriptor }))
                    },
                    Ports = { new ServerPort("localhost", Port, ServerCredentials.Insecure) },
                };

                server.Start();
                Console.WriteLine("The server is listening on the port: 50051");
                Console.ReadKey();
            }
            catch (IOException e)
            {
                Console.WriteLine($"Something went wrong - {e.Message}");
                throw;
            }
            finally
            {
                if (server != null)
                    server.ShutdownAsync().Wait();
            }
        }
    }
}
