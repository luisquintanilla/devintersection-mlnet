// See https://aka.ms/new-console-template for more information
using System.Text.Json;
using PredictiveMaintenanceApp;


// In this case, we're using simulated data 
// telemetry values. In the real-world, these would
// be read from device sensors directly.
var telemetryData = ReadData("RaspberryTelemetry.json");

var telemetryService = new TelemetryService("testtelemetry");

foreach (var reading in telemetryData)
{
    // Use model to make predictions
    var result = PredictiveMaintenanceModel.Predict(reading);

    // Send telemetry reading with prediction to DB
    telemetryService.Send(reading, result);

    // Handle machine failure
    if (result.Prediction == 1)
    {
        HandleFailure();
    }
    else
    {
        Console.WriteLine($"Healthy system state. Telemetry sent successfully");
    }

    // Add a slight delay
    Thread.Sleep(100);
}

void HandleFailure()
{
    Console.WriteLine("Machine failure detected! Restarting machine");
}

IEnumerable<PredictiveMaintenanceModel.ModelInput> ReadData(string dataFilePath)
{
    var rawData = File.ReadAllText(dataFilePath);

    var data = JsonSerializer.Deserialize<IEnumerable<PredictiveMaintenanceModel.ModelInput>>(rawData);

    return data;
}