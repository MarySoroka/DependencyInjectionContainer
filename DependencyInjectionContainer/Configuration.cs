using System;

namespace DependencyInjectionContainer
{
    public class ConfigurationType
    {
        public bool IsSingleton { get; }

        public Type Interface { get; }

        public Type Implementation { get; }

        public ConfigurationType(Type inter, Type implementation, bool isSingleton = false)
        {
            Interface = inter;
            Implementation = implementation;
            IsSingleton = isSingleton;
        }
    }
}