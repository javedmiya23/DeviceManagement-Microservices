# Kafka Setup (Windows)

## 1. Start Zookeeper

cd C:\kafka
.\bin\windows\zookeeper-server-start.bat .\config\zookeeper.properties

## 2. Start Kafka Broker

cd C:\kafka
.\bin\windows\kafka-server-start.bat .\config\server.properties

## 3. Create Topics

.\bin\windows\kafka-topics.bat --create --topic user-commands --bootstrap-server localhost:9092
.\bin\windows\kafka-topics.bat --create --topic device-commands --bootstrap-server localhost:9092

## 4. Verify Topics

.\bin\windows\kafka-topics.bat --list --bootstrap-server localhost:9092
