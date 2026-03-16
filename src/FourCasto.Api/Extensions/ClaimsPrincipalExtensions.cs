namespace FourCasto.Api.Extensions;

using System.Security.Claims;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var claim = user.FindFirst(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException("User ID claim not found");
        return Guid.Parse(claim.Value);
    }

    public static Guid GetFourCastoWlId(this ClaimsPrincipal user)
    {
        var claim = user.FindFirst("fourcastowl_id")
            ?? throw new UnauthorizedAccessException("FourCastoWL ID claim not found");
        return Guid.Parse(claim.Value);
    }

    public static bool IsGuest(this ClaimsPrincipal user)
    {
        var claim = user.FindFirst("is_guest");
        return claim != null && bool.TryParse(claim.Value, out var isGuest) && isGuest;
    }
}
