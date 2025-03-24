# Stock Exchange Dashboard

A real-time stock price monitoring dashboard with live updates and interactive charts.

## Features

- **Real-time Stock Updates**: Get live stock price updates using SignalR WebSockets
- **Interactive Dashboard**: Sort, filter, and search stocks in real-time
- **Price Charts**: View historical price data in an interactive chart
- **Responsive Design**: Works seamlessly on desktop and mobile devices
- **Drag and Drop**: Customize the order of your stock cards

## Technologies Used

### Backend
- ASP.NET Core 8.0
- SignalR for real-time communication
- Serilog for structured logging
- Swagger for API documentation

### Frontend
- HTML5, CSS3, JavaScript
- Chart.js for interactive charts
- SignalR client for WebSocket connections
- Sortable.js for drag-and-drop functionality

## Getting Started

### Prerequisites
- .NET 8.0 SDK or later
- Visual Studio 2022 or VS Code

### Installation

1. Clone the repository
```bash
git clone https://github.com/MateusjsSilva/StockExchangeDashboard.git
cd StockExchangeDashboard
```

2. Build and run the application
```bash
cd StocksEnchange.API
dotnet build
dotnet run
```

3. Open your browser and navigate to:
```
https://localhost:5001
```

### Docker Support

Coming soon!

## API Endpoints

| Endpoint | Method | Description |
|----------|--------|-------------|
| `/api/stock` | GET | Get all current stock prices |
| `/api/stock/info` | GET | Get API information |
| `/stockHub` | WebSocket | SignalR hub for real-time updates |

## Real-time Events

| Event | Description |
|-------|-------------|
| `ReceiveStockUpdates` | Receive batch updates for all stocks |
| `Welcome` | Connection confirmation with connection ID |

## Project Structure

```
StockExchangeDashboard/
├── StocksEnchange.API/           # API project
│   ├── Controllers/              # API controllers
│   ├── Hubs/                    # SignalR hubs
│   ├── Models/                  # Data models
│   ├── Services/                # Background services
│   ├── wwwroot/                 # Static web assets
│   │   └── index.html          # Main dashboard UI
│   └── Program.cs              # Application entry point
└── README.md                    # This file
```

## Development

### Logging

The application uses Serilog for structured logging:
- Console logs in development
- File-based logs in the `logs` directory:
  - `info_.log` for general information
  - `error_.log` for error tracking

### Adding New Stocks

To add new stock symbols, modify the `_stockSymbols` array in the `StockService.cs` file.

## License

This project is licensed under the Apache License 2.0 - see the [LICENSE](LICENSE) file for details.

## Acknowledgments

- Chart.js for the charting library
- SignalR for real-time communication capabilities
- Sortable.js for drag and drop functionality