# GraphQL Demo Message Consumer

Welcome to the GraphQL Demo Message Consumer project! This repository demonstrates how to build a message consumer service that processes messages using RabbitMQ, and stores the results in a MongoDB database. The project follows clean architecture principles, ensuring maintainability and scalability.

## Features

- **Message Consumption**: Consumes messages from RabbitMQ, processes themGraphQL. 
- **MongoDB Storage**: Stores the processed data in MongoDB, leveraging its NoSQL capabilities.
- **Clean Architecture**: Adheres to clean architecture principles, promoting separation of concerns and modularity.
- **Dependency Injection**: Uses dependency injection to manage services, enhancing testability and flexibility.

## Technologies Used

- **.NET Core**: The foundation of the application, providing a cross-platform framework.
- **RabbitMQ**: A message broker for processing and managing messages.
- **MongoDB**: A NoSQL database used for storing processed data.
- **C#**: The primary programming language used in the project.
- **Dependency Injection**: Ensures loose coupling and enhances testability.
- **Clean Architecture**: Organizes code into layers for better maintainability and scalability.

## Getting Started

### Prerequisites

- .NET Core SDK 6.0 or later
- RabbitMQ server
- MongoDB instance

### Installation

1. **Clone the repository:**

   ```bash
   git clone https://github.com/wajid7511/GraphqlDemoMessageConsumer.git
   cd GraphqlDemoMessageConsumer
   ```

2. **Configure the application:**

   Update the `appsettings.json` file with your RabbitMQ and MongoDB settings:

   ```json
   {
     "RabbitMQ": {
       "HostName": "your-rabbitmq-hostname",
       "QueueName": "your-queue-name",
       "UserName": "your-username",
       "Password": "your-password"
     },
     "MongoDB": {
       "ConnectionString": "your-mongodb-connection-string",
       "DatabaseName": "your-database-name"
     }
   }
   ```

3. **Restore dependencies and build the project:**

   ```bash
   dotnet restore
   dotnet build
   ```

4. **Run the application:**

   ```bash
   dotnet run
   ```

## Usage

- The service will automatically start consuming messages from RabbitMQ base on configuration.
- It will process each message.
- The processed data will be stored in the configured MongoDB database.

## Contributing

Contributions are welcome! If you'd like to contribute to this project, please fork the repository, make your changes, and submit a pull request.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

## Acknowledgements

- [RabbitMQ](https://www.rabbitmq.com/) for providing the messaging backbone.
- [GraphQL](https://graphql.org/) for enabling flexible and efficient data querying.
- [MongoDB](https://www.mongodb.com/) for its powerful NoSQL database platform.

---
This project is a solid foundation for building scalable and efficient message-driven applications using .NET Core, RabbitMQ, GraphQL, and MongoDB. Explore, experiment, and expand it to fit your needs!
```