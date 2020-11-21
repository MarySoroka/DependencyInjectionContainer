using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DependencyInjectionContainer
{
public class DependencyProvider
    {
        private readonly DependencyConfiguration _config;
        private readonly ConcurrentDictionary<Type, object> _implementationInstances = new ConcurrentDictionary<Type, object>();

        public DependencyProvider(DependencyConfiguration config)
        {
            _config = config;
        }

        public TDependency Resolve<TDependency>()
        {
            return (TDependency)GetInstance(typeof(TDependency));
        }

        private object GetInstance(Type dependencyType)
        {
            if (typeof(IEnumerable).IsAssignableFrom(dependencyType) && dependencyType.IsGenericType)
            {
                var collection = CreateIEnumerable(dependencyType);
                return collection;
            }

            if (!dependencyType.IsGenericType || _config.GetConfigurationType(dependencyType) != null)
                return _config.GetConfigurationType(dependencyType) != null
                    ? Create(_config.GetConfigurationType(dependencyType))
                    : null;
            var implementation = ImplementationsForOpenGeneric(dependencyType);
            var configuredType = _config.GetConfigurationType(dependencyType.GetGenericTypeDefinition());
            return Create(configuredType, implementation);

        }

        private object CreateIEnumerable(Type type)
        {
            var argType = type.GetGenericArguments()[0];
            var configuredType = _config.GetConfigurationType(argType);
            if (configuredType == null) return null;
            var collection = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(argType));

            var configuredTypes = _config.GetConfigurationTypes(argType);
            foreach (var confType in configuredTypes)
            {
                collection?.Add(Create(confType));
            }

            return collection;
        }

        private object Create(ConfigurationType configType, Type extraImpl=null)
        {
            var implementation = extraImpl == null ? configType.Implementation : extraImpl;

            if (configType.IsSingleton && _implementationInstances.ContainsKey(implementation))
                return _implementationInstances[implementation];

            var constructors = implementation.GetConstructors().OrderByDescending(x => x.GetParameters().Length).ToArray();
            object resultObject = null;
            var isCreated = false;
            var ctorNum = 1;
            while (!isCreated && ctorNum <= constructors.Length)
            {
                try
                {
                    var useConstructor = constructors[ctorNum - 1];
                    var parameters = GetConstructorParams(useConstructor);
                    resultObject = Activator.CreateInstance(implementation, parameters);
                    isCreated = true;
                }
                catch(Exception)
                {
                    isCreated = false;
                    ctorNum++; 
                }
            }

            if (!configType.IsSingleton || _implementationInstances.ContainsKey(implementation)) return resultObject;
            return !_implementationInstances.TryAdd(implementation, resultObject) ? _implementationInstances[implementation] : resultObject;
        }

        private Type ImplementationsForOpenGeneric(Type dependencyType)
        {
            var arg = dependencyType.GetGenericArguments().FirstOrDefault();
            var implementation = _config.GetConfigurationType(dependencyType.GetGenericTypeDefinition()).Implementation;
            return implementation != null ? implementation.MakeGenericType(arg!) : null;
            
        }


        private object[] GetConstructorParams(MethodBase constructor)
        {
            var parameters = constructor.GetParameters();
            var parametersValues = new object[parameters.Length];

            for (var i = 0; i < parameters.Length; i++)
            {
                var param = _config.GetConfigurationType(parameters[i].ParameterType);
                var paramType = param != null ? param.Interface : parameters[i].ParameterType;
                parametersValues[i] = GetInstance(paramType);
                
            }

            return parametersValues;
        }

    }
}