using System;
using System.Linq;
using System.Reflection;

namespace Game.Runtime.Utils.Helpers
{
    public static partial class Helpers
    {
        public static class ReflectionHelper
        {
            public static Type[] FindAllSubsClasses<T>()
            {
                Type baseType = typeof(T);
                Assembly assembly = Assembly.GetAssembly(baseType);

                Type[] types = assembly.GetTypes();
                Type[] subclasses = types.Where(type => type.IsSubclassOf(baseType) && !type.IsAbstract).ToArray();

                return subclasses;
            }

            public static Type[] FindAllImplementations<TInterface>() where TInterface : class
            {
                Type interfaceType = typeof(TInterface);
                Assembly assembly = Assembly.GetAssembly(interfaceType);

                Type[] types = assembly.GetTypes();
                Type[] implementations = types.Where(type => 
                    !type.IsAbstract && 
                    !type.IsInterface && 
                    type.GetInterfaces().Contains(interfaceType)).ToArray();

                return implementations;
            }
        }
    }
}