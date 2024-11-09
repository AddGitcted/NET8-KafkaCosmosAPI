# .NET8-CosmosDB-simple-API

## Features
- **Producer API**: Send single or a large batch of messages to a Kafka topic.
- **Consumer API**: Consume messages from Kafka and store them in CosmosDB.
- **Dockerized**: Run the entire stack (Kafka, CosmosDB Emulator, APIs) using Docker compose.


## Project Structure
```
.
├── ProducerAPI/            # Kafka producer API
├── ConsumerAPI/            # Kafka consumer API
└── docker-compose.yml      # Docker Compose file to run the services
```


## Setup & Run
1. **Clone the repository**:
   ```bash
   git clone https://github.com/AddGitcted/NET8-KafkaCosmosAPI.git
   cd NET8-KafkaCosmosAPI
   ```

2. **Build and run the services**:
   ```bash
   docker-compose up --build
   ```

3. **Access the Producer API**:
   - Single message: `POST http://localhost:5000/api/producer/single`
   - Batch messages: `POST http://localhost:5000/api/producer/batch`

4. **Access the CosmosDB Emulator**:
    ```bash
    https://localhost:8081/_explorer/index.html
    ```

## Sample Requests
### Single Message
```bash
curl -X POST http://localhost:5000/api/producer/single \
-H "Content-Type: application/json" \
-d '{"message": "Hello, Kafka!"}'
```

### Batch Messages
```bash
curl -X POST http://localhost:5000/api/producer/batch \
-H "Content-Type: application/json" \
-d '{"count": 1500}'
```