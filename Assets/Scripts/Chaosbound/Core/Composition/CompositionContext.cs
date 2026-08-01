using Chaosbound.Content.Expeditions.Runtime.Configs;
using System;
using System.Collections.Generic;

namespace Chaosbound.Core.Composition
{
    /// <summary>
    /// Shared context used during world composition.
    /// Provides access to the runtime expedition configuration
    /// and to the objects created throughout the composition pipeline.
    /// </summary>
    public sealed class CompositionContext
    {
        private readonly Dictionary<Type, object> _registry = new();

        public RunSession RunSession { get; }

        public RuntimeExpeditionConfig RuntimeConfig { get; }

        public CompositionContext(
            RunSession runSession,
            RuntimeExpeditionConfig runtimeConfig)
        {
            RunSession = runSession
                ?? throw new ArgumentNullException(nameof(runSession));

            RuntimeConfig = runtimeConfig
                ?? throw new ArgumentNullException(nameof(runtimeConfig));
        }

        /// <summary>
        /// Registers a single instance for the specified type.
        /// Only one instance per type is allowed.
        /// </summary>
        public void Register<T>(T instance)
            where T : class
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            Type type = typeof(T);

            if (_registry.ContainsKey(type))
            {
                throw new InvalidOperationException(
                    $"Type '{type.Name}' is already registered.");
            }

            _registry.Add(type, instance);
        }

        /// <summary>
        /// Resolves a previously registered instance.
        /// Throws if the type has not been registered.
        /// </summary>
        public T Resolve<T>()
            where T : class
        {
            Type type = typeof(T);

            if (_registry.TryGetValue(type, out object instance))
            {
                return (T)instance;
            }

            throw new InvalidOperationException(
                $"Type '{type.Name}' has not been registered.");
        }

        /// <summary>
        /// Attempts to resolve a registered instance.
        /// </summary>
        public bool TryResolve<T>(out T instance)
            where T : class
        {
            if (_registry.TryGetValue(typeof(T), out object value))
            {
                instance = (T)value;
                return true;
            }

            instance = null;
            return false;
        }

        /// <summary>
        /// Returns true if the specified type has already been registered.
        /// </summary>
        public bool IsRegistered<T>()
            where T : class
        {
            return _registry.ContainsKey(typeof(T));
        }

        /// <summary>
        /// Clears all registered instances from the context.
        /// </summary>
        public void Clear()
        {
            _registry.Clear();
        }
    }
}