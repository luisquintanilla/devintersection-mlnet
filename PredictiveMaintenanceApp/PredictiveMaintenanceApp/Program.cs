// See https://aka.ms/new-console-template for more information
using Microsoft.ML;
using Microsoft.ML.Data;

var mlContext = new MLContext();

// Make a single prediction
ModelInput singleDataPoint = new ModelInput()
{
    Product_ID = @"M14860",
    Type = @"M",
    Air_temperature = 298.1F,
    Process_temperature = 308.6F,
    Rotational_speed = 1551F,
    Torque = 42.8F,
    Tool_wear = 0F
};

ITransformer mlModel = mlContext.Model.Load("PredictiveMaintenanceModel.zip", out var _);

var predictionEngine = mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(mlModel);

var singlePrediction = predictionEngine.Predict(singleDataPoint);

Console.WriteLine($"Single Prediction: {singlePrediction.Prediction}\n");


// Make a batch prediction
// Load input data:
IDataView inputData = mlContext.Data.LoadFromTextFile<ModelInput>("predictive-maintenance-batch.txt", separatorChar: ',');

// Make batch predictions with Transform method:
IDataView predictions = mlModel.Transform(inputData);

// Inspect and print the predicted values:
float[] predictionArray = predictions.GetColumn<float>("PredictedLabel").ToArray();

Console.WriteLine("Batch Predictions:");
int i = 1;
foreach (var prediction in predictionArray)
{
    Console.WriteLine($"Prediction {i}: {prediction}");
    i++;
}

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

public class ModelOutput
{
    [ColumnName("PredictedLabel")]
    public float Prediction { get; set; }

    public float[] Score { get; set; }
}