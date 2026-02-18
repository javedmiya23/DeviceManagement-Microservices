# Docker Setup Guide

This project supports full containerized startup using Docker Compose.

Docker allows all services to start with a single command.

---

# Prerequisites

- Install Docker Desktop
- Ensure Docker is running

Verify installation:

```bash

docker --version
docker compose version

From project root:

docker compose up --build


This will start:

- Zookeeper
- Kafka
- MongoDB
- Redis
- SQL Server
- UserService
- DeviceService
- Nginx

---

## Create Kafka Topics (After Startup)

Find Kafka container:

docker ps


Enter container:

docker exec -it <kafka_container_name> bash


Create required topics:

kafka-topics --create --topic user-commands --bootstrap-server kafka:9092
kafka-topics --create --topic device-commands --bootstrap-server kafka:9092


---

## Access Application

Through Nginx:

http://localhost/api/auth/login
http://localhost/api/users
http://localhost/api/devices