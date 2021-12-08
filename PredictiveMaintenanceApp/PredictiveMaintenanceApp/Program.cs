using Microsoft.ML.Data;
using Microsoft.ML;

// Initialize MLContext
var mlContext = new MLContext();

// Load the model
ITransformer mlModel = mlContext.Model.Load("PredictiveMaintenanceModel.zip", out var _);

Console.WriteLine("Making single prediction...");

// Create Prediction Engine
var predictionEngine = mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(mlModel);

// Define model input data
ModelInput singleDataPoint = new ModelInput()
{
    Product_ID = @"M14860",
    Type = @"M",
    Air_temperature = 290F,
    Process_temperature = 300F,
    Rotational_speed = 1200F,
    Torque = 45F,
    Tool_wear = 1F
};

// Predict
var singlePrediction = predictionEngine.Predict(singleDataPoint);

Console.WriteLine($"Single Prediction: {singlePrediction.Prediction}\n");

Console.WriteLine("Making batch predictions...");

// Load input data
IDataView inputData = mlContext.Data.LoadFromTextFile<ModelInput>("predictive-maintenance-batch.txt", separatorChar: ',', hasHeader: true);

// Make batch predictions with Transform method
IDataView predictions = mlModel.Transform(inputData);

// Inspect and print the predicted values
float[] predictionArray = predictions.GetColumn<float>("PredictedLabel").ToArray();

int i = 1;
foreach (var prediction in predictionArray)
{
    Console.WriteLine($"Prediction {i}: {prediction}");
    i++;
}

// Define model input schema
public class ModelInput
{
    [LoadColumn(1), ColumnName(@"Product ID")]
    public string Product_ID { get; set; }

    [LoadColumn(2), ColumnName(@"Type")]
    public string Type { get; set; }

    [LoadColumn(3), ColumnName(@"Air temperature")]
    public float Air_temperature { get; set; }

    [LoadColumn(4), ColumnName(@"Process temperature")]
    public float Process_temperature { get; set; }

    [LoadColumn(5), ColumnName(@"Rotational speed")]
    public float Rotational_speed { get; set; }

    [LoadColumn(6), ColumnName(@"Torque")]
    public float Torque { get; set; }

    [LoadColumn(7), ColumnName(@"Tool wear")]
    public float Tool_wear { get; set; }

    [LoadColumn(8), ColumnName(@"Machine failure")]
    public float Machine_failure { get; set; }
}

// Define model output schema
public class ModelOutput
{
    [ColumnName("PredictedLabel")]
    public float Prediction { get; set; }

    public float[] Score { get; set; }
}