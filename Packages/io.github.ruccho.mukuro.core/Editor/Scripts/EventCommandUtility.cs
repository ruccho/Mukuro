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
            IEnumerable<Type> commandTypes = assemblies.SelectMany(a => a.GetTypes()).Where(t => t.IsSubclassOf(typeof(EventCommand)) && !t.IsAbstract);
            /*
            foreach (var assembly in assemblies)
            {
                var typesInAssembly =
                    assembly
                        .GetTypes().Where(t => t.IsSubclassOf(typeof(EventCommand)) && !t.IsAbstract);

                if (commandTypes == null)
                {
                    commandTypes = typesInAssembly;
                }
                else
                {
                    commandTypes = commandTypes.Concat(typesInAssembly);
                }
            }*/

            return commandTypes;
        }

        public static Type GetEventCommandType(string fullName)
        {
            var commandTypes = GetAllEventCommandTypes();
            return commandTypes.FirstOrDefault(t => t.FullName == fullName);
        }
        
        //<Type (コマンドタイプ), Type (コマンドエディタタイプ)>
        private static readonly Dictionary<Type, Type> commandEditorTypeDictionary = new Dictionary<Type, Type>();

        public static Type GetCommandEditorType(Type commandType)
        {
            if (commandEditorTypeDictionary.ContainsKey(commandType)) return commandEditorTypeDictionary[commandType];
            
            var editorTypes = GetAllCommandEditorTypes();

            //Attributeで選別
            foreach (var editorType in editorTypes)
            {
                IEnumerable<CustomEventCommandEditorAttribute> attributes =
                    editorType.GetCustomAttributes(typeof(CustomEventCommandEditorAttribute), false).OfType<CustomEventCommandEditorAttribute>();
                if (attributes.Any())
                {
                    //該当TypeのAttributeを取得
                    var matches = attributes.Where((a) =>
                        (a as CustomEventCommandEditorAttribute).CommandType == commandType);
                    if (matches.Any())
                    {
                        commandEditorTypeDictionary[commandType] = editorType;
                        return editorType;
                    }
                }
            }

            return null;
        }

        private static Assembly[] cachedAssembliesReferencingMukuroEditor = default;
        private static IEnumerable<Assembly> GetAssembliesReferencingMukuroEditor()
        {
            if (cachedAssembliesReferencingMukuroEditor == null)
            {
                //Mukuro.Editorを参照しているアセンブリを取得（ぜんぶ取得すると重いので）
                var mukuroEditorAssemblyFullName = Assembly.GetAssembly(typeof(EventCommandEditor)).FullName;
                var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(a =>
                    a.GetReferencedAssemblies()
                        .FirstOrDefault(
                            n => n.FullName == mukuroEditorAssemblyFullName
                        ) != default || a.FullName == mukuroEditorAssemblyFullName
                );

                cachedAssembliesReferencingMukuroEditor = assemblies.ToArray();
            }

            return cachedAssembliesReferencingMukuroEditor;
        }

        private static Type[] cachedAllCommandEditorTypes = default;
        private static IEnumerable<Type> GetAllCommandEditorTypes()
        {
            if (cachedAllCommandEditorTypes == null)
            {
                var assemblies = GetAssembliesReferencingMukuroEditor();

                //EventCommandEditorを継承したクラスを取得
                IEnumerable<Type> editorTypes = null;
                foreach (var assembly in assemblies)
                {
                    var typesInAssembly =
                        assembly
                            .GetTypes().Where(t =>
                            {
                                return t.IsSubclassOf(typeof(EventCommandEditor)) && !t.IsAbstract;
                            });

                    if (editorTypes == null)
                    {
                        editorTypes = typesInAssembly;
                    }
                    else
                    {
                        editorTypes = editorTypes.Concat(typesInAssembly);
                    }
                }

                cachedAllCommandEditorTypes = editorTypes.ToArray();
            }

            return cachedAllCommandEditorTypes;
        }
    }
}