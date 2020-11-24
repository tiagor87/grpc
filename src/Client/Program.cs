using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using gRPC.Customer;
using Grpc.Net.Client;

namespace gRPC.Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using var channel = GrpcChannel.ForAddress(new Uri("http://localhost:5000"));
            var client = new Greeter.GreeterClient(channel);
            var source = new CancellationTokenSource();
            var stream = client.SayHelloStream(new HelloRequest()
            {
            }, cancellationToken: source.Token);
            var i = 0;
            while (!source.IsCancellationRequested && await stream.ResponseStream.MoveNext())
            {
                Interlocked.Increment(ref i);
                
                if (i % 10_000 == 0) Console.WriteLine(stream.ResponseStream.Current.Message);
                if (i % 12_000 == 0)
                {
                    source.Cancel();
                    Console.WriteLine("Finalized");
                }
            }

            Console.ReadLine();
        }
    }
}
