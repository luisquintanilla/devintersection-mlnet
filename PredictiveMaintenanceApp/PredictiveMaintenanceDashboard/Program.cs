using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Text.Json;
using Azure.Data.Tables;
using PredictiveMaintenanceDashboard.Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString = Environment.GetEnvironmentVariable("DEVINT_TELEMETRY_CS");

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<TelemetryDBService>(sp =>
{
    return new TelemetryDBService(connectionString, "testtelemetry");
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}


app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

SeedData("TelemetrySeedData.json");

app.Run();

void SeedData(string fileName)
{
    var rawData = File.ReadAllText(fileName);

    var telemetryData = JsonSerializer.Deserialize<IEnumerable<TelemetrySeedData>>(rawData);

    var client = new TableClient(connectionString, "testtelemetry");

    foreach (var reading in telemetryData)
    {
        var telemetryEvent = new TableEntity();
        telemetryEvent.PartitionKey = reading.Product_ID.ToString();
        telemetryEvent.RowKey = $"{DateTimeOffset.Now.ToUnixTimeMilliseconds()}";
        telemetryEvent["AirTemperature"] = reading.Air_temperature;
        telemetryEvent["ProcessTemperature"] = reading.Process_temperature;
        telemetryEvent["RotationalSpeed"] = reading.Rotational_speed;
        telemetryEvent["Torque"] = reading.Torque;
        telemetryEvent["ToolWear"] = reading.Tool_wear;
        telemetryEvent["MachineFailure"] = reading.Machine_failure;

        client.AddEntity(telemetryEvent);
    }
}