using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

public class FcmsApi {
    private const string API_URL = "https://fleetcarrier.space/api";
    private const int API_TIMEOUT_SEC = 5;

    public string user;
    public string apiKey;
    private readonly string userAgent; // "EDD-FCMS/1.0.0"
    private readonly HttpClient httpClient;


    public FcmsApi(string user, string apiKey, string userAgent, HttpClient httpClient) {
        this.user = user;
        this.apiKey = apiKey;
        this.userAgent = userAgent;
        this.httpClient = httpClient;
        
        var maskedKey = (apiKey.Length <= 4) ? "..." : $"{apiKey[..2]}...{apiKey[^2..]}";
        Logger.log($"FCMS API: user [{user}], key [{maskedKey}], userAgent [{userAgent}]");
    }


    public string GenerateCarrierJumpJSON(string systemName, string body) {
        string additionalData = $"\"SystemName\":\"{systemName}\",\"Body\":\"{body}\"";
        return GenerateJsonData("CarrierJumpRequest", additionalData);
    }

    public string GenerateCarrierJumpCancelledJSON() {
        return GenerateJsonData("CarrierJumpCancelled");
    }

    public string GenerateMarketUpdateJSON() {
        return GenerateJsonData("MarketUpdate");
    }

    private string GenerateJsonData(string eventName, string additionalData = "") {
        string dataContent = string.IsNullOrEmpty(additionalData) 
            ? $"{{\"event\":\"{eventName}\"}}" 
            : $"{{\"event\":\"{eventName}\",{additionalData}}}";
        
        return $"{{\"user\":\"{user}\",\"key\":\"{apiKey}\",\"data\":{dataContent}}}";
    }


    public async Task SendRequestAsync(string jsonData) {
        try {
            Logger.log(jsonData);
            
            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(apiKey)) {
                Logger.log("Error: No user name or API key set!");
                return;
            }
            
            httpClient.Timeout = TimeSpan.FromSeconds(API_TIMEOUT_SEC);
            
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(API_URL, content);
            var responseContent = await response.Content.ReadAsStringAsync();
            
            Logger.log($"HTTP {(int)response.StatusCode}: {responseContent}");
        }
        catch (HttpRequestException ex) {
            Logger.log($"Network error: {ex.Message}");
        }
        catch (TaskCanceledException ex) {
            Logger.log($"Request timeout: {ex.Message}");
        }
        catch (Exception ex) {
            Logger.log($"Error: {ex.Message}");
        }
    }


    public void SendCarrierJumpRequest(string systemName, string body) {
        string json = GenerateCarrierJumpJSON(systemName, body);
        SendRequestAsync(json).GetAwaiter().GetResult();
    }

    public void SendCarrierJumpCancelled() {
        string json = GenerateCarrierJumpCancelledJSON();
        SendRequestAsync(json).GetAwaiter().GetResult();
    }

    public void SendMarketUpdate() {
        string json = GenerateMarketUpdateJSON();
        SendRequestAsync(json).GetAwaiter().GetResult();
    }
}
