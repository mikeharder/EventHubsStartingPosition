import { EventHubConsumerClient, EventPosition, latestEventPosition } from "@azure/event-hubs";

const connectionString = process.env["EVENT_HUBS_CONNECTION_STRING"] || "";
if (connectionString == "") { throw new Error("EVENT_HUBS_CONNECTION_STRING not set"); }

const maxWaitTimeInSeconds = 1;
const testDurationSeconds = 5;

export async function main() {
    console.log(`Each test will run for ${testDurationSeconds} seconds`);
    await test({ sequenceNumber: -1, isInclusive: false }, "{ sequenceNumber: -1, isInclusive: false }")
    await test({ sequenceNumber: -1, isInclusive: true }, "{ sequenceNumber: -1, isInclusive: true }")
    await test({ offset: -1, isInclusive: false }, "{ offset: -1, isInclusive: false }")
    await test({ offset: -1, isInclusive: true }, "{ offset: -1, isInclusive: true }")
}

async function test(startPosition: EventPosition, description: string) {
    console.log(description);
    console.log("PartID\tSeqNo");

    const consumerClient = new EventHubConsumerClient("$Default", connectionString);

    const subscription = consumerClient.subscribe(
        {
            processEvents: async (events, context) => {
                for (const event of events) {
                    console.log(`${context.partitionId}\t${event.sequenceNumber}`);
                }
            },
            processError: async (err, context) => {
                console.log(`Error on partition "${context.partitionId}": ${err}`);
            },
        },
        {
            maxWaitTimeInSeconds: maxWaitTimeInSeconds,
            startPosition: startPosition
        }
    );

    await new Promise(f => setTimeout(f, testDurationSeconds * 1000));

    await subscription.close();
    await consumerClient.close();

    console.log();
}

main().catch((error) => {
    console.error("Error running sample:", error);
});
