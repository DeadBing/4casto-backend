namespace FourCasto.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using FourCasto.Domain.Identity;
using FourCasto.Domain.Accounts;
using FourCasto.Domain.Markets;
using FourCasto.Domain.Settlement;
using FourCasto.Domain.CountryRules;
using FourCasto.Domain.Pricing;
using FourCasto.Domain.Fraud;
using FourCasto.Domain.Admin;

public class FourCastoDbContext : DbContext
{
    public FourCastoDbContext(DbContextOptions<FourCastoDbContext> options) : base(options) { }

    // Identity & Tenancy
    public DbSet<FourCastoWl> FourCastoWls => Set<FourCastoWl>();
    public DbSet<User> Users => Set<User>();
    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
    public DbSet<FourCastoWlUser> FourCastoWlUsers => Set<FourCastoWlUser>();

    // Accounts & Ledger
    public DbSet<Wallet> Wallets => Set<Wallet>();
    public DbSet<WalletBalance> WalletBalances => Set<WalletBalance>();
    public DbSet<TradingAccount> TradingAccounts => Set<TradingAccount>();
    public DbSet<TradingAccountBalance> TradingAccountBalances => Set<TradingAccountBalance>();
    public DbSet<WalletTransfer> WalletTransfers => Set<WalletTransfer>();
    public DbSet<BetHoldReservation> BetHoldReservations => Set<BetHoldReservation>();
    public DbSet<LedgerEntry> LedgerEntries => Set<LedgerEntry>();
    public DbSet<FundingSource> FundingSources => Set<FundingSource>();
    public DbSet<PaymentTransaction> PaymentTransactions => Set<PaymentTransaction>();

    // Markets & Bets
    public DbSet<SubjectGroup> SubjectGroups => Set<SubjectGroup>();
    public DbSet<Subject> Subjects => Set<Subject>();
    public DbSet<Signal> Signals => Set<Signal>();
    public DbSet<SignalOutcome> SignalOutcomes => Set<SignalOutcome>();
    public DbSet<Market> Markets => Set<Market>();
    public DbSet<Bet> Bets => Set<Bet>();
    public DbSet<BetPayoutSnapshot> BetPayoutSnapshots => Set<BetPayoutSnapshot>();
    public DbSet<BetEntryPriceContext> BetEntryPriceContexts => Set<BetEntryPriceContext>();
    public DbSet<SignalProgressMetric> SignalProgressMetrics => Set<SignalProgressMetric>();
    public DbSet<SignalLifecycleSnapshot> SignalLifecycleSnapshots => Set<SignalLifecycleSnapshot>();

    // Settlement & Cancellation
    public DbSet<Domain.Settlement.Settlement> Settlements => Set<Domain.Settlement.Settlement>();
    public DbSet<BetPayout> BetPayouts => Set<BetPayout>();
    public DbSet<BetSettlementExecution> BetSettlementExecutions => Set<BetSettlementExecution>();
    public DbSet<BetCancellationRequest> BetCancellationRequests => Set<BetCancellationRequest>();
    public DbSet<BetCancellationExecution> BetCancellationExecutions => Set<BetCancellationExecution>();
    public DbSet<BetCancellationEligibilitySnapshot> BetCancellationEligibilitySnapshots => Set<BetCancellationEligibilitySnapshot>();
    public DbSet<ResolutionSource> ResolutionSources => Set<ResolutionSource>();
    public DbSet<EvidenceRecord> EvidenceRecords => Set<EvidenceRecord>();
    public DbSet<ReasonCorrection> ReasonCorrections => Set<ReasonCorrection>();
    public DbSet<SettlementAdjustment> SettlementAdjustments => Set<SettlementAdjustment>();
    public DbSet<DisputeCase> DisputeCases => Set<DisputeCase>();
    public DbSet<DisputeComment> DisputeComments => Set<DisputeComment>();

    // Country Rules & Config
    public DbSet<ConfigVersion> ConfigVersions => Set<ConfigVersion>();
    public DbSet<CountryStatus> CountryStatuses => Set<CountryStatus>();
    public DbSet<CountryStatusQualificationRule> CountryStatusQualificationRules => Set<CountryStatusQualificationRule>();
    public DbSet<UserCountryStatusAssignment> UserCountryStatusAssignments => Set<UserCountryStatusAssignment>();
    public DbSet<CountryStatusBenefitPolicy> CountryStatusBenefitPolicies => Set<CountryStatusBenefitPolicy>();
    public DbSet<PayoutPolicyRule> PayoutPolicyRules => Set<PayoutPolicyRule>();
    public DbSet<BetCancellationPolicy> BetCancellationPolicies => Set<BetCancellationPolicy>();
    public DbSet<SignalProgressRule> SignalProgressRules => Set<SignalProgressRule>();
    public DbSet<SignalAvailabilityRule> SignalAvailabilityRules => Set<SignalAvailabilityRule>();
    public DbSet<PolicyResolutionLog> PolicyResolutionLogs => Set<PolicyResolutionLog>();
    public DbSet<RulePriorityPolicy> RulePriorityPolicies => Set<RulePriorityPolicy>();

    // Pricing
    public DbSet<QuoteSource> QuoteSources => Set<QuoteSource>();
    public DbSet<PriceFeedPolicy> PriceFeedPolicies => Set<PriceFeedPolicy>();
    public DbSet<PriceSnapshot> PriceSnapshots => Set<PriceSnapshot>();
    public DbSet<PriceTick> PriceTicks => Set<PriceTick>();

    // Fraud
    public DbSet<FraudRule> FraudRules => Set<FraudRule>();
    public DbSet<FraudEvent> FraudEvents => Set<FraudEvent>();
    public DbSet<UserRestriction> UserRestrictions => Set<UserRestriction>();

    // Admin
    public DbSet<AdminOverride> AdminOverrides => Set<AdminOverride>();
    public DbSet<IdempotencyRecord> IdempotencyRecords => Set<IdempotencyRecord>();
    public DbSet<ConcurrencyPolicy> ConcurrencyPolicies => Set<ConcurrencyPolicy>();
    public DbSet<NotificationTemplate> NotificationTemplates => Set<NotificationTemplate>();
    public DbSet<UserNotificationPreference> UserNotificationPreferences => Set<UserNotificationPreference>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FourCastoDbContext).Assembly);
    }
}
