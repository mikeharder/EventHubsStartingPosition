# EventHubsStartingPosition

## Repro Steps
1. Create an EventHub with 4 partitions.
2. Send 1 event to partition 0, 2 events to partition 1, and 0 events to partitions 2 and 3.  I used Service Bus Explorer.
3. Set EVENT_HUBS_CONNECTION_STRING environment variable.
4. Run app.

```
Each test will run for 00:00:05

EventPosition.FromSequenceNumber(-1, isInclusive: false)
PartID  SeqNo
1       0
1       1

EventPosition.FromSequenceNumber(-1, isInclusive: true)
PartID  SeqNo
1       0
1       1
0       0

EventPosition.FromOffset(-1, isInclusive: false)
PartID  SeqNo
0       0
1       0
1       1

EventPosition.FromOffset(-1, isInclusive: true)
PartID  SeqNo
0       0
1       0
1       1
```
