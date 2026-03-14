using AinaLabs.Core.Interfaces;
using Microsoft.AspNetCore.Http;

namespace AinaLabs.Infraestructure;

public class TenantService : ITenantService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TenantService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    public Guid GetTenantId()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
        {
            throw new InvalidOperationException("No HTTP context available");
        }

        var tenantClaim = httpContext.User.FindFirst("tenant_id")?.Value;
        
        if (string.IsNullOrEmpty(tenantClaim) || !Guid.TryParse(tenantClaim, out var tenantId))
        {
            throw new UnauthorizedAccessException("Tenant ID is missing or invalid");
        }
        
        return tenantId;
    }
}