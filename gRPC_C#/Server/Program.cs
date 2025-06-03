// See https://aka.ms/new-console-template for more information



//using 

using EventHandler;
using Server;
using Sum;

const int port = 50051;

Grpc.Core.Server server = null;

try
{
    server = new Grpc.Core.Server()
    {
       Services = { EventService.BindService(new EventServiceImpl()) },
       Ports = { new Grpc.Core.ServerPort("localhost", port, Grpc.Core.ServerCredentials.Insecure) }
    };

    server.Start();
    Console.WriteLine("Server Is listing on the port 50051");
    Console.ReadKey();
}
catch (IOException ex)
{
    Console.WriteLine("the server is failed to start " + ex.InnerException);
    throw;
}
finally
{
    if (server != null)
    {
        server.ShutdownAsync().Wait();
    }
}