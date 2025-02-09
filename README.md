# Simple User Activity Logger

## Introduction

This project demonstrates an **event-driven approach** to logging user activities using **RabbitMQ and .NET Web API**. Instead of logging actions synchronously within the application, user registration events are **published to RabbitMQ**, and a **background service consumes** these events for logging.

## Features

- **User Registration API**: Registers users and stores them in memory.
- **RabbitMQ Integration**: Publishes `UserRegistered` events asynchronously.
- **Event Consumer**: Listens for user registration events and logs them.

## Prerequisites

Ensure you have the following installed:

- **.NET SDK 8.0 or later**
- **RabbitMQ** (Ensure RabbitMQ is running)

## Setup

### 1. Clone the Repository

```sh
git clone <repository_url>
cd UserActivityLogger
```

### 2. Install Dependencies

```sh
dotnet restore
```

### 3. Configure RabbitMQ

Update `appsettings.json` with your RabbitMQ settings:

```json
"RabbitMQ": {
    "HostName": "localhost",
    "Port": 5672,
    "UserName": "guest",
    "Password": "guest",
    "QueueName": "user_registration_queue"
}
```

### 4. Run RabbitMQ

Ensure RabbitMQ is running locally:

```sh
rabbitmq-server
```

### 5. Run the Application

Start the .NET Web API:

```sh
dotnet run
```

## Usage

### **Register a User**

Send a POST request to register a user:

```sh
curl -X POST http://localhost:5000/api/user/register -H "Content-Type: application/json" -d '{"username": "johndoe", "email": "john@example.com"}'
```

Expected response:

```json
"User registered successfully"
```

### **Check the Consumer Output**

If the consumer is running, it should log the event:

```sh
User registration event received: User johndoe registered with email john@example.com
```

## Code Overview

### **User Registration API**

Handles user registration and publishes events.

```csharp
[HttpPost("register")]
public IActionResult RegisterUser([FromBody] User user)
{
    UserStore.Users.Add(user);
    _rabbitMQService.PublishMessage($"User {user.Username} registered with email {user.Email}");
    return Ok("User registered successfully");
}
```

### **RabbitMQ Service**

Establishes connection and publishes messages to RabbitMQ.

```csharp
var factory = new ConnectionFactory() { HostName = _settings.HostName };
_connection = factory.CreateConnection();
_channel = _connection.CreateModel();
_channel.QueueDeclare(_settings.QueueName, durable: true, exclusive: false, autoDelete: false);
```

### **Consumer Service**

Listens for messages and processes them asynchronously.

```csharp
var consumer = new EventingBasicConsumer(_channel);
consumer.Received += (model, ea) =>
{
    var message = Encoding.UTF8.GetString(ea.Body.ToArray());
    _logger.LogInformation($"User registration event received: {message}");
};
_channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);
```

## License

MIT License

## Contributing

Feel free to submit pull requests or open issues for improvements!

## Contact

For inquiries, reach out to [Your Name] at [your-email@example.com].
