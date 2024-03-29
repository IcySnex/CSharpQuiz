﻿using CommunityToolkit.Mvvm.Input;
using CSharpQuiz.Services;
using Microsoft.Extensions.Logging;

namespace CSharpQuiz.ViewModels;

public partial class HomeViewModel
{
    readonly Navigation navigation;

    public HomeViewModel(
        ILogger<HomeViewModel> logger,
        Navigation navigation)
    {
        this.navigation = navigation;

        logger.LogInformation("HomeViewModel wurde initialisiert.");
    }


    [RelayCommand]
    void StartQuiz() =>
        navigation.Navigate("Quiz");

    [RelayCommand]
    void GiveFeedback() =>
        navigation.Navigate("Feedback");
}