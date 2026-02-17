# DeviceManagement-Microservices

## Architecture Overview

This project demonstrates a production-grade microservices architecture built using:

- ASP.NET Core Web API
- SQL Server
- MongoDB
- Apache Kafka
- Redis (optional distributed cache)
- Nginx (API Gateway)
- JWT Authentication
- Permission-based Authorization
- Background Services (Bulk Audit Logging)

---

## Microservices

### UserService
- SQL Server
- JWT Issuer
- Authentication & Authorization
- Kafka Producer/Consumer
- Cache (InMemory / Redis)
- Mongo Audit Logging
- Mongo Application Logging

### DeviceService
- MongoDB
- Kafka Producer/Consumer
- JWT Validation
- Permission-based Authorization
- Cache (InMemory / Redis)
- Bulk Audit Logging
- Application Logging

---

## Request Flow

Client → Nginx → Microservice → Kafka → Background Processing → Database

---

## Technologies

- .NET 8
- MongoDB
- SQL Server
- Apache Kafka
- Redis
- Nginx

---

## Author
Javed Miya
