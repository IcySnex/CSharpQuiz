using System;
using System.Windows;
using Wpf.Ui.Controls;

namespace CSharpQuiz.Views.Dialogs;

public partial class ResultOverviewDialog : ContentDialog
{
    public ResultOverviewDialog(
        double reachedPoints,
        double points,
        int correctAnswersCount,
        int questionCount,
        int hintCount,
        string timeEvolved)
    {
        Style = (Style)Application.Current.Resources[typeof(ContentDialog)];
        InitializeComponent();

        PointsRun.Text = $"{reachedPoints}/{points}";
        CorrectAnswersRun.Text = $"{correctAnswersCount}/{questionCount}";
        HintCountRun.Text = hintCount.ToString();
        TimeEvolvedRun.Text = timeEvolved;


        double basicScore = reachedPoints / points;
        double correctAnswersBonus = (double)correctAnswersCount / questionCount * 0.25;
        double hintPenalty = Math.Min((double)hintCount / questionCount, 0.15);
        double finalScore = Math.Max(Math.Min(basicScore + correctAnswersBonus - hintPenalty, 1.0), 0.0);

        Title = finalScore switch
        {
            < 0 => "?",
            < 0.25 => "Das ist nicht so toll.",
            < 0.5 => "Da ist noch Luft nach oben.",
            < 0.75 => "Nicht schlecht!",
            < 0.9 => "Gut gemacht!",
            _ => "WOW! Das war großartig!"
        };
        MarkTextBlock.Text = finalScore switch
        {
            >= 0.98 => "1",
            >= 0.93 => "1-",
            >= 0.88 => "1-2",
            >= 0.83 => "2+",
            >= 0.78 => "2",
            >= 0.73 => "2-",
            >= 0.68 => "2-3",
            >= 0.63 => "3+",
            >= 0.58 => "3",
            >= 0.53 => "3-",
            >= 0.48 => "3-4",
            >= 0.43 => "4+",
            >= 0.38 => "4",
            >= 0.33 => "4-",
            >= 0.28 => "4-5",
            >= 0.23 => "5+",
            >= 0.18 => "5",
            >= 0.13 => "5-",
            >= 0.08 => "5-6",
            >= 0.03 => "6+",
            _ => "6"
        };
    }
}