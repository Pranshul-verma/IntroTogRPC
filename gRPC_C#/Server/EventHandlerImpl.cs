using EventHandler;
using Grpc.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Server
{
    public class EventServiceImpl : EventService.EventServiceBase
    {
        // private readonly ILogger<EventServiceImpl> _logger;
        private static ConcurrentDictionary<IServerStreamWriter<EventUpdate>, bool> _subscribers = new();
        System.Timers.Timer aTimer;
        public EventServiceImpl()
        {
            aTimer = new System.Timers.Timer(3000);
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
           
            //  _logger = logger;
        }
        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            NotifyClientsAsync();
        }
        public override async Task SubscribeToEvents(EventSubscriptionRequest request, IServerStreamWriter<EventUpdate> responseStream, ServerCallContext context)
        {
            _subscribers.TryAdd(responseStream, true);
            aTimer.Enabled = true;

            try
            {
                while (!context.CancellationToken.IsCancellationRequested)
                {
                    await Task.Delay(1000); // Keep the stream open
                }
            }
            finally
            {
                _subscribers.TryRemove(responseStream, out _);
            }
            //
            //_logger.LogInformation($"Client {request.ClientId} disconnected from event stream.");
        }


        public static async Task NotifyClientsAsync()
        {
            var update = new EventUpdate
            {
                Message = "event has been raised",
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            };

            foreach (var subscriber in _subscribers.Keys)
            {
                try
                {
                    await subscriber.WriteAsync(update);
                }
                catch
                {
                    _subscribers.TryRemove(subscriber, out _); // remove disconnected clients
                }
            }
        }
    }
}
