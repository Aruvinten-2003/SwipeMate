using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace SwipeMate.Api.HealthChecks;

public sealed class DatabaseHealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(HealthCheckResult.Healthy("Database health check placeholder is healthy."));
    }
}
