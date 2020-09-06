using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.LowLevel;

namespace Mukuro
{
    /// <summary>
    /// PlayerLoopのコピーに変更を行い、Dispose()のタイミングで有効化します。
    /// usingと併せて使うのがおすすめ。
    /// </summary>
    public class PlayerLoopModifier : IDisposable
    {
        private PlayerLoopSystem root;

        public PlayerLoopModifier()
        {
            root = PlayerLoop.GetCurrentPlayerLoop();
        }

        /// <summary>
        /// 任意のPlayerLoopSystemの直後に新しくPlayerLoopSystemを挿入する。
        /// </summary>
        /// <param name="subSystem">追加するPlayerLoopSystem。</param>
        /// <typeparam name="T">Tで指定されたtypeのPlayerLoopSystemの直後に追加します。</typeparam>
        /// <returns>追加が成功したかどうか。</returns>
        public bool InsertAfter<T>(PlayerLoopSystem subSystem) where T : struct
        {
            return InsertAfter<T>(subSystem, ref root);
        }

        /// <summary>
        /// 任意のPlayerLoopSystemの直前に新しくPlayerLoopSystemを挿入する。
        /// </summary>
        /// <param name="subSystem">追加するPlayerLoopSystem。</param>
        /// <typeparam name="T">Tで指定されたtypeのPlayerLoopSystemの直前に追加します。</typeparam>
        /// <returns>追加が成功したかどうか。</returns>
        public bool InsertBefore<T>(PlayerLoopSystem subSystem) where T : struct
        {
            return InsertBefore<T>(subSystem, ref root);
        }

        /// <summary>
        /// 任意のPlayerLoopSystemの子に新しくPlayerLoopSystemを挿入する。
        /// </summary>
        /// <param name="subSystem">追加するPlayerLoopSystem。</param>
        /// <typeparam name="T">Tで指定されたtypeのPlayerLoopSystemの子に追加します。</typeparam>
        /// <returns>追加が成功したかどうか。</returns>
        public bool InsertIn<T>(PlayerLoopSystem subSystem) where T : struct
        {
            return InsertIn<T>(subSystem, ref root);
        }

        public void Dispose()
        {
            PlayerLoop.SetPlayerLoop(root);
        }

        private static bool InsertAfter<T>(PlayerLoopSystem subSystem, ref PlayerLoopSystem parentSystem)
            where T : struct
        {
            var subSystems = parentSystem.subSystemList?.ToList();
            if (subSystems == default) return false;

            bool found = false;
            for (int i = 0; i < subSystems.Count; i++)
            {
                var s = subSystems[i];
                if (s.type == typeof(T))
                {
                    found = true;
                    subSystems.Insert(i + 1, subSystem);
                    break;
                }
            }

            if (!found)
            {
                for (int i = 0; i < subSystems.Count; i++)
                {
                    var s = subSystems[i];
                    if (InsertAfter<T>(subSystem, ref s))
                    {
                        found = true;
                        subSystems[i] = s;
                        break;
                    }
                }
            }

            if (found)
                parentSystem.subSystemList = subSystems.ToArray();

            return found;
        }

        private static bool InsertBefore<T>(PlayerLoopSystem subSystem, ref PlayerLoopSystem parentSystem)
            where T : struct
        {
            var subSystems = parentSystem.subSystemList?.ToList();
            if (subSystems == default) return false;

            bool found = false;
            for (int i = 0; i < subSystems.Count; i++)
            {
                var s = subSystems[i];
                if (s.type == typeof(T))
                {
                    found = true;
                    subSystems.Insert(i, subSystem);
                    break;
                }
            }

            if (!found)
            {
                for (int i = 0; i < subSystems.Count; i++)
                {
                    var s = subSystems[i];
                    if (InsertBefore<T>(subSystem, ref s))
                    {
                        found = true;
                        subSystems[i] = s;
                        break;
                    }
                }
            }

            if (found)
                parentSystem.subSystemList = subSystems.ToArray();

            return found;
        }

        private static bool InsertIn<T>(PlayerLoopSystem subSystem, ref PlayerLoopSystem parentSystem)
            where T : struct
        {
            var subSystems = parentSystem.subSystemList?.ToList();
            if (subSystems == default) subSystems = new List<PlayerLoopSystem>();

            bool found = false;
            if (parentSystem.type == typeof(T))
            {
                subSystems.Insert(0, subSystem);
                found = true;
            }
            else
            {
                for (int i = 0; i < subSystems.Count; i++)
                {
                    var s = subSystems[i];
                    if (InsertIn<T>(subSystem, ref s))
                    {
                        found = true;
                        subSystems[i] = s;
                        break;
                    }
                }
            }

            if (found)
                parentSystem.subSystemList = subSystems.ToArray();

            return found;
        }
    }
}