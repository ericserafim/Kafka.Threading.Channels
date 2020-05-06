# Kafka.Threading.Channels
Proof of concept by using Threading.Channels to increase the processing of a Kafka topic.
The idea was testing the package Threading.Channels in a real world problem. 
Once, Kafka only allow one consumer for a specific partition/topic. I tried to overcome this by using Threading.Channels.
With Threading.Channels, I can scale the application even it is limited to run only one instance.

## This solution is composed of:
* Publisher: It send messages to a Kafka broker
* Subscriber: It consume messages from a Kafka broker

![Project Flow](https://github.com/ericserafim/Kafka.Threading.Channels/blob/master/Docs/Kafka%20Subscriber.png)

## Settings
You can increase the processing, changing the settings "numberOfConsumers".
