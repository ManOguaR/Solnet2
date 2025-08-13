using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Solnet.Core.Exceptions;
using Solnet.Core.Monitoring;
using Solnet.Core.UsageTracking;

namespace Solnet.Core.Configuration
{
    /// <summary>
    /// Standard registration extensions, which are used to configure consumers, sagas, and activities on receive endpoints from a
    /// dependency injection container.
    /// </summary>
    public static class DependencyInjectionRegistrationExtensions
    {
        /// <summary>
        /// Adds SolnetCore and its dependencies to the <paramref name="collection" />, and allows consumers, sagas, and activities to be configured
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="configure"></param>
        public static IServiceCollection AddSolnetCore(this IServiceCollection collection, Action<ISolnetCoreConfigurator>? configure = null)
        {
            if (collection.Any(d => d.ServiceType == typeof(ISolnet)))
            {
                throw new ConfigurationException(
                    "AddSolnetCore() was already called and may only be called once per container. To configure additional bus instances, refer to the documentation: https://masstransit-project.com/usage/containers/multibus.html");
            }

            AddHostedService(collection);
            AddInstrumentation(collection);
            AddUsageTracker(collection);

            var configurator = new ServiceCollectionSolnetConfigurator(collection);

            configure?.Invoke(configurator);

            configurator.Complete();

            return collection;
        }

        ///// Adds the SolnetCore Mediator to the <paramref name="collection" />, and allows consumers, sagas, and activities (which are not supported
        ///// by the Mediator) to be configured.
        ///// </summary>
        ///// <param name="collection"></param>
        ///// <param name="configure"></param>
        ///// <param name="baseAddress"></param>
        //public static IServiceCollection AddMediator(this IServiceCollection collection, Uri baseAddress,
        //    Action<IMediatorRegistrationConfigurator> configure = null)
        //{
        //    if (collection.Any(d => d.ServiceType == typeof(IMediator)))
        //        throw new ConfigurationException("AddMediator() was already called and may only be called once per container.");

        //    var configurator = new ServiceCollectionMediatorConfigurator(collection, baseAddress);

        //    configure?.Invoke(configurator);

        //    AddInstrumentation(collection);

        //    configurator.Complete();

        //    return collection;
        //}

        ///// <summary>
        ///// Adds the SolnetCore Mediator to the <paramref name="collection" />, and allows consumers, sagas, and activities (which are not supported
        ///// by the Mediator) to be configured.
        ///// </summary>
        ///// <param name="collection"></param>
        ///// <param name="configure"></param>
        //public static IServiceCollection AddMediator(this IServiceCollection collection, Action<IMediatorRegistrationConfigurator> configure = null)
        //{
        //    return AddMediator(collection, null, configure);
        //}

        ///// <summary>
        ///// Configure a SolnetCore bus instance, using the specified <typeparamref name="TSolnet" /> bus type, which must inherit directly from <see cref="ISolnet" />.
        ///// A type that implements <typeparamref name="TSolnet" /> is required, specified by the <typeparamref name="TSolnetInstance" /> parameter.
        ///// </summary>
        ///// <param name="collection">The service collection</param>
        ///// <param name="configure">Solnet instance configuration method</param>
        //public static IServiceCollection AddSolnetCore<TSolnet, TSolnetInstance>(this IServiceCollection collection,
        //    Action<ISolnetRegistrationConfigurator<TSolnet>> configure)
        //    where TSolnet : class, ISolnet
        //    where TSolnetInstance : SolnetInstance<TSolnet>, TSolnet
        //{
        //    if (configure == null)
        //        throw new ArgumentNullException(nameof(configure));

        //    if (collection.Any(d => d.ServiceType == typeof(TSolnet)))
        //    {
        //        throw new ConfigurationException(
        //            $"AddSolnetCore<{typeof(TSolnet).Name},{typeof(TSolnetInstance).Name}>() was already called and may only be called once per container. To configure additional bus instances, refer to the documentation: https://masstransit-project.com/usage/containers/multibus.html");
        //    }

        //    AddHostedService(collection);
        //    AddInstrumentation(collection);

        //    var configurator = new ServiceCollectionSolnetConfigurator<TSolnet, TSolnetInstance>(collection);

        //    configure?.Invoke(configurator);

        //    configurator.Complete();

        //    return collection;
        //}

        ///// <summary>
        ///// Configure a SolnetCore MultiSolnet instance, using the specified <typeparamref name="TSolnet" /> bus type, which must inherit directly from <see cref="ISolnet" />.
        ///// A dynamic type will be created to support the bus instance, which will be initialized when the <typeparamref name="TSolnet" /> type is retrieved
        ///// from the container.
        ///// </summary>
        ///// <param name="collection">The service collection</param>
        ///// <param name="configure">Solnet instance configuration method</param>
        //public static IServiceCollection AddSolnetCore<TSolnet>(this IServiceCollection collection, Action<ISolnetRegistrationConfigurator<TSolnet>> configure)
        //    where TSolnet : class, ISolnet
        //{
        //    if (configure == null)
        //        throw new ArgumentNullException(nameof(configure));

