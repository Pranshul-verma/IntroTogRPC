// See https://aka.ms/new-console-template for more information
using Dummy;
using EventHandler;
using Grpc.Core;

Console.WriteLine("Hello, World!");

const string target = "127.0.0.1:50051";

Channel channel = new Channel(target, ChannelCredentials.Insecure);

channel.ConnectAsync().ContinueWith((task) =>
{
    if (task.Status == TaskStatus.RanToCompletion)
    { Console.WriteLine("Client is connected to Server"); }
});
var tokenSource = new CancellationTokenSource();

var client = new EventService.EventServiceClient(channel);

var request = new EventSubscriptionRequest { ClientId = "1"};

//var request = new  { Sum = vartoAdd };

var responce =  client.SubscribeToEvents(request,cancellationToken: tokenSource.Token);


await foreach (var update in responce.ResponseStream.ReadAllAsync())
{
    Console.WriteLine($"Received Update: {update.Message} at {update.Timestamp}");
    tokenSource.Cancel();
    break;
}
//Console.WriteLine(responce.Result);
channel.ShutdownAsync().Wait();
Console.ReadKey();
