# Product Demand Calculator

## Overview

This is a C# console application designed to efficiently calculate product demand from a large input CSV file. The
application features:

- Large file processing without full memory loading
- Multithreaded calculation
- Real-time progress tracking
- Configurable parallelism
- Cancellation support

## Definitions

- **Average Daily Sales (ADS)** — The average number of items sold per day.
  *Example*: If 5 rubber ducks were sold on one day and 10 on another, the ADS would be 7.5.

- **Sales Prediction** — An estimate of how many items of a specific product will be sold in the future, based on
  calculated metrics.

- **Demand** — The number of items that need to be purchased to meet future expected sales.

## Calculation Logic

### Average Daily Sales (ADS)

- Calculated as the total sales divided by the number of days the product was in stock
- Only considers days with actual sales
- Provides a normalized view of product popularity

### Sales Prediction

- Calculated by multiplying ADS by the number of forecast days
- Represents expected sales volume for a specific time period

### Demand Calculation

- Demand = Sales Prediction - Current Stock
- Determines the number of items that need to be purchased
- Ensures sufficient inventory to meet predicted sales

## Problem Statement

The application solves a specific business problem:

- Input: A CSV file with product details (ID, sales prediction, current stock)
- Process: Calculate demand for each product
- Output: A CSV file with product demand

### Input File Format

```
id, prediction, stock
123, 2, 1
456, 1, 3
```

### Output File Format

```
id, demand
123, 1
456, 0
```

## Features

- 🚀 Efficient large file processing
- 🔄 Multithreaded computation
- 📊 Real-time progress display
- ⚙️ Dynamic parallelism management
- 🛑 Calculation cancellation support

## Technologies

- .NET Core
- Spectre.Console (Console UI)
- System.Threading.Channels

## Configuration

The application uses `Presentation/Sales.Presentation.Console/appSettings.json` for configuration:

```json
{
  "AppSettings": {
    "ParallelismDegree": 10,
    "ChannelReaderCapacityInMb": 10,
    "ChannelWriterCapacityInMb": 10,
    "InputFilePath": "input.csv",
    "OutputFilePath": "output.csv"
  }
}
```

### Configuration Parameters

- `ParallelismDegree`: Number of simultaneous processing threads
- `ChannelReaderCapacityInMb`: Channel reader buffer size
- `ChannelWriterCapacityInMb`: Channel writer buffer size
- `InputFilePath`: Path to input CSV file
- `OutputFilePath`: Path for output CSV file

## Installation

1. Clone the repository
2. Ensure .NET Core SDK is installed
3. Restore dependencies:
   ```bash
   dotnet restore
   ```
4. Run the application:

    ```bash
    dotnet run
    ```

## Performance Considerations

- Processes large files without loading entire content into memory
- Uses channels for efficient thread communication
- Configurable parallelism for optimal resource utilization

## Additional Task

The application supports dynamic parallelism management:

- Adjust parallelism without restart
- Maintains calculation progress

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.