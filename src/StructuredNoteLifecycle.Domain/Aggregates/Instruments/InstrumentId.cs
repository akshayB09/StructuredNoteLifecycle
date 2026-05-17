namespace StructuredNoteLifecycle.Domain.Aggregates.Instruments;

public sealed record InstrumentId(Guid Value)
{
    public static InstrumentId New() => new(Guid.NewGuid());
    public override string ToString() => $"INS-{Value:N}";
}
