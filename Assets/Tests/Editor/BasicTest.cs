using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using MenuStack;
using System.Reflection;
using System.Linq;

public class BasicTest
{
    [Test]
    public void DisableRuntimeMenuTagging_True()
    {
        var menuRoot = new GameObject();
        var menu = menuRoot.AddComponent<MenuRoot>();
        menu.DisableRuntimeMenuTagging = true;

        var subMenu1Root = new GameObject();
        subMenu1Root.transform.parent = menuRoot.transform;
        var subMenu1 = subMenu1Root.AddComponent<MenuStack.Menu>();

        var subMenu2Root = new GameObject();
        subMenu2Root.transform.parent = menuRoot.transform;
        var subMenu2 = subMenu2Root.AddComponent<MenuStack.Menu>();

        var expectedTrackedMenus = new MenuStack.Menu[] { subMenu1, subMenu2 };

        menu.TrackedMenus = expectedTrackedMenus;

        menu.GetType().GetMethod("Awake", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance).Invoke(menu, null);

        CollectionAssert.AreEqual(expectedTrackedMenus, menu.TrackedMenus);
    }

    [Test]
    public void DisableRuntimeMenuTagging_False()
    {
        var menuRoot = new GameObject();
        var menu = menuRoot.AddComponent<MenuRoot>();
        menu.DisableRuntimeMenuTagging = false;

        var subMenu1Root = new GameObject("menu1");
        subMenu1Root.transform.parent = menuRoot.transform;

        var subMenu2Root = new GameObject("menu2");
        subMenu2Root.transform.parent = menuRoot.transform;

        var expectedMenuParentObjects = new GameObject[] { subMenu1Root, subMenu2Root };

        menu.GetType().GetMethod("Awake", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance).Invoke(menu, null);

        CollectionAssert.AreEqual(expectedMenuParentObjects, menu.TrackedMenus.ToList().Select(m => m.gameObject).ToArray());
    }

    [Test]
    public void DefaultActiveCheck()
    {
        var menuRoot = new GameObject();
        var menu = menuRoot.AddComponent<MenuRoot>();

        var subMenu1Root = new GameObject();
        subMenu1Root.transform.parent = menuRoot.transform;
        var subMenu1 = subMenu1Root.AddComponent<MenuStack.Menu>();

        var subMenu2Root = new GameObject();
        subMenu2Root.transform.parent = menuRoot.transform;
        var subMenu2 = subMenu2Root.AddComponent<MenuStack.Menu>();

        var subMenu2Object1 = new GameObject();
        subMenu2Object1.transform.parent = subMenu2Root.transform;
        subMenu2Object1.SetActive(true);

        menu.TrackedMenus = new MenuStack.Menu[] { subMenu1, subMenu2 };

        menu.GetType().GetMethod("Awake", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance).Invoke(menu, null);

        // both will be active, since menus don't get disabled when they aren't visible (only their children do)
        Assert.IsTrue(subMenu1Root.activeInHierarchy);
        Assert.IsTrue(subMenu2Root.activeInHierarchy);
        Assert.IsFalse(subMenu2Object1.activeInHierarchy);
    }

    [Test]
    public void DefaultInteractiveCheck()
    {
        var menuRoot = new GameObject();
        var menu = menuRoot.AddComponent<MenuRoot>();

        var subMenu1Root = new GameObject();
        subMenu1Root.transform.parent = menuRoot.transform;
        var subMenu1 = subMenu1Root.AddComponent<MenuStack.Menu>();

        var subMenu1ButtonRoot = new GameObject();
        subMenu1ButtonRoot.transform.parent = subMenu1Root.transform;
        var subMenu1Button = subMenu1ButtonRoot.AddComponent<UnityEngine.UI.Button>();
        subMenu1Button.enabled = true;

        var subMenu2Root = new GameObject();
        subMenu2Root.transform.parent = menuRoot.transform;
        var subMenu2 = subMenu2Root.AddComponent<MenuStack.Menu>();

        var subMenu2ButtonRoot = new GameObject();
        subMenu2ButtonRoot.transform.parent = subMenu2Root.transform;
        var subMenu2Button = subMenu2ButtonRoot.AddComponent<UnityEngine.UI.Button>();
        subMenu2Button.enabled = true;

        menu.TrackedMenus = new MenuStack.Menu[] { subMenu1, subMenu2 };

        menu.GetType().GetMethod("Awake", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance).Invoke(menu, null);

        Assert.IsTrue(subMenu1Button.interactable);
        Assert.IsFalse(subMenu2Button.interactable);
    }
}
