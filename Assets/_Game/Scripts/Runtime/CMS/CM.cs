using System;
using System.Collections.Generic;
using System.Text;
using Game.Runtime.Utils;
using Game.Runtime.Utils.Helpers;
using UnityEngine;

namespace Game.Runtime.CMS
{
    public static class CM
    {
        private static bool _loaded;
        private static CMSTable<CMSEntity> _entitiesDatabase;

        public static void Load()
        {
            if (_loaded) return;

            AutoCacheEntities();
            _loaded = true;
        }

        public static void Unload()
        {
            _loaded = false;
            _entitiesDatabase = new CMSTable<CMSEntity>();
        }
        
        public static void Reload()
        {
            Unload();
            Load();
        }

        public static CMSEntity Get(string entityId)
        {
            var entity = _entitiesDatabase.GetEntityOrDefault(entityId);

            if (entity == default)
            {
                LogUtil.LogError(nameof(CM),$"Unable to resolve entity id '{entityId}");
                throw new NullReferenceException();
            }

            return entity;
        }

        public static CMSEntity Get<T>() where T : CMSEntity
        {
            var entityId = typeof(T).FullName;
            var entity = _entitiesDatabase.GetEntityOrDefault(entityId);

            if (entity == default)
            {
                LogUtil.LogError(nameof(CM),$"Unable to resolve entity id '{entityId}");
                throw new NullReferenceException();
            }

            return entity;
        }

        public static List<CMSEntity> GetAll<T>() where T : CMSComponent, new()
        {
            var filteredEntities = new List<CMSEntity>();

            foreach (var entity in _entitiesDatabase.GetAll())
            {
                if (entity.Is<T>(out var component))
                    filteredEntities.Add(entity);
            }

            return filteredEntities;
        }

        private static void AutoCacheEntities()
        {
            _entitiesDatabase = new CMSTable<CMSEntity>();
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("AutoCachedEntities:");
            
            var entities = Helpers.ReflectionHelper.FindAllSubsClasses<CMSEntity>();
            foreach (var entity in entities)
            {
                try
                {
                    CMSEntity entityInstance = Activator.CreateInstance(entity, entity.FullName, null) as CMSEntity;
                    _entitiesDatabase.Add(entityInstance);

                    if (entityInstance != null) stringBuilder.Append($"\n{entityInstance.EntityId}");
                }
                catch (Exception exception)
                {
                    LogUtil.LogError(nameof(CM),$"Failed to initialize {entity.Name}: {exception.Message}");
                }
            }

            var entityPrefabs = Resources.LoadAll<CMSPrefab>("");
            foreach (var entityPrefab in entityPrefabs)
            {
                try
                {
#if UNITY_EDITOR
                    entityPrefab.PingEntity();
#endif
                    stringBuilder.Append($"\n{entityPrefab.EntityId}");

                    var entity = new CMSEntity(entityPrefab.EntityId, entityPrefab.Components);
                    _entitiesDatabase.Add(entity);
                }
                catch (Exception exception)
                {
                    LogUtil.LogError(nameof(CM),$"Failed to initialize {entityPrefab.EntityId}: {exception.Message}");
                }
            }
            
            LogUtil.Log(nameof(CM), stringBuilder + "\n", Color.green);
        }
    }
}