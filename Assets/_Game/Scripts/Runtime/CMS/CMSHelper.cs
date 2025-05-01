namespace Game.Runtime.CMS
{
    public static class CMSHelper
    {
        public static string GetEntityId<T>() where T : CMSEntity
        {
            return typeof(T).FullName;
        }
    }
}