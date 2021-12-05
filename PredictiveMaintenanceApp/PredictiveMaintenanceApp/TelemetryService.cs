using System;
using System.Text.Json.Serialization;
using Azure.Data.Tables;
using PredictiveMaintenanceApp;

class TelemetryService
{
    string _connectionString
    {
        get { return Environment.GetEnvironmentVariable("DEVINT_TELEMETRY_CS"); }
    }

    readonly TableClient _tableClient;

    public TelemetryService(string tableName)
    {
        _tableClient = new TableClient(_connectionString, tableName);
    }

    public void Send(PredictiveMaintenanceModel.ModelInput reading, PredictiveMaintenanceModel.ModelOutput prediction)
    {
        var telemetryEvent = MapTelemetryEventToTableEntity(reading, prediction);

        _tableClient.AddEntity(telemetryEvent);
    }

    public TableEntity MapTelemetryEventToTableEntity(PredictiveMaintenanceModel.ModelInput reading, PredictiveMaintenanceModel.ModelOutput prediction)
    {
        var telemetryEvent = new TableEntity();
        telemetryEvent.PartitionKey = reading.Product_ID.ToString();
        telemetryEvent.RowKey = $"{DateTimeOffset.Now.ToUnixTimeMilliseconds()}";
        telemetryEvent["AirTemperature"] = reading.Air_temperature;
        telemetryEvent["ProcessTemperature"] = reading.Process_temperature;
        telemetryEvent["RotationalSpeed"] = reading.Rotational_speed;
        telemetryEvent["Torque"] = reading.Torque;
        telemetryEvent["ToolWear"] = reading.Tool_wear;
        telemetryEvent["MachineFailure"] = prediction.Prediction;

        return telemetryEvent;
    }
}
