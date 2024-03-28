using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CSharpQuiz.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wpf.Ui;
using Wpf.Ui.Controls;
using Wpf.Ui.Extensions;

namespace CSharpQuiz.ViewModels;

public partial class FeedbackViewModel : ObservableObject
{
    readonly ILogger<FeedbackViewModel> logger;
    readonly ContentDialogService dialogService;
    readonly GoogleForms googleForms;

    public FeedbackViewModel(
        ILogger<FeedbackViewModel> logger,
        ContentDialogService dialogService,
        GoogleForms googleForms)
    {
        this.logger = logger;
        this.googleForms = googleForms;
        this.dialogService = dialogService;

        logger.LogInformation("FeedbackViewModel wurde initialisiert.");
    }


    [ObservableProperty]
    double structurOfPresentation = 0;

    [ObservableProperty]
    double contentOfPresentation = 0;

    [ObservableProperty]
    ThumbRateState codeExamplesInPresentation = ThumbRateState.None;

    [ObservableProperty]
    double quizProgram = 0;

    [ObservableProperty]
    double usabilityOfQuizProgram = 0;

    [ObservableProperty]
    ThumbRateState difficultyOfQuiz = ThumbRateState.None;

    [ObservableProperty]
    ThumbRateState cSharpKnowledge = ThumbRateState.None;

    [ObservableProperty]
    string miscToPresentation = string.Empty;

    [ObservableProperty]
    string miscToQuizProgram = string.Empty;


    [RelayCommand]
    async Task SendAsync()
    {
        string formsId = "1FAIpQLSeBpEYweDF4iKjpnaM6cg6lD2EN5ZRdq0ox61NtwWWoVcZzNQ";

        Dictionary<string, string> formData = new()
        {
            { "entry.2081862456", $"{Environment.UserDomainName}: {Environment.UserName}" }
        };
        AddToFormData(formData, "1020723914", StructurOfPresentation);
        AddToFormData(formData, "913469758", ContentOfPresentation);
        AddToFormData(formData, "991316867", CodeExamplesInPresentation);
        AddToFormData(formData, "1058047094", QuizProgram);
        AddToFormData(formData, "1135088694", UsabilityOfQuizProgram);
        AddToFormData(formData, "1182605272", DifficultyOfQuiz);
        AddToFormData(formData, "785402086", CSharpKnowledge);
        AddToFormData(formData, "683359787", MiscToPresentation);
        AddToFormData(formData, "20210245", MiscToQuizProgram);

        if (formData.Count < 2)
        {
            logger.LogInformation("Feedback wurde abgebrochen: Keine Fragen beantwortet.");
            await dialogService.ShowAlertAsync("Etwas ist schief gelaufen.", "Du musst mindestens eine Frage beatworten um dein Feedback abzusenden.", "Okay");
            return;
        }

        try
        {
            await googleForms.SubmitAsync(formsId, formData);
            logger.LogInformation("Feedback wurde aufgenommen und gesendet.");

            await dialogService.ShowAlertAsync("Vielen Dank!", "Dein Feedback wurde an mich gesendet und ich werde es mir bald anschauen.", "Okay");
        }
        catch (Exception ex)
        {
            logger.LogError("Feedback Aufnahme fehlgeschlagen: {exception}.", ex.Message);
            await dialogService.ShowAlertAsync("Etwas ist schief gelaufen.", $"Dein Feedback konnte nicht gesendet werden. ({ex.Message})", "Okay");
        }
    }

    static void AddToFormData(
        Dictionary<string, string> formData,
        string entry,
        double value)
    {
        if (value != 0)
            formData.Add($"entry.{entry}", (value * 2).ToString());
    }

    static void AddToFormData(
        Dictionary<string, string> formData,
        string entry,
        ThumbRateState value)
    {
        switch (value)
        {
            case ThumbRateState.Liked:
                formData.Add($"entry.{entry}", "Ja");
                break;
            case ThumbRateState.Disliked:
                formData.Add($"entry.{entry}", "Nein");
                break;
        }
    }

    static void AddToFormData(
        Dictionary<string, string> formData,
        string entry,
        string value)
    {
        if (!string.IsNullOrWhiteSpace(value))
            formData.Add($"entry.{entry}", value);
    }
}