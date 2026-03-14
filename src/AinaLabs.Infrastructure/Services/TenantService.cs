using AinaLabs.Core.Interfaces;
using Microsoft.AspNetCore.Http;

namespace AinaLabs.Infrastructure.Services;

public class TenantService : ITenantService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    
    private readonly Guid _mockTenantId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    public TenantService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    public Guid GetTenantId()
    {
        return _mockTenantId;
        
        // Todo: cuando implementemos el login ...
    //     var httpContext = _httpContextAccessor.HttpContext;
    //     if (httpContext == null)
    //     {
    //         throw new InvalidOperationException("No HTTP context available");
    //     }
    //
    //     var tenantClaim = httpContext.User.FindFirst("tenant_id")?.Value;
    //     
    //     if (string.IsNullOrEmpty(tenantClaim) || !Guid.TryParse(tenantClaim, out var tenantId))
    //     {
    //         throw new UnauthorizedAccessException("Tenant ID is missing or invalid");
    //     }
    //     
    //     return tenantId;
    }
}