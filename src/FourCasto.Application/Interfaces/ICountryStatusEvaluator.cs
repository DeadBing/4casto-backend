namespace FourCasto.Application.Interfaces;

public record CountryStatusResult(
    Guid? CountryStatusId,
    string StatusName,
    string CountryCode
);

public interface ICountryStatusEvaluator
{
    Task<CountryStatusResult> EvaluateAsync(Guid fourCastoWlId, Guid userId);
}
