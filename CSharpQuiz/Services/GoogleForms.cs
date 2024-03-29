using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace CSharpQuiz.Services;

public class GoogleForms(
        ILogger<GoogleForms> logger)
{
    readonly ILogger<GoogleForms> logger = logger;

    readonly HttpClient client = new();

    public async Task SubmitAsync(
        string id,
        Dictionary<string, string> formData)
    {
        logger.LogInformation("Google Forms Einreichung wird gesendet...");

        HttpResponseMessage response = await client.PostAsync($"https://docs.google.com/forms/d/e/{id}/formResponse", new FormUrlEncodedContent(formData));
        response.EnsureSuccessStatusCode();
    }
}