namespace CSharpQuiz.Shared;

public class Buch(
    string titel,
    string autor,
    double preis,
    bool istVerfügbar)
{
    public string Titel { get; } = titel;
    public string Autor { get; } = autor;
    public double Preis { get; set; } = preis;
    public bool IstVerfügbar { get; set; } = istVerfügbar;


    public override string ToString() =>
        $"{{ {Titel}, {Autor} - {Preis}€, {(IstVerfügbar ? "Verfügbar" : "Nicht Verfügbar")} }}";
}