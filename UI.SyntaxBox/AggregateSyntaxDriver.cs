namespace UI.SyntaxBox;

/// <summary>
/// Aggregates a driver collection to be used using the singular ISyntaxDriver
/// interface.
/// </summary>
class AggregateSyntaxDriver : ISyntaxDriver
{
    readonly SyntaxDriverCollection drivers;

    internal AggregateSyntaxDriver(SyntaxDriverCollection Drivers)
    {
        drivers = Drivers ?? throw new ArgumentNullException(nameof(Drivers));
    }


    public DriverOperation Abilities =>
        drivers
            .Select((x) => x.Abilities)
            .Aggregate(DriverOperation.None, (a, b) => a | b);

    public IEnumerable<FormatInstruction> Match(DriverOperation operation, string text) =>
        drivers
            .Where((driver) => driver.Abilities.HasFlag(operation))
            .SelectMany((driver) => driver.Match(operation, text))
            .ToList();
}