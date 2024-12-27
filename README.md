# Redis Streaming Example

This project demonstrates the usage of Redis Streams in an ASP.NET Core 9 application. It simulates the process of generating 1 million fake account records and streaming them through Redis, where they are processed by consumers.

## Project Structure

- **AccountController**: 
  - Exposes an endpoint (`POST /accounts/create-1-million`) to generate 1 million fake account records and push them to Redis Stream using a producer.

- **Producer**:
  - `AccountCreateProducer`: Responsible for adding account records to Redis Stream.
  - `BaseProducer`: A generic base class for producing messages to a Redis Stream.

- **Consumer**:
  - `AccountCreateConsumer`: Reads messages from the Redis Stream and processes them.
  - `BaseConsumer`: A generic base class for consuming messages from a Redis Stream with support for message acknowledgment and batch processing.

- **Redis Integration**:
  - Redis is used for the stream-based messaging system where data is pushed by the producer and processed by the consumer.

## Key Concepts

- **Producer**: Generates data and pushes it to a Redis Stream.
- **Consumer**: Reads and processes the data from the Redis Stream.
- **Redis Stream**: A message queue used to handle large volumes of data asynchronously.

## How It Works

1. **Account Generation**: 
   - When the `POST /accounts/create-1-million` endpoint is hit, the controller generates 1 million fake account records using the [Bogus](https://github.com/bchavez/Bogus) library.
   
2. **Data Streaming**: 
   - These records are serialized and pushed to a Redis Stream using the `AccountCreateProducer`.

3. **Data Consumption**: 
   - The `AccountCreateConsumer` continuously listens for new records in the Redis Stream and processes them asynchronously.
