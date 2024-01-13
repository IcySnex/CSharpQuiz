using CommunityToolkit.Mvvm.Input;
using CSharpQuiz.Services;
using Microsoft.Extensions.Logging;

namespace CSharpQuiz.ViewModels;

internal partial class HomeViewModel
{
    readonly ILogger<HomeViewModel> logger;
    readonly Navigation navigation;

    public HomeViewModel(
        ILogger<HomeViewModel> logger,
        Navigation navigation)
    {
        this.logger = logger;
        this.navigation = navigation;
    }

    [RelayCommand]
    void Start()
    {
        navigation.Navigate("Quiz");
    }
}