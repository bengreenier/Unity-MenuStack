using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace MenuStack.Editor
{
    /// <summary>
    /// An EditorWindow for quickly working with <see cref="Menu"/>s
    /// </summary>
    public class MenuStackWindow : EditorWindow
    {
        /// <summary>
        /// Internal representation of an object change, to track history
        /// </summary>
        private struct ObjectChange
        {
            public Menu Object;
            public bool Operation;
        }
        
        /// <summary>
        /// Initializer that gets or creates a window
        /// </summary>
        [MenuItem("Window/MenuStack/Hierarchy")]
        static void Init()
        {
            MenuStackWindow window = (MenuStackWindow)EditorWindow.GetWindow(typeof(MenuStackWindow));
            window.titleContent.text = "MenuStack - Hierarchy";

            window.Refresh();
            window.Show();
        }

        /// <summary>
        /// Internal reference to a MenuRoot
        /// </summary>
        private MenuRoot selectedRoot;
        
        /// <summary>
        /// Internal reference to all tracked menus
        /// </summary>
        private List<Menu> menus = new List<Menu>();

        /// <summary>
        /// Internal history stack of <see cref="ObjectChange"/>s
        /// </summary>
        private Stack<ObjectChange> history = new Stack<ObjectChange>();
        
        /// <summary>
        /// Finds all tracked <see cref="Menu"/>s and adds them to <see cref="menus"/>
        /// </summary>
        void Refresh()
        {
            if (selectedRoot == null)
            {
                selectedRoot = GameObject.FindObjectOfType<MenuRoot>();
            }

            var tagger = new RuntimeMenuTagger(selectedRoot.transform, selectedRoot.MenuPrefix, selectedRoot.OverlayPrefix);

            menus.AddRange(selectedRoot.TrackedMenus);
            menus.AddRange(tagger.Tag());

            menus = menus.Where(m => m != null).Distinct().ToList();
        }

        /// <summary>
        /// Undoes the history of all operations
        /// </summary>
        void Cleanup()
        {
            while (history.Count > 0)
            {
                var top = history.Pop();

                top.Object.SetVisible(!top.Operation);
            }
        }

        /// <summary>
        /// Unity hook for rendering
        /// </summary>
        void OnGUI()
        {
            // create a label
            EditorGUILayout.LabelField("Menus");
            EditorGUILayout.Separator();

            // only make the Reset button clickable if we have something to reset
            GUI.enabled = history.Count > 0;
            if (GUILayout.Button("Reset"))
            {
                Cleanup();
            }
            GUI.enabled = true;

            // create a vertical section full of menus that you can toggle visibility on/off for
            GUILayout.BeginVertical(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            foreach (var m in menus)
            {
                var name = string.IsNullOrEmpty(m.CustomName) ? m.name : m.CustomName;
                var res = GUILayout.Toggle(m.Visible, name, GUILayout.ExpandWidth(true));
                if (res != m.Visible)
                {
                    // apply and track the change
                    m.SetVisible(res);
                    history.Push(new ObjectChange()
                    {
                        Object = m,
                        Operation = res
                    });
                }
            }
            GUILayout.EndVertical();
        }

        /// <summary>
        /// Unity hook for window focus
        /// </summary>
        void OnFocus()
        {
            Refresh();
        }

        /// <summary>
        /// Unity hook for scene heirarchy change
        /// </summary>
        void OnHeirarchyChange()
        {
            Refresh();
        }

        /// <summary>
        /// Unity hook for window destruction
        /// </summary>
        void OnDestroy()
        {
            Cleanup();
        }

        /// <summary>
        /// Unity hook for project change
        /// </summary>
        void OnProjectChange()
        {
            Cleanup();
        }
    }
}