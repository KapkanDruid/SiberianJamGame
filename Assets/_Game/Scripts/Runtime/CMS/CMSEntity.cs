using System;
using System.Collections.Generic;

namespace Game.Runtime.CMS
{
    public class CMSEntity
    {
        public string EntityId { get; private set; }
        private readonly List<CMSComponent> _components;

        public CMSEntity(string entityId, List<CMSComponent> components = null)
        {
            EntityId = entityId;
            _components = components ?? new List<CMSComponent>();
        }

        public T Define<T>() where T : CMSComponent, new()
        {
            var component = GetComponent<T>();
            if (component != null)
                return component;

            component = new T();
            _components.Add(component);

            return component;
        }

        public bool Is<T>(out T unknown) where T : CMSComponent, new()
        {
            unknown = GetComponent<T>();
            return unknown != null;
        }

        public bool Is<T>() where T : CMSComponent, new()
        {
            return GetComponent<T>() != null;
        }

        public bool Is(Type type)
        {
            return _components.Find(m => m.GetType() == type) != null;
        }

        public T GetComponent<T>() where T : CMSComponent, new()
        {
            return _components.Find(m => m is T) as T;
        }

        public List<CMSComponent> GetAll()
        {
            return _components;
        }
    }
}