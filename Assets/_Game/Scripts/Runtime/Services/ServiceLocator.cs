using System;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime._Game.Scripts.Runtime.Services
{
    public static class ServiceLocator
    {
        private static Dictionary<ServiceScope, Dictionary<Type, object>> _scopes;

        public static void RegisterService<T>(IService service, ServiceScope scope) where T : IService
        {
            _scopes ??= new Dictionary<ServiceScope, Dictionary<Type, object>>();
            
            var type = typeof(T);

            var scopeServices = GetScopeServices(scope);
            if (scopeServices.TryAdd(type, service))
            {
                Debug.Log($"[ServiceLocator] {scope}. Registered {type.Name}.");
            }
            else
            {
                Debug.LogError($"[ServiceLocator] Cannot registered {type.Name}");
            }
        }

        public static void InitializeServices(ServiceScope scope)
        {
            var scopeServices = GetScopeServices(scope);
            foreach (var service in scopeServices)
            {
                if (service.Value is IInitializable initialize)
                    initialize.Initialize();
            }
        }

        public static T GetService<T>() where T : IService
        {
            var type = typeof(T);
            foreach (var scope in _scopes)
            {
                if (scope.Value.TryGetValue(type, out object service))
                    return (T)service;
            }
            
            Debug.LogError($"[ServiceLocator] Cannot resolve type {type.Name}");
            return default;
        }
        
        public static void UnregisterService<T>(ServiceScope scope) where T : IService
        {
            var type = typeof(T);
            var scopeServices = GetScopeServices(scope);

            if (scopeServices.ContainsKey(type))
            {
                scopeServices.Remove(type);
                Debug.Log($"[ServiceLocator] Unregistered {type.Name}");
            }
            else
            {
                Debug.LogWarning($"[ServiceLocator] Cannot unregistered {type.Name}");
            }
        }

        private static Dictionary<Type, object> GetScopeServices(ServiceScope scope)
        {
            if (_scopes.TryGetValue(scope, out var services))
                return services;

            var newScope = new Dictionary<Type, object>();
            _scopes.Add(scope, newScope);
            
            Debug.Log($"[ServiceLocator] {scope} registered.");
            return newScope;
        }
        
        public static void DisposeScope(ServiceScope scope)
        {
            if (_scopes == null) return;
            
            if (!_scopes.TryGetValue(scope, out var scopeServices))
            {
                Debug.LogWarning($"[ServiceLocator] {scope} is null. Cannot dispose.");
                return;
            }

            foreach (var service in scopeServices)
            {
                if (service.Value is IDisposable disposable)
                    disposable.Dispose();
            }

            _scopes.Remove(scope);
        }

        public static void Clear()
        {
            foreach (var scope in _scopes)
            {
                foreach (var service in scope.Value)
                {
                    if (service.Value is IDisposable disposable)
                        disposable.Dispose();
                }
            }
            
            _scopes.Clear();
            _scopes = null;
        }
    }
}