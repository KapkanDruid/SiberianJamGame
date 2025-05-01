using System.Collections.Generic;

namespace Game.Runtime.CMS
{
    public class CMSTable<T> where T : CMSEntity
    {
        private readonly List<T> list = new();
        private readonly Dictionary<string, T> dict = new();

        public void Add(T inst)
        {
            list.Add(inst);
            dict.Add(inst.EntityId, inst);
        }

        public List<T> GetAll()
        {
            return list;
        }

        public T GetEntityOrDefault(string id)
        {
            return dict.GetValueOrDefault(id);
        }
    }
}