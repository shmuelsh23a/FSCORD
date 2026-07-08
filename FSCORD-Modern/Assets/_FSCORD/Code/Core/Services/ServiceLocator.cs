using System;
using System.Collections.Generic;

namespace FSCORD.Core
{
    /// <summary>
    /// Minimal typed service registry used as the composition root. Services are
    /// created and registered once in GameBootstrap, then handed to systems -
    /// replacing the 2015 code's scattered static singletons.
    /// </summary>
    public sealed class ServiceLocator
    {
        readonly Dictionary<Type, object> _services = new();

        public void Register<T>(T service) where T : class => _services[typeof(T)] = service;

        public T Get<T>() where T : class
        {
            if (_services.TryGetValue(typeof(T), out var s)) return (T)s;
            throw new InvalidOperationException($"Service not registered: {typeof(T).Name}");
        }

        public bool TryGet<T>(out T service) where T : class
        {
            if (_services.TryGetValue(typeof(T), out var s)) { service = (T)s; return true; }
            service = null; return false;
        }
    }
}
