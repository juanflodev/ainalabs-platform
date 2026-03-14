using System.Net.Http.Json;

namespace AinaLabs.Infrastructure;


// DTOs para la solicitud
public record AnalyzePdfRequest(string tenant_id, string user_id, FileMetadata file_metadata, AnalysisConfig analysis_config);
public record AnalysisConfig(string language = "es", string focus = "general", bool generate_summary = true);
public record FileMetadata(string file_id, string filename, string storage_path);

// DTOs para la respuesta
public record AnalyzePdfResponse(string status, AnalysisResult analysis_result, UsageStats usage_stats);
public record AnalysisResult(string summary, List<KeyFinding> key_findings, List<string> entities, string vector_index_status);
public record KeyFinding(string topic, string risk_level, string description);
public record UsageStats(int tokens_consumed, string model_used);

public class AiEngineClient
{
    
    private readonly HttpClient _httpClient;

    public AiEngineClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<AnalyzePdfResponse?> AnalyzePdfAsync(AnalyzePdfRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("/v1/formulas/analyze-pdf", request);
        response.EnsureSuccessStatusCode();
        
        return await response.Content.ReadFromJsonAsync<AnalyzePdfResponse>();
    }
}

