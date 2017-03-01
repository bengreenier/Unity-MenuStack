using UnityEngine;
using NUnit.Framework;
using MenuStack;
using System.Reflection;
using System;
using System.Diagnostics;

public class TimingTest
{
    void CreateMenus(GameObject parent, int number)
    {
        for (var i = 0; i < number; i++)
        {
            var subMenu1Root = new GameObject("menu" + Guid.NewGuid().ToString());
            subMenu1Root.transform.parent = parent.transform;
            subMenu1Root.AddComponent<MenuStack.Menu>();

            var subMenu1ButtonRoot = new GameObject();
            subMenu1ButtonRoot.transform.parent = subMenu1Root.transform;
            var subMenu1Button = subMenu1ButtonRoot.AddComponent<UnityEngine.UI.Button>();
            subMenu1Button.enabled = true;
        }
    }

    [Test]
    public void AwakeLargeMenuTree_RuntimeImpact()
    {
        var testMenuCount = 600;

        var menuRoot = new GameObject();
        var menu = menuRoot.AddComponent<MenuRoot>();
        menu.DisableRuntimeMenuTagging = false;

        CreateMenus(menuRoot, testMenuCount);

        var watch = new Stopwatch();

        watch.Start();
        menu.GetType().GetMethod("Awake", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance).Invoke(menu, null);
        watch.Stop();

        Assert.IsTrue(watch.ElapsedMilliseconds < 5 * 1000,
            "A " + testMenuCount + "Menu Runtime tag took " + watch.ElapsedMilliseconds + "ms (> 5s)");
    }
}
