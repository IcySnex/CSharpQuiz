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
        logger.LogInformation("FormData wird zu url encoded...");
        FormUrlEncodedContent encodedContent = new(formData);

        HttpResponseMessage response = await client.PostAsync($"https://docs.google.com/forms/d/e/{id}/formResponse", encodedContent);
        response.EnsureSuccessStatusCode();

        logger.LogInformation("Google Forms Einreichung abgesendet.");
    }
}