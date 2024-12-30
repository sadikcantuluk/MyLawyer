using Microsoft.Extensions.Logging;
using AvukatimYanimda.Models;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace AvukatimYanimda.Services
{
    public interface IChatService
    {
        Task<string> GetResponseFromAI(string userMessage);
        Task<string> InitializeSourceDocument(string pdfPath);
    }

    public class ChatService : IChatService
    {
        private readonly ILogger<ChatService> _logger;
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private static string _sourceId = string.Empty;

        public ChatService(ILogger<ChatService> logger, HttpClient httpClient, IConfiguration configuration)
        {
            _logger = logger;
            _httpClient = httpClient;
            _apiKey = configuration["ChatPDF:ApiKey"] ?? throw new ArgumentNullException(nameof(configuration));
            _httpClient.DefaultRequestHeaders.Add("x-api-key", _apiKey);
        }

        public async Task<string> InitializeSourceDocument(string pdfPath)
        {
            try
            {
                if (!string.IsNullOrEmpty(_sourceId))
                {
                    _logger.LogInformation("PDF zaten yüklenmiş, mevcut sourceId kullanılıyor: {SourceId}", _sourceId);
                    return _sourceId;
                }

                _logger.LogInformation("PDF yükleme başlıyor: {PdfPath}", pdfPath);
                var pdfBytes = await File.ReadAllBytesAsync(pdfPath);
                _logger.LogInformation("PDF dosyası okundu, boyut: {Size} bytes", pdfBytes.Length);

                var content = new MultipartFormDataContent();
                content.Add(new ByteArrayContent(pdfBytes), "file", "anayasa.pdf");

                var response = await _httpClient.PostAsync("https://api.chatpdf.com/v1/sources/add-file", content);
                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("ChatPDF yanıtı: {Response}", responseContent);

                response.EnsureSuccessStatusCode();

                var sourceInfo = JsonSerializer.Deserialize<Dictionary<string, string>>(responseContent);
                
                if (sourceInfo == null || !sourceInfo.ContainsKey("sourceId"))
                {
                    throw new Exception($"Source ID alınamadı. Yanıt: {responseContent}");
                }

                _sourceId = sourceInfo["sourceId"];
                _logger.LogInformation("PDF başarıyla yüklendi, sourceId: {SourceId}", _sourceId);
                return _sourceId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PDF kaynağı yüklenirken hata oluştu: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<string> GetResponseFromAI(string userMessage)
        {
            try
            {
                if (string.IsNullOrEmpty(_sourceId))
                {
                    _logger.LogWarning("sourceId bulunamadı, PDF henüz yüklenmemiş olabilir");
                    return "Üzgünüm, henüz anayasa dökümanı yüklenmemiş. Lütfen sistem yöneticisine başvurun.";
                }

                _logger.LogInformation("AI yanıtı isteniyor, sourceId: {SourceId}, soru: {Question}", _sourceId, userMessage);

                var request = new
                {
                    sourceId = _sourceId,
                    messages = new[]
                    {
                        new
                        {
                            role = "user",
                            content = "Sen bir hukuk asistanısın. Türkiye Anayasası hakkında sorulan sorulara net ve anlaşılır cevaplar vermelisin. Cevaplarını verirken ilgili anayasa maddelerini de belirtmelisin."
                        },
                        new
                        {
                            role = "assistant",
                            content = "Anlaşıldı, size Türkiye Anayasası hakkında yardımcı olacağım ve cevaplarımda ilgili maddeleri belirteceğim."
                        },
                        new
                        {
                            role = "user",
                            content = userMessage
                        }
                    }
                };

                var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("https://api.chatpdf.com/v1/chats/message", content);
                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("ChatPDF yanıtı: {Response}", responseContent);

                response.EnsureSuccessStatusCode();

                var chatResponse = JsonSerializer.Deserialize<Dictionary<string, string>>(responseContent);
                var aiResponse = chatResponse?["content"];

                if (string.IsNullOrEmpty(aiResponse))
                {
                    _logger.LogWarning("AI boş yanıt döndü");
                    return "Üzgünüm, bir cevap alınamadı.";
                }

                _logger.LogInformation("AI yanıtı başarıyla alındı");
                return aiResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AI yanıtı alınırken hata oluştu: {Message}", ex.Message);
                return $"Üzgünüm, bir hata oluştu: {ex.Message}";
            }
        }
    }
} 