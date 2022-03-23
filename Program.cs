using Azure.Messaging.EventHubs.Consumer;

namespace EventHubsStartingPosition
{
    internal static class Program
    {
        private static readonly string _connectionString = Environment.GetEnvironmentVariable("EVENT_HUBS_CONNECTION_STRING")
                ?? throw new Exception("EVENT_HUBS_CONNECTION_STRING not set");

        private static readonly TimeSpan _testDuration = TimeSpan.FromSeconds(5);
        private static readonly ReadEventOptions _readOptions = new ReadEventOptions { MaximumWaitTime = TimeSpan.FromSeconds(1) };

        static async Task Main(string[] args)
        {
            Console.WriteLine($"Each test will run for {_testDuration}");
            Console.WriteLine();

            await Test(EventPosition.FromSequenceNumber(-1, isInclusive: false), "EventPosition.FromSequenceNumber(-1, isInclusive: false)");
            await Test(EventPosition.FromSequenceNumber(-1, isInclusive: true), "EventPosition.FromSequenceNumber(-1, isInclusive: true)");
            await Test(EventPosition.FromOffset(-1, isInclusive: false), "EventPosition.FromOffset(-1, isInclusive: false)");
            await Test(EventPosition.FromOffset(-1, isInclusive: true), "EventPosition.FromOffset(-1, isInclusive: true)");
        }

        private static async Task Test(EventPosition startingPosition, string description)
        {
            Console.WriteLine(description);

            var cts = new CancellationTokenSource(_testDuration);

            Console.WriteLine("PartID\tSeqNo");

            var tasks = new Task[4];
            for (var i = 0; i < 4; i++)
            {
                var j = i;
                tasks[j] = Task.Run(async () =>
                {
                    try
                    {
                        await using var consumer = new EventHubConsumerClient(EventHubConsumerClient.DefaultConsumerGroupName, _connectionString);

                        var events = consumer.ReadEventsFromPartitionAsync(j.ToString(), startingPosition, _readOptions, cts.Token);
                        await foreach (var e in events)
                        {
                            if (e.Data != null)
                            {
                                Console.WriteLine($"{e.Partition.PartitionId}\t{e.Data?.SequenceNumber}");
                            }
                        }
                    }
                    catch (TaskCanceledException)
                    {
                    }
                });
            }

            await Task.WhenAll(tasks);
            Console.WriteLine();
        }
    }
}
