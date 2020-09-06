using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;


namespace Mukuro.Editors
{
    public static class EventCommandUtility
    {
        /// <summary>
        /// EventCommandを参照しているAssemblyの一覧
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Assembly> GetAssemblies()
        {
            var mukuroAssemblyFullName = Assembly.GetAssembly(typeof(EventCommand)).FullName;
            return AppDomain.CurrentDomain.GetAssemblies().Where(a =>
                a.GetReferencedAssemblies()
                    .FirstOrDefault(
                        n => n.FullName == mukuroAssemblyFullName
                    ) != default || a.FullName == mukuroAssemblyFullName
            );
        }

        public static IEnumerable<Type> GetEventCommandTypes(Assembly assembly)
        {
            IEnumerable<Type> commandTypes =
                assembly
                    .GetTypes().Where(t => { return t.IsSubclassOf(typeof(EventCommand)) && !t.IsAbstract; });
            return commandTypes;
        }
        
        
        public static IEnumerable<Type> GetAllEventCommandTypes()
        {
            var assemblies = GetAssemblies();
            IEnumerable<Type> commandTypes = null;
            foreach (var assembly in assemblies)
            {
                var typesInAssembly = 
                    assembly
                        .GetTypes().Where(t => { return t.IsSubclassOf(typeof(EventCommand)) && !t.IsAbstract; });
                
                if (commandTypes == null)
                {
                    commandTypes = typesInAssembly;
                }
                else
                {
                    commandTypes = commandTypes.Concat(typesInAssembly);
                }
            }
            
            return commandTypes;
        }

        public static Type GetEventCommandType(string fullName)
        {
            var commandTypes = GetAllEventCommandTypes();
            return commandTypes.FirstOrDefault(t => t.FullName == fullName);
        }
    }
}