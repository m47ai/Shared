#!/bin/bash

# Create a topic called '$TEST_TOPIC_NAME'
kafka-topics --create \
    --topic $TEST_TOPIC_NAME \
    --bootstrap-server broker:9092 \
    --replication-factor 1 \
    --partitions 1

# Insert data into the topic
echo "data1" | /usr/bin/kafka-console-producer --broker-list broker:9092 --topic $TEST_TOPIC_NAME
echo "data2" | /usr/bin/kafka-console-producer --broker-list broker:9092 --topic $TEST_TOPIC_NAME