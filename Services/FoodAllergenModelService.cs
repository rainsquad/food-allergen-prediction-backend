using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using System.Text.Json;

public class FoodAllergenModelService
{
    private readonly Dictionary<int, InferenceSession> _models = new();
    private readonly Dictionary<int, int> _constants = new();
    private readonly Dictionary<string, int> _vocab = new();
    private readonly List<string> _classes = new List<string>();
    private readonly int _featureCount;

    public FoodAllergenModelService(IWebHostEnvironment env)
    {
        var basePath = Path.Combine(env.ContentRootPath, "Models", "onnx");

        _constants = JsonSerializer.Deserialize<Dictionary<int, int>>(
            File.ReadAllText(Path.Combine(basePath, "constants.json"))
        )!;

        var exported = JsonSerializer.Deserialize<List<int>>(
            File.ReadAllText(Path.Combine(basePath, "exported.json"))
        )!;

        _vocab = JsonSerializer.Deserialize<Dictionary<string, int>>(
            File.ReadAllText(Path.Combine(basePath, "vocab.json"))
        )!;

        _classes = JsonSerializer.Deserialize<List<string>>(
             File.ReadAllText(Path.Combine(basePath, "labels.json"))
        )!;

        foreach (var i in exported)
        {
            var path = Path.Combine(basePath, $"allergen_{i}.onnx");
            _models[i] = new InferenceSession(path);
        }

        // ⚠️ MUST match training vector size
        _featureCount = 777;
    }

    public Dictionary<int, bool> Predict(float[] features)
    {
        if (features.Length != _featureCount)
            throw new Exception("Invalid feature vector length");

        var results = new Dictionary<int, bool>();

        foreach (var kv in _constants)
            results[kv.Key] = kv.Value == 1;


        foreach (var kv in _models)
        {
            var tensor = new DenseTensor<float>(features, new[] { 1, _featureCount });
            var input = NamedOnnxValue.CreateFromTensor("float_input", tensor);

            using var output = kv.Value.Run(new[] { input });
            var firstOutput = output.FirstOrDefault();

            if (firstOutput == null)
            {
                results[kv.Key] = false;
                continue;
            }

            // Handle int64 outputs (0 or 1)
            if (firstOutput.AsTensor<long>() is Tensor<long> longTensor && longTensor.Length > 0)
            {
                var pred = longTensor.GetValue(0); // 0 or 1
                results[kv.Key] = pred == 1;
            }
            else
            {
                results[kv.Key] = false; // fallback
            }
        }

        return results;

    }

    public float[] Tokenize(string text)
    {
        float[] vector = new float[_vocab.Count];
        var tokens = text.ToLower().Split(new[] { ' ', ',', ';', '-', '.' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (var token in tokens)
        {
            if (_vocab.TryGetValue(token, out int idx))
                vector[idx] += 1; // CountVectorizer style
        }
        return vector;
    }

    public List<string> MapToClasses(Dictionary<int, bool> results)
    {
        List<string> allergenNames = new List<string>();

        foreach (var kv in results)
        {
            if (kv.Value)
                allergenNames.Add(_classes[kv.Key]);
        }

        return allergenNames;
    }

}
