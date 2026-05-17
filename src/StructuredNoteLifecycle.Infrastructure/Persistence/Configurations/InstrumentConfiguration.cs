using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StructuredNoteLifecycle.Domain.Aggregates.Instruments;

namespace StructuredNoteLifecycle.Infrastructure.Persistence.Configurations;

public sealed class InstrumentConfiguration : IEntityTypeConfiguration<Instrument>
{
    public void Configure(EntityTypeBuilder<Instrument> builder)
    {
        builder.ToTable("Instruments");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasConversion(id => id.Value, value => new InstrumentId(value));

        builder.Property(x => x.Version).IsConcurrencyToken();
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(50);
        builder.Property(x => x.TradeId).HasMaxLength(100);
        builder.Property(x => x.ExternalConfirmRef).HasMaxLength(200);
        builder.Property(x => x.SettlementRef).HasMaxLength(200);

        builder.OwnsOne(x => x.Terms, terms =>
        {
            terms.Property(t => t.OrderId).HasMaxLength(100);
            terms.Property(t => t.QuoteId).HasMaxLength(100);
            terms.Property(t => t.CounterpartyId).HasMaxLength(50);
            terms.Property(t => t.Currency).HasMaxLength(3);
            terms.Property(t => t.IssuerId).HasMaxLength(50);
            terms.Property(t => t.ProductType).HasMaxLength(100);
            terms.Property(t => t.Isin).HasMaxLength(12);
            terms.Property(t => t.Notional).HasPrecision(18, 4);

            terms.OwnsOne(t => t.EconomicTerms, et =>
            {
                et.Property(e => e.Price).HasPrecision(18, 6);
                et.Property(e => e.Yield).HasPrecision(10, 6);
                et.Property(e => e.UpfrontFee).HasPrecision(10, 6);
                et.Property(e => e.HedgeRef).HasMaxLength(100);
            });

            // PayoffDefinition (complex nested with collections) stored as JSON
            terms.OwnsOne(t => t.PayoffDefinition, pd => pd.ToJson());
        });

        // CouponPeriods in a child table; EF accesses the private _couponPeriods field
        builder.OwnsMany(x => x.CouponPeriods, cp =>
        {
            cp.ToTable("CouponPeriods");
            cp.Property(x => x.PeriodId);
            cp.Property(x => x.Status).HasConversion<string>().HasMaxLength(50);
            cp.Property(x => x.DeterminedAmount).HasPrecision(18, 4);
            cp.Property(x => x.PaidAmount).HasPrecision(18, 4);
        });
        builder.Navigation(x => x.CouponPeriods)
            .HasField("_couponPeriods")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        // Snapshots stored as JSON on the row (audit trail, not queried relationally)
        builder.OwnsMany(x => x.Snapshots, s => s.ToJson());
        builder.Navigation(x => x.Snapshots)
            .HasField("_snapshots")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
