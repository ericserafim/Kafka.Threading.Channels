# Kafka.Threading.Channels
Proof of concept by using Threading.Channels to increase the processing of a Kafka topic.
The idea was testing the package Threading.Channels in a real world problem.

## This solution is composed of:
* Publisher: It send messages to a Kafka broker
* Subscriber: It consume messages from a Kafka broker

![Project Flow](https://github.com/ericserafim/Kafka.Threading.Channels/blob/master/Docs/Kafka%20Subscriber.png)

You can increase the processing, changing the settings "numberOfConsumers".
