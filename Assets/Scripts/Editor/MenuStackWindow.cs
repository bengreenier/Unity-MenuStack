using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace MenuStack.Editor
{
    public class MenuStackWindow : EditorWindow
    {
        private struct ObjectChange
        {
            public GameObject Object;
            public bool originalActive;
        }

        [MenuItem("Window/MenuStack/Hierarchy")]
        static void Init()
        {
            MenuStackWindow window = (MenuStackWindow)EditorWindow.GetWindow(typeof(MenuStackWindow));
            window.titleContent.text = "MenuStack - Hierarchy";

            window.Refresh();
            window.Show();
        }

        private MenuRoot selectedRoot;
        
        private List<GameObject> menus = new List<GameObject>();
        private Stack<ObjectChange> history = new Stack<ObjectChange>();
        
        void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            foreach (var m in menus)
            {
                var res = EditorGUILayout.Toggle(m.name, m.activeInHierarchy);
                if (res != m.activeInHierarchy)
                {
                    history.Push(new ObjectChange()
                    {
                        Object = m,
                        originalActive = m.activeInHierarchy
                    });
                    
                    Undo.RecordObject(m, "MenuStack.Hierarchy active change");
                    m.SetActive(res);
                }
            }
        }

        void OnFocus()
        {
            Refresh();
        }

        void OnHeirarchyChange()
        {
            Refresh();
        }

        void Refresh()
        {
            if (selectedRoot == null)
            {
                selectedRoot = GameObject.FindObjectOfType<MenuRoot>();
            }

            var tagger = new RuntimeMenuTagger(selectedRoot.transform, selectedRoot.MenuPrefix, selectedRoot.OverlayPrefix);

            menus.AddRange(selectedRoot.TrackedMenus.Select(m => m.gameObject));
            menus.AddRange(tagger.Tag().Select(m => m.gameObject));

            menus = menus.Distinct().ToList();
        }

        void Cleanup()
        {
            while (history.Count > 0)
            {
                var top = history.Pop();

                top.Object.SetActive(top.originalActive);
            }
        }

        void OnDestroy()
        {
            Cleanup();
        }

        void OnProjectChange()
        {
            Cleanup();
        }
    }
}