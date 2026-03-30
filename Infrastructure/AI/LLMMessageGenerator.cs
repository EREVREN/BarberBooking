using System.Text;
using System.Text.Json;
using BarberBooking.Contracts.AI;
using Microsoft.Extensions.Configuration;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace BarberBooking.Infrastructure.AI;

public class LLMMessageGenerator : IAIMessageGenerator
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<LLMMessageGenerator> _logger;
    private readonly string? _apiKey;
    private readonly string _baseUrl;
    private readonly string _provider;

    private readonly string? _openAiApiKey;
    private readonly string _openAiBaseUrl;
    private readonly string _openAiModel;

    private readonly string? _vercelAiGatewayApiKey;
    private readonly string _vercelAiGatewayBaseUrl;
    private readonly string _vercelAiGatewayModel;
    private readonly string[] _vercelAiGatewayFallbackModels;

    private readonly string? _huggingFaceApiKey;
    private readonly string _huggingFaceBaseUrl;
    private readonly string? _huggingFaceModel;

    private readonly string? _groqApiKey;
    private readonly string _groqBaseUrl;
    private readonly string _groqModel;

    private readonly string _ollamaBaseUrl;
    private readonly string? _ollamaModel;

    private readonly string? _modalApiKey;
    private readonly string _modalBaseUrl;
    private readonly string _modalModel;

    public LLMMessageGenerator(IConfiguration config, HttpClient httpClient, ILogger<LLMMessageGenerator> logger)
    {
        _httpClient = httpClient;
        _logger = logger;

        _provider = config["LLM:Provider"] ?? "Gemini";

        _openAiApiKey = NormalizeApiKey(config["OpenAI:ApiKey"]);
        _openAiBaseUrl = NormalizeBaseUrl(config["OpenAI:BaseUrl"], "https://api.openai.com/v1");
        _openAiModel = config["OpenAI:Model"] ?? "gpt-5";

        _vercelAiGatewayApiKey = NormalizeApiKey(config["VercelAiGateway:ApiKey"]);
        _vercelAiGatewayBaseUrl = NormalizeBaseUrl(config["VercelAiGateway:BaseUrl"], "https://ai-gateway.vercel.sh/v1");
        _vercelAiGatewayModel = config["VercelAiGateway:Model"] ?? "openai/gpt-5";
        _vercelAiGatewayFallbackModels = config
            .GetSection("VercelAiGateway:FallbackModels")
            .GetChildren()
            .Select(c => c.Value)
            .Where(v => !string.IsNullOrWhiteSpace(v))
            .Select(v => v!.Trim())
            .ToArray();

        _apiKey = NormalizeApiKey(config["LLM:ApiKey"]);
        _baseUrl = NormalizeBaseUrl(config["LLM:BaseUrl"], "https://generativelanguage.googleapis.com/v1beta");

        _huggingFaceApiKey = NormalizeApiKey(config["HuggingFace:ApiKey"]);
        // Common misconfig: people put HF token into LLM:ApiKey while Provider=HuggingFace.
        if (_huggingFaceApiKey == null &&
            string.Equals(_provider, "HuggingFace", StringComparison.OrdinalIgnoreCase))
        {
            _huggingFaceApiKey = _apiKey;
        }
        _huggingFaceBaseUrl = NormalizeBaseUrl(config["HuggingFace:BaseUrl"], "https://router.huggingface.co/hf-inference/models");
        _huggingFaceModel = config["HuggingFace:Model"];
        if (string.IsNullOrWhiteSpace(_huggingFaceModel))
        {
            _huggingFaceModel = null;
        }

        _groqApiKey = NormalizeApiKey(config["groq:ApiKey"]);
        // Common misconfig: people put Groq key into LLM:ApiKey while Provider=groq.
        if (_groqApiKey == null &&
            string.Equals(_provider, "groq", StringComparison.OrdinalIgnoreCase))
        {
            _groqApiKey = _apiKey;
        }
        _groqBaseUrl = NormalizeBaseUrl(config["groq:BaseUrl"], "https://api.groq.dev/v1");
        _groqModel = config["groq:Model"] ?? "groq/groq-1.5-mini";

        _modalApiKey = NormalizeApiKey(config["Modal:ApiKey"]);
        if (_modalApiKey == null &&
            (string.Equals(_provider, "Modal", StringComparison.OrdinalIgnoreCase) ||
             string.Equals(_provider, "TensorRTLLM", StringComparison.OrdinalIgnoreCase) ||
             string.Equals(_provider, "TensorRT-LLM", StringComparison.OrdinalIgnoreCase)))
        {
            // Common misconfig: people put Modal/TensorRT-LLM key into LLM:ApiKey while Provider=Modal.
            _modalApiKey = _apiKey;
        }
        _modalBaseUrl = NormalizeBaseUrl(config["Modal:BaseUrl"], "");
        _modalModel = (config["Modal:Model"] ?? "llama3-8b").Trim().Trim('"', '\'');
        _ollamaBaseUrl = NormalizeBaseUrl(config["Ollama:BaseUrl"], "http://localhost:11434");
        _ollamaModel = config["Ollama:Model"];
        if (string.IsNullOrWhiteSpace(_ollamaModel))
        {
            _ollamaModel = null;
        }

        LogProviderConfigSummary();
    }

    private void LogProviderConfigSummary()
    {
        // Do not log secrets. Only log whether they are present and what base URL/model is configured.
        _logger.LogWarning(
            "AI provider={Provider}. OpenAI key={HasOpenAiKey} baseUrl={OpenAiBaseUrl} model={OpenAiModel}. " +
            "VercelAiGateway key={HasGatewayKey} baseUrl={GatewayBaseUrl} model={GatewayModel}. " +
            "HuggingFace key={HasHfKey} baseUrl={HfBaseUrl} model={HfModel}. " +
            "Groq key={HasGroqKey} baseUrl={GroqBaseUrl} model={GroqModel}. " +
            "Modal key={HasModalKey} baseUrl={ModalBaseUrl} model={ModalModel}. " +
            "Ollama baseUrl={OllamaBaseUrl} model={OllamaModel}.",
            _provider,
            _openAiApiKey != null,
            _openAiBaseUrl,
            _openAiModel,
            _vercelAiGatewayApiKey != null,
            _vercelAiGatewayBaseUrl,
            _vercelAiGatewayModel,
            _huggingFaceApiKey != null,
            _huggingFaceBaseUrl,
            _huggingFaceModel ?? "(null)",
            _groqApiKey != null,
            _groqBaseUrl,
            _groqModel,
              _modalApiKey != null,
              _modalBaseUrl,
              _modalModel,
            _ollamaBaseUrl,
            _ollamaModel ?? "(null)");
    }

    private static string? NormalizeApiKey(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        var trimmed = value.Trim().Trim('\"', '\'');

        // These are common placeholder strings that sometimes end up committed into appsettings.
        if (trimmed.Equals("HuggingFace_API_KEY", StringComparison.OrdinalIgnoreCase)) return null;
        if (trimmed.Equals("GEMINI_API_KEY", StringComparison.OrdinalIgnoreCase)) return null;
        if (trimmed.Equals("OPENAI_API_KEY", StringComparison.OrdinalIgnoreCase)) return null;
        if (trimmed.StartsWith("process.env.", StringComparison.OrdinalIgnoreCase)) return null;

        return trimmed;
    }

    private static string NormalizeBaseUrl(string? value, string fallback)
    {
        var raw = string.IsNullOrWhiteSpace(value) ? fallback : value!;
        return raw.Trim().Trim('\"', '\'');
    }

    public async Task<string> GenerateConfirmationMessageAsync(
        string customerName,
        DateTime appointmentTime,
        string serviceName)
    {
        var prompt = $"Generate a friendly barber shop appointment confirmation message for {customerName}. " +
                     $"Service: {serviceName}, Time: {appointmentTime:f}. Keep it concise and professional.";

        return await CallApiAsync(prompt) ?? $"Hi {customerName}, your {serviceName} is confirmed for {appointmentTime:f}.";
    }

    public async Task<string> GenerateReminderMessageAsync(
        string customerName,
        DateTime appointmentTime,
        string serviceName)
    {
        var prompt = $"Generate a friendly appointment reminder for {customerName}. " +
                     $"Service: {serviceName}, Time: {appointmentTime:f}. Mention we are excited to see them.";

        return await CallApiAsync(prompt) ?? $"Reminder: {customerName}, your {serviceName} is at {appointmentTime:f}.";
    }

    public async Task<DateTime> DetermineOptimalReminderTimeAsync(DateTime appointmentTime, string serviceName)
    {
        var prompt = $"Given an appointment for '{serviceName}' at {appointmentTime:f}, " +
                     $"return ONLY a JSON object with a field 'minutesBefore' representing how many minutes before the appointment " +
                     $"the reminder should be sent for optimal customer preparation.";

        var response = await CallApiAsync(prompt, expectsJson: true);

        try
        {
            if (response != null)
            {
                var match = System.Text.RegularExpressions.Regex.Match(response, @"\d+");
                if (match.Success && int.TryParse(match.Value, out int minutes))
                {
                    return appointmentTime.AddMinutes(-minutes);
                }
            }
        }
        catch { /* Fallback */ }

        return appointmentTime.AddHours(-1);
    }

    public async Task<VoiceCommandResponse> ProcessVoiceCommandAsync(
        string transcript,
        string context,
        string? previousResponseId = null)
    {
        var prompt = $"The user said: '{transcript}'. Current context: {context}. " +
                     "Analyze the intent. Return ONLY a JSON with: " +
                     "'spokenResponse' (what to say back), 'steps' (array of strings describing technical steps to take), " +
                     "and optional 'scheduledTime' (ISO string if they want to book).";

        VoiceModelResult llm = new(null, null);
        if (string.Equals(_provider, "OpenAI", StringComparison.OrdinalIgnoreCase))
        {
            llm = await CallOpenAiResponsesAsync(
                apiKey: _openAiApiKey,
                baseUrl: _openAiBaseUrl,
                model: _openAiModel,
                prompt: prompt,
                expectsJson: true,
                previousResponseId: previousResponseId,
                fallbackModels: null);
        }
        else if (string.Equals(_provider, "VercelAiGateway", StringComparison.OrdinalIgnoreCase) ||
                 string.Equals(_provider, "AiGateway", StringComparison.OrdinalIgnoreCase))
        {
            llm = await CallOpenAiResponsesAsync(
                apiKey: _vercelAiGatewayApiKey,
                baseUrl: _vercelAiGatewayBaseUrl,
                model: _vercelAiGatewayModel,
                prompt: prompt,
                expectsJson: true,
                previousResponseId: previousResponseId,
                fallbackModels: _vercelAiGatewayFallbackModels.Length > 0 ? _vercelAiGatewayFallbackModels : null);
        }

        var response = llm.Text ?? await CallApiAsync(prompt, expectsJson: true);

        if (!string.IsNullOrWhiteSpace(response))
        {
            try
            {
                var jsonString = ExtractJsonObject(response);
                if (jsonString != null)
                {
                    using var doc = JsonDocument.Parse(jsonString);
                    var root = doc.RootElement;

                    var spoken = root.TryGetProperty("spokenResponse", out var spokenValue) &&
                                 spokenValue.ValueKind == JsonValueKind.String
                        ? spokenValue.GetString()
                        : null;

                    var steps = new List<string>();
                    if (root.TryGetProperty("steps", out var stepsValue) &&
                        stepsValue.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var step in stepsValue.EnumerateArray())
                        {
                            if (step.ValueKind == JsonValueKind.String)
                            {
                                steps.Add(step.GetString() ?? string.Empty);
                            }
                            else if (step.ValueKind != JsonValueKind.Null &&
                                     step.ValueKind != JsonValueKind.Undefined)
                            {
                                // Contract expects strings; preserve structured steps as compact JSON.
                                steps.Add(step.GetRawText());
                            }
                        }
                        steps.RemoveAll(s => string.IsNullOrWhiteSpace(s));
                    }

                    DateTime? scheduledTime = null;
                    if (root.TryGetProperty("scheduledTime", out var scheduledValue))
                    {
                        if (scheduledValue.ValueKind == JsonValueKind.String &&
                            DateTime.TryParse(scheduledValue.GetString(), out var parsed))
                        {
                            scheduledTime = parsed;
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(spoken) || steps.Count > 0 || scheduledTime != null)
                    {
                        return new VoiceCommandResponse(
                            spoken ?? "Okay.",
                            steps,
                            scheduledTime,
                            llm.ResponseId);
                    }
                }
            }
            catch
            {
                // fall through to raw response fallback
            }
        }

        // If the model didn't return JSON, still return something user-visible.
        if (!string.IsNullOrWhiteSpace(response))
        {
            return new VoiceCommandResponse(response.Trim(), new List<string>(), ScheduledTime: null, ResponseId: llm.ResponseId);
        }

        return new VoiceCommandResponse(
            "I'm sorry, I couldn't process that command.",
            new List<string> { "Error Parsing" },
            ScheduledTime: null,
            ResponseId: llm.ResponseId);
    }

    private async Task<string?> CallApiAsync(string prompt, bool expectsJson = false)
    {
        _logger.LogWarning("AI CallApiAsync provider={Provider} expectsJson={ExpectsJson}.", _provider, expectsJson);

        if (string.Equals(_provider, "Ollama", StringComparison.OrdinalIgnoreCase))
        {
            return await CallOllamaAsync(prompt, expectsJson);
        }

        if (string.Equals(_provider, "HuggingFace", StringComparison.OrdinalIgnoreCase))
        {
            return await CallHuggingFaceAsync(prompt);
        }

        if (string.Equals(_provider, "groq", StringComparison.OrdinalIgnoreCase))
        {
            return await CallGroqAsync(prompt, expectsJson);
        }
        if (string.Equals(_provider, "Modal", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(_provider, "TensorRTLLM", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(_provider, "TensorRT-LLM", StringComparison.OrdinalIgnoreCase))
        {
            return await CallModalAsync(prompt, expectsJson);
        }


        if (string.Equals(_provider, "OpenAI", StringComparison.OrdinalIgnoreCase))
        {
            var result = await CallOpenAiResponsesAsync(
                apiKey: _openAiApiKey,
                baseUrl: _openAiBaseUrl,
                model: _openAiModel,
                prompt: prompt,
                expectsJson: expectsJson,
                previousResponseId: null,
                fallbackModels: null);
            return result.Text;
        }

        if (string.Equals(_provider, "VercelAiGateway", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(_provider, "AiGateway", StringComparison.OrdinalIgnoreCase))
        {
            var result = await CallOpenAiResponsesAsync(
                apiKey: _vercelAiGatewayApiKey,
                baseUrl: _vercelAiGatewayBaseUrl,
                model: _vercelAiGatewayModel,
                prompt: prompt,
                expectsJson: expectsJson,
                previousResponseId: null,
                fallbackModels: _vercelAiGatewayFallbackModels.Length > 0 ? _vercelAiGatewayFallbackModels : null);
            return result.Text;
        }

        if (!string.Equals(_provider, "Gemini", StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogWarning("Unknown LLM:Provider={Provider}. Defaulting to Gemini call.", _provider);
        }
        return await CallGeminiAsync(prompt);
    }

    private async Task<string?> CallGroqAsync(string prompt, bool expectsJson)
    {
        if (_groqApiKey == null)
        {
            return null;
        }

        if (!TryBuildAbsoluteUri(_groqBaseUrl, "chat/completions", out var uri))
        {
            _logger.LogWarning("Invalid Groq baseUrl={BaseUrl}.", _groqBaseUrl);
            return null;
        }

        // Groq API is OpenAI-chat-completions compatible for many deployments.
        // Keep this minimal and defensive.
        var requestBody = new Dictionary<string, object?>
        {
            ["model"] = _groqModel,
            ["messages"] = new[]
            {
                new Dictionary<string, object?>
                {
                    ["role"] = "user",
                    ["content"] = prompt
                }
            },
            ["temperature"] = 0.2,
            ["max_tokens"] = 256
        };

        if (expectsJson)
        {
            requestBody["response_format"] = new Dictionary<string, object?> { ["type"] = "json_object" };
        }

        using var request = new HttpRequestMessage(HttpMethod.Post, uri);
        request.Headers.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _groqApiKey);
        request.Content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

        try
        {
            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode) return null;

            var responseJson = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseJson);

            if (doc.RootElement.ValueKind == JsonValueKind.Object &&
                doc.RootElement.TryGetProperty("choices", out var choices) &&
                choices.ValueKind == JsonValueKind.Array &&
                choices.GetArrayLength() > 0)
            {
                var choice0 = choices[0];
                if (choice0.ValueKind == JsonValueKind.Object &&
                    choice0.TryGetProperty("message", out var message) &&
                    message.ValueKind == JsonValueKind.Object &&
                    message.TryGetProperty("content", out var content) &&
                    content.ValueKind == JsonValueKind.String)
                {
                    return content.GetString();
                }
            }

            return null;
        }
        catch
        {
            return null;
        }
    }

    private async Task<string?> CallModalAsync(string prompt, bool expectsJson)
    {
        if (_modalApiKey == null)
        {
            return null;
        }

        if (!TryBuildAbsoluteUri(_modalBaseUrl, "chat/completions", out var uri))
        {
            _logger.LogWarning("Invalid Modal baseUrl={BaseUrl}.", _modalBaseUrl);
            return null;
        }

        // Modal deployment is expected to be OpenAI-chat-completions compatible.
        var requestBody = new Dictionary<string, object?>
        {
            ["model"] = _modalModel,
            ["messages"] = new[]
            {
                new Dictionary<string, object?>
                {
                    ["role"] = "user",
                    ["content"] = prompt
                }
            },
            ["temperature"] = 0.2,
            ["max_tokens"] = 256
        };

        if (expectsJson)
        {
            requestBody["response_format"] = new Dictionary<string, object?> { ["type"] = "json_object" };
        }

        using var request = new HttpRequestMessage(HttpMethod.Post, uri);
        request.Headers.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _modalApiKey);
        request.Content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

        try
        {
            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode) return null;

            var responseJson = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseJson);

            if (doc.RootElement.ValueKind == JsonValueKind.Object &&
                doc.RootElement.TryGetProperty("choices", out var choices) &&
                choices.ValueKind == JsonValueKind.Array &&
                choices.GetArrayLength() > 0)
            {
                var choice0 = choices[0];
                if (choice0.ValueKind == JsonValueKind.Object &&
                    choice0.TryGetProperty("message", out var message) &&
                    message.ValueKind == JsonValueKind.Object &&
                    message.TryGetProperty("content", out var content) &&
                    content.ValueKind == JsonValueKind.String)
                {
                    return content.GetString();
                }
            }

            return null;
        }
        catch
        {
            return null;
        }
    }
    private sealed record VoiceModelResult(string? Text, string? ResponseId);

    private async Task<VoiceModelResult> CallOpenAiResponsesAsync(
        string? apiKey,
        string baseUrl,
        string model,
        string prompt,
        bool expectsJson,
        string? previousResponseId,
        string[]? fallbackModels)
    {
        if (apiKey == null)
        {
            return new VoiceModelResult(null, null);
        }

        if (!TryBuildAbsoluteUri(baseUrl, "responses", out var uri))
        {
            return new VoiceModelResult(null, null);
        }

        var requestBody = new Dictionary<string, object?>
        {
            ["model"] = model,
            ["input"] = prompt
        };

        if (!string.IsNullOrWhiteSpace(previousResponseId))
        {
            requestBody["previous_response_id"] = previousResponseId;
        }

        if (expectsJson)
        {
            requestBody["instructions"] = "Return only valid JSON. Do not include markdown fences or extra text.";
        }

        // Vercel AI Gateway routing: allow listing fallback models.
        if (fallbackModels is { Length: > 0 })
        {
            requestBody["providerOptions"] = new
            {
                gateway = new
                {
                    models = fallbackModels
                }
            };
        }

        using var request = new HttpRequestMessage(HttpMethod.Post, uri);
        request.Headers.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);
        request.Content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

        try
        {
            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                return new VoiceModelResult(null, null);
            }

            var responseJson = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseJson);

            var id = doc.RootElement.TryGetProperty("id", out var idValue) && idValue.ValueKind == JsonValueKind.String
                ? idValue.GetString()
                : null;

            // Prefer output_text if present.
            string? text = null;
            if (doc.RootElement.TryGetProperty("output_text", out var outputText) &&
                outputText.ValueKind == JsonValueKind.String)
            {
                text = outputText.GetString();
            }

            // Fallback: output[0].content[0].text
            if (text == null &&
                doc.RootElement.TryGetProperty("output", out var output) &&
                output.ValueKind == JsonValueKind.Array &&
                output.GetArrayLength() > 0)
            {
                var first = output[0];
                if (first.ValueKind == JsonValueKind.Object &&
                    first.TryGetProperty("content", out var content) &&
                    content.ValueKind == JsonValueKind.Array &&
                    content.GetArrayLength() > 0)
                {
                    var firstContent = content[0];
                    if (firstContent.ValueKind == JsonValueKind.Object &&
                        firstContent.TryGetProperty("text", out var textValue) &&
                        textValue.ValueKind == JsonValueKind.String)
                    {
                        text = textValue.GetString();
                    }
                }
            }

            return new VoiceModelResult(text, id);
        }
        catch
        {
            return new VoiceModelResult(null, null);
        }
    }

    private static bool TryBuildAbsoluteUri(string baseUrl, string relativePath, out Uri uri)
    {
        uri = null!;

        var trimmed = (baseUrl ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(trimmed))
        {
            return false;
        }

        // Allow env vars like "ai-gateway.vercel.sh/v1" (no scheme).
        if (!trimmed.Contains("://", StringComparison.Ordinal))
        {
            trimmed = $"https://{trimmed}";
        }

        if (!trimmed.EndsWith("/", StringComparison.Ordinal))
        {
            trimmed += "/";
        }

        if (!Uri.TryCreate(trimmed, UriKind.Absolute, out var baseUri))
        {
            return false;
        }

        // Ensure we append path segments instead of replacing the last segment.
        var rel = relativePath.TrimStart('/');
        uri = new Uri(baseUri, rel);
        return true;
    }

    private async Task<string?> CallGeminiAsync(string prompt)
    {
        if (_apiKey == null)
        {
            // LLM key not configured; callers will fall back to non-AI responses.
            if (string.Equals(_provider, "Gemini", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning("Gemini provider selected but LLM:ApiKey is missing.");
            }
            return null;
        }

        var requestBody = new
        {
            contents = new[]
            {
                new { parts = new[] { new { text = prompt } } }
            }
        };

        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        if (!TryBuildAbsoluteUri(
                _baseUrl,
                $"models/gemini-1.5-flash:generateContent?key={Uri.EscapeDataString(_apiKey)}",
                out var uri))
        {
            _logger.LogWarning("Invalid Gemini baseUrl={BaseUrl}.", _baseUrl);
            return null;
        }

        try
        {
            var response = await _httpClient.PostAsync(uri, content);
            if (!response.IsSuccessStatusCode) return null;

            var responseJson = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseJson);

            return doc.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();
        }
        catch { return null; }
    }

    private async Task<string?> CallHuggingFaceAsync(string prompt)
    {
        if (_huggingFaceApiKey == null || _huggingFaceModel == null)
        {
            if (string.Equals(_provider, "HuggingFace", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning(
                    "HuggingFace provider selected but missing config: apiKeyPresent={HasKey} model={Model}.",
                    _huggingFaceApiKey != null,
                    _huggingFaceModel ?? "(null)");
            }
            return null;
        }

        if (!TryBuildAbsoluteUri(_huggingFaceBaseUrl, _huggingFaceModel, out var uri))
        {
            _logger.LogWarning("Invalid HuggingFace baseUrl={BaseUrl} model={Model}.", _huggingFaceBaseUrl, _huggingFaceModel);
            return null;
        }

        // Text-generation style request. The response shape varies by model/task,
        // so parsing is defensive and will return null on unknown shapes.
        var requestBody = new
        {
            inputs = prompt,
            parameters = new
            {
                max_new_tokens = 256,
                return_full_text = false
            }
        };

        using var request = new HttpRequestMessage(HttpMethod.Post, uri);
        request.Headers.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _huggingFaceApiKey);
        request.Content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

        try
        {
            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode) return null;

            var responseJson = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseJson);

            if (doc.RootElement.ValueKind == JsonValueKind.Array && doc.RootElement.GetArrayLength() > 0)
            {
                var first = doc.RootElement[0];
                if (first.ValueKind == JsonValueKind.Object &&
                    first.TryGetProperty("generated_text", out var generatedText) &&
                    generatedText.ValueKind == JsonValueKind.String)
                {
                    return generatedText.GetString();
                }
            }

            if (doc.RootElement.ValueKind == JsonValueKind.Object)
            {
                if (doc.RootElement.TryGetProperty("generated_text", out var generatedText) &&
                    generatedText.ValueKind == JsonValueKind.String)
                {
                    return generatedText.GetString();
                }

                if (doc.RootElement.TryGetProperty("error", out var errorValue) &&
                    errorValue.ValueKind == JsonValueKind.String)
                {
                    return null;
                }
            }

            return null;
        }
        catch
        {
            return null;
        }
    }

    private async Task<string?> CallOllamaAsync(string prompt, bool expectsJson)
    {
        if (_ollamaModel == null)
        {
            if (string.Equals(_provider, "Ollama", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning("Ollama provider selected but Ollama:Model is missing.");
            }
            return null;
        }

        if (!TryBuildAbsoluteUri(_ollamaBaseUrl, "api/generate", out var uri))
        {
            _logger.LogWarning("Invalid Ollama baseUrl={BaseUrl}.", _ollamaBaseUrl);
            return null;
        }
        var requestBody = new
        {
            model = _ollamaModel,
            prompt,
            stream = false,
            format = expectsJson ? "json" : null,
            // Keep responses bounded so "return JSON" prompts don't run forever.
            options = new
            {
                num_predict = 256
            }
        };

        try
        {
            var response = await _httpClient.PostAsync(
                uri,
                new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json"));

            if (!response.IsSuccessStatusCode) return null;

            var responseJson = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseJson);

            if (doc.RootElement.ValueKind == JsonValueKind.Object &&
                doc.RootElement.TryGetProperty("response", out var responseValue) &&
                responseValue.ValueKind == JsonValueKind.String)
            {
                return responseValue.GetString();
            }

            return null;
        }
        catch
        {
            return null;
        }
    }

    private class LLMVoiceResponse
    {
        public string SpokenResponse { get; set; } = "";
        public List<string> Steps { get; set; } = new();
        public DateTime? ScheduledTime { get; set; }
    }

    private static string? ExtractJsonObject(string input)
    {
        // Strip common markdown fences, then try to slice the first {...} region.
        var cleaned = input.Replace("```json", "", StringComparison.OrdinalIgnoreCase)
            .Replace("```", "", StringComparison.OrdinalIgnoreCase)
            .Trim();

        var start = cleaned.IndexOf('{');
        var end = cleaned.LastIndexOf('}');
        if (start < 0 || end <= start) return null;

        return cleaned.Substring(start, end - start + 1);
    }
}