        //    var doIt = new Callback<TSolnet>(collection, configure);

        //    SolnetInstanceBuilder.Instance.GetSolnetInstanceType(doIt);

        //    return collection;
        //}

        ///// <summary>
        ///// In some situations, it may be necessary to Remove the SolnetCoreHostedService from the container, such as
        ///// when using older versions of the Azure Functions runtime.
        ///// </summary>
        ///// <param name="services"></param>
        ///// <returns></returns>
        //public static IServiceCollection RemoveSolnetCoreHostedService(this IServiceCollection services)
        //{
        //    return RemoveHostedService<SolnetCoreHostedService>(services);
        //}

        ///// <summary>
        ///// Remove the specified hosted service from the service collection
        ///// </summary>
        ///// <param name="services"></param>
        ///// <returns></returns>
        //public static IServiceCollection RemoveHostedService<T>(this IServiceCollection services)
        //    where T : IHostedService
        //{
        //    var descriptor = services.FirstOrDefault(x => x.ServiceType == typeof(IHostedService) && x.ImplementationType == typeof(T));
        //    if (descriptor != null)
        //        services.Remove(descriptor);

        //    return services;
        //}

        ///// <summary>
        ///// Replace a scoped service registration with a new one
        ///// </summary>
        ///// <typeparam name="TService"></typeparam>
        ///// <typeparam name="TImplementation"></typeparam>
        //public static void ReplaceScoped<TService, TImplementation>(this IServiceCollection services)
        //    where TService : class
        //    where TImplementation : class, TService
        //{
        //    services.Replace(new ServiceDescriptor(typeof(TService), typeof(TImplementation), ServiceLifetime.Scoped));
        //}

        static void AddInstrumentation(IServiceCollection collection)
        {
            collection.AddOptions<InstrumentationOptions>();
            collection.AddSingleton<IConfigureOptions<InstrumentationOptions>, ConfigureDefaultInstrumentationOptions>();
        }

        static void AddUsageTracker(IServiceCollection collection)
        {
            collection.AddOptions<UsageTelemetryOptions>();
            collection.TryAddSingleton<IUsageTracker, UsageTracker>();
        }

        static void AddHostedService(IServiceCollection collection)
        {
            collection.AddOptions();
            collection.AddHealthChecks();
            collection.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigureOptions<HealthCheckServiceOptions>, ConfigureSolnetHealthCheckServiceOptions>());

            collection.AddOptions<SolnetCoreHostOptions>();
            collection.TryAddSingleton<IValidateOptions<SolnetCoreHostOptions>, ValidateSolnetCoreHostOptions>();
            collection.TryAddEnumerable(ServiceDescriptor.Singleton<IHostedService, SolnetCoreHostedService>());
        }

        //internal static void RemoveSolnetCore(this IServiceCollection collection)
        //{
        //    collection.RemoveAll<IClientFactory>();
        //    collection.RemoveAll<Bind<ISolnet, ISolnetRegistrationContext>>();
        //    collection.RemoveAll<ISolnetRegistrationContext>();
        //    collection.RemoveAll(typeof(IReceiveEndpointDispatcher<>));
        //    collection.RemoveAll<IReceiveEndpointDispatcherFactory>();


        //    collection.RemoveAll<ISolnetDepot>();
        //    collection.RemoveAll<IScopedConsumeContextProvider>();
        //    collection.RemoveAll<Bind<ISolnet, ISetScopedConsumeContext>>();
        //    collection.RemoveAll<Bind<ISolnet, IScopedConsumeContextProvider>>();
        //    collection.RemoveAll<IScopedSolnetContextProvider<ISolnet>>();
        //    collection.RemoveAll<ConsumeContext>();
        //    collection.RemoveAll<ISendEndpointProvider>();
        //    collection.RemoveAll<IPublishEndpoint>();
        //    collection.RemoveAll(typeof(IRequestClient<>));
        //    collection.RemoveAll<IMessageScheduler>();

        //    collection.RemoveAll<Bind<ISolnet, ISolnetInstance>>();
        //    collection.RemoveAll<ISolnetInstance>();
        //    collection.RemoveAll<IReceiveEndpointConnector>();
        //    collection.RemoveAll<ISolnetControl>();
        //    collection.RemoveAll<ISolnet>();

        //    collection.RemoveAll<IScopedClientFactory>();
        //}


        //class Callback<TSolnet> :
        //    ISolnetInstanceBuilderCallback<TSolnet, IServiceCollection>
        //    where TSolnet : class, ISolnet
        //{
        //    readonly Action<ISolnetRegistrationConfigurator<TSolnet>> _configure;
        //    readonly IServiceCollection _services;

        //    public Callback(IServiceCollection services, Action<ISolnetRegistrationConfigurator<TSolnet>> configure)
        //    {
        //        _services = services;
        //        _configure = configure;
        //    }

        //    public IServiceCollection GetResult<TSolnetInstance>()
        //        where TSolnetInstance : SolnetInstance<TSolnet>, TSolnet
        //    {
        //        return _services.AddSolnetCore<TSolnet, TSolnetInstance>(_configure);
        //    }
        //}
    }
}
