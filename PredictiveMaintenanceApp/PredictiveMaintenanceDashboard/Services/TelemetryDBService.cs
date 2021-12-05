using System.Text.Json.Serialization;
using Azure.Data.Tables;

namespace PredictiveMaintenanceDashboard.Services
{
    public class TelemetryDBService
    {
        readonly TableClient _tableClient;

        public TelemetryDBService(string connectionString, string tableName)
        {
            _tableClient = new TableClient(connectionString, tableName);
        }

        public TelemetryModel MapEntityToTelemetryModel(TableEntity entity)
        {
            var telemetryModel = new TelemetryModel
            {
                ProductID = entity.PartitionKey,
                Observation = entity.RowKey,
                Timestamp = entity.Timestamp,
                Etag = entity.ETag.ToString(),
            };

            foreach (var key in entity.Keys)
            {
                telemetryModel[key] = entity[key];
            }

            return telemetryModel;
        }

        public IEnumerable<TelemetryModel> GetAllRows()
        {
            var entities = _tableClient.Query<TableEntity>();

            return entities.Select(entity => MapEntityToTelemetryModel(entity));
        }
    }

    public class TelemetrySeedData
    {
        public float UDI { get; set; }

        [JsonPropertyName(@"Product ID")]
        public string Product_ID { get; set; }

        public string Type { get; set; }

        [JsonPropertyName(@"Air temperature")]
        public float Air_temperature { get; set; }

        [JsonPropertyName(@"Process temperature")]
        public float Process_temperature { get; set; }

        [JsonPropertyName(@"Rotational speed")]
        public float Rotational_speed { get; set; }

        public float Torque { get; set; }

        [JsonPropertyName(@"Tool wear")]
        public float Tool_wear { get; set; }

        [JsonPropertyName(@"Machine failure")]
        public float Machine_failure { get; set; }
    }

    public class TelemetryModel
    {
        private Dictionary<string, object> _properties = new Dictionary<string, object>();

        public DateTimeOffset? Timestamp { get; set; }

        public string Etag { get; set; }

        public string ProductID { get; set; }
        public string Observation { get; set; }

        public object this[string name]
        {
            get => (ContainsProperty(name)) ? _properties[name] : null;
            set => _properties[name] = value;
        }

        public ICollection<string> PropertyNames => _properties.Keys;

        public int PropertyCount => _properties.Count;

        public bool ContainsProperty(string name) => _properties.ContainsKey(name);


    }
}
