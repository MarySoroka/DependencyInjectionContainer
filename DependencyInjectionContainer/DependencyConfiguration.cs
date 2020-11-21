using System;
using System.Collections.Generic;
using System.Linq;

namespace DependencyInjectionContainer
{
  public class DependencyConfiguration
    {
        private readonly Dictionary<Type, List<ConfigurationType>> _configuration= new Dictionary<Type, List<ConfigurationType>>();

        public void Register<TInterface, TImplementation>()
            where TInterface : class
            where TImplementation :TInterface
        {
            RegisterType(typeof(TInterface), typeof(TImplementation));
        }

        public void Register(Type @interface, Type implementation)
        {
            RegisterType(@interface, implementation);
        }

        public void RegisterSingleton<TInterface, TImplementation>()
            where TInterface : class
            where TImplementation : TInterface
        {
            RegisterType(typeof(TInterface), typeof(TImplementation), true);
        }

        public void RegisterSingleton(Type @interface, Type implementation)
        {
            RegisterType(@interface, implementation, true);
        }

        private void RegisterType(Type @interface, Type implementation, bool isSingleton = false)
        {
            if (!IsValid(@interface, implementation)) return;
            var configuredType = new ConfigurationType(@interface, implementation, isSingleton);

            if (_configuration.ContainsKey(@interface))
            {
                _configuration[@interface].Add(configuredType);
            }
            else
            {
                _configuration.Add(@interface, new List<ConfigurationType> { configuredType });
            }
        }

        public ConfigurationType GetConfigurationType(Type @interface)
        {
            return _configuration.TryGetValue(@interface, out var configuredTypes) ? configuredTypes.Last() : null;
        }

        public IEnumerable<ConfigurationType> GetConfigurationTypes(Type @interface)
        {
            return _configuration.TryGetValue(@interface, out var configuredTypes) ? configuredTypes : null;
        }

        private static bool IsValid(Type dependencyType, Type implementation)
        {
            return !implementation.IsAbstract && !implementation.IsInterface&&
                    (dependencyType.IsAssignableFrom(implementation) || dependencyType.IsGenericTypeDefinition);  
        }
    }
}