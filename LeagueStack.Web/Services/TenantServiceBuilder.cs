using LeagueStack.Abstractions;

namespace LeagueStack.Web.Services;

public class TenantServiceRegistry
{
    private readonly Dictionary<(string TenantId, Type ServiceType), Type> _map = [];

    public void Register(string tenantId, Type serviceType, Type implementationType)
    {
        _map[(tenantId, serviceType)] = implementationType;
    }

    public Type? GetImplementationType(string tenantId, Type serviceType)
    {
        return _map.GetValueOrDefault((tenantId, serviceType));
    }

    public IReadOnlyDictionary<Type, Type> DefaultImplementations { get; } = new Dictionary<Type, Type>();

    public void RegisterDefault(Type serviceType, Type implementationType)
    {
        ((Dictionary<Type, Type>)DefaultImplementations)[serviceType] = implementationType;
    }

    public void RegisterDefault<TService, TImplementation>()
    {
        RegisterDefault(typeof(TService), typeof(TImplementation));
    }

    public IEnumerable<Type> OverriddenServiceTypes
    {
        get
        {
            return _map.Keys.Select(k => k.ServiceType).Distinct();
        }
    }
}

public class TenantServiceBuilder(string tenantId, TenantServiceRegistry registry) : ITenantServiceBuilder
{
    private readonly string _tenantId = tenantId;
    private readonly TenantServiceRegistry _registry = registry;

    public ITenantServiceBuilder AddScoped<TService, TImplementation>()
        where TService : class
        where TImplementation : class, TService
    {
        _registry.Register(_tenantId, typeof(TService), typeof(TImplementation));
        return this;
    }
}
