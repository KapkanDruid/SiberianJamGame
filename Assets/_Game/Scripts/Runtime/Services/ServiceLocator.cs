using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Runtime.Services
{
    public static class ServiceLocator
    {
        private static Dictionary<Scope, Dictionary<Type, object>> _scopes;

        public static void Register<T>(IService service, Scope scope) where T : IService
        {
            _scopes ??= new Dictionary<Scope, Dictionary<Type, object>>();
            
            var type = typeof(T);

            var scopeServices = GetScopeServices(scope);
            scopeServices.TryAdd(type, service);
        }
        
        public static T Get<T>() where T : IService
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

        public static void InitializeScope(Scope scope)
        {
            var scopeServices = GetScopeServices(scope);
            foreach (var service in scopeServices)
            {
                if (service.Value is IInitializable initialize)
                    initialize.Initialize();
            }
        }
        
        public static void DisposeScope(Scope scope)
        {
            if (_scopes == null) return;
            
            if (!_scopes.TryGetValue(scope, out var scopeServices))
                return;

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
        
        private static Dictionary<Type, object> GetScopeServices(Scope scope)
        {
            if (_scopes.TryGetValue(scope, out var services))
                return services;

            var newScope = new Dictionary<Type, object>();
            _scopes.Add(scope, newScope);
            
            return newScope;
        }
    }
}