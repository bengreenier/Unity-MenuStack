using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using MenuStack;
using System.Reflection;
using System.Linq;

public class ComplexTest
{
    /// <summary>
    /// Ensure opening menus works
    /// </summary>
    [Test]
    public void Open()
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

        Assert.IsTrue(subMenu1Root.activeInHierarchy);
        Assert.IsTrue(subMenu2Root.activeInHierarchy);
        Assert.IsFalse(subMenu2ButtonRoot.activeInHierarchy);
        Assert.IsTrue(subMenu1Button.enabled);
        Assert.IsFalse(subMenu2Button.enabled);

        menu.Open(subMenu2);

        Assert.IsTrue(subMenu1Root.activeInHierarchy);
        Assert.IsTrue(subMenu2Root.activeInHierarchy);
        Assert.IsFalse(subMenu1ButtonRoot.activeInHierarchy);
        Assert.IsFalse(subMenu1Button.enabled);
        Assert.IsTrue(subMenu2Button.enabled);
    }

    /// <summary>
    /// Ensure opening menus and leaving the current menu visible works
    /// </summary>
    [Test]
    public void Open_LeaveVisible()
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
        var subMenu2 = subMenu2Root.AddComponent<MenuStack.OverlayMenu>();

        var subMenu2ButtonRoot = new GameObject();
        subMenu2ButtonRoot.transform.parent = subMenu2Root.transform;
        var subMenu2Button = subMenu2ButtonRoot.AddComponent<UnityEngine.UI.Button>();
        subMenu2Button.enabled = true;

        menu.TrackedMenus = new MenuStack.Menu[] { subMenu1, subMenu2 };

        menu.GetType().GetMethod("Awake", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance).Invoke(menu, null);

        Assert.IsTrue(subMenu1Root.activeInHierarchy);
        Assert.IsTrue(subMenu2Root.activeInHierarchy);
        Assert.IsFalse(subMenu2ButtonRoot.activeInHierarchy);
        Assert.IsTrue(subMenu1Button.enabled);
        Assert.IsFalse(subMenu2Button.enabled);

        // block scope for old bool-driven API
        {
#pragma warning disable 0618
            menu.Open(subMenu2, leaveOldVisible: true);
#pragma warning restore 0618

            Assert.IsTrue(subMenu1Root.activeInHierarchy);
            Assert.IsTrue(subMenu2Root.activeInHierarchy);
            Assert.IsTrue(subMenu2ButtonRoot.activeInHierarchy);
            Assert.IsFalse(subMenu1Button.enabled);
            Assert.IsTrue(subMenu2Button.enabled);
        }

        // block scope for new class-driven API
        {
            menu.Open(subMenu2);

            Assert.IsTrue(subMenu1Root.activeInHierarchy);
            Assert.IsTrue(subMenu2Root.activeInHierarchy);
            Assert.IsTrue(subMenu2ButtonRoot.activeInHierarchy);
            Assert.IsFalse(subMenu1Button.enabled);
            Assert.IsTrue(subMenu2Button.enabled);
        }
    }

    /// <summary>
    /// Ensure opening and closing menus works
    /// </summary>
    [Test]
    public void OpenClose()
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

        Assert.IsTrue(subMenu1Root.activeInHierarchy);
        Assert.IsTrue(subMenu2Root.activeInHierarchy);
        Assert.IsFalse(subMenu2ButtonRoot.activeInHierarchy);
        Assert.IsTrue(subMenu1Button.enabled);
        Assert.IsFalse(subMenu2Button.enabled);

        bool wasMenuOpened = false;
        menu.Opened += (m) =>
        {
            Assert.AreEqual(subMenu2.gameObject.name, m.gameObject.name);
            wasMenuOpened = true;
        };

        menu.Open(subMenu2);

        Assert.IsTrue(wasMenuOpened);
        Assert.IsTrue(subMenu1Root.activeInHierarchy);
        Assert.IsFalse(subMenu1ButtonRoot.activeInHierarchy);
        Assert.IsTrue(subMenu2Root.activeInHierarchy);
        Assert.IsFalse(subMenu1Button.enabled);
        Assert.IsTrue(subMenu2Button.enabled);

        bool wasMenuClosed = false;
        menu.Closed += (m) =>
        {
            Assert.AreEqual(subMenu2.gameObject.name, m.gameObject.name);
            wasMenuClosed = true;
        };

        menu.Close();

        Assert.IsTrue(wasMenuClosed);
        Assert.IsTrue(subMenu1Root.activeInHierarchy);
        Assert.IsTrue(subMenu2Root.activeInHierarchy);
        Assert.IsFalse(subMenu2ButtonRoot.activeInHierarchy);
        Assert.IsTrue(subMenu1Button.enabled);
        Assert.IsFalse(subMenu2Button.enabled);
    }

    /// <summary>
    /// ensure submenus don't open when you open their parent menu
    /// </summary>
    [Test]
    public void Open_SubMenus()
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

        var subMenu2SubMenu1Root = new GameObject();
        subMenu2SubMenu1Root.transform.parent = subMenu2Root.transform;
        var subMenu2SubMenu1 = subMenu2SubMenu1Root.AddComponent<MenuStack.Menu>();

        var subMenu2SubMenu1ButtonRoot = new GameObject();
        subMenu2SubMenu1ButtonRoot.transform.parent = subMenu2SubMenu1Root.transform;

        var subMenu2SubMenu1Button = subMenu2SubMenu1ButtonRoot.AddComponent<UnityEngine.UI.Button>();
        subMenu2SubMenu1Button.enabled = true;

        menu.TrackedMenus = new MenuStack.Menu[] { subMenu1, subMenu2, subMenu2SubMenu1 };

        menu.GetType().GetMethod("Awake", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance).Invoke(menu, null);

        Assert.IsTrue(subMenu1Root.activeInHierarchy);
        Assert.IsTrue(subMenu2Root.activeInHierarchy);
        Assert.IsTrue(subMenu2SubMenu1Root.activeInHierarchy);
        Assert.IsFalse(subMenu2SubMenu1ButtonRoot.activeInHierarchy);
        Assert.IsTrue(subMenu1Button.enabled);
        Assert.IsFalse(subMenu2Button.enabled);
        Assert.IsFalse(subMenu2SubMenu1Button.enabled);

        menu.Open(subMenu2);

        Assert.IsTrue(subMenu1Root.activeInHierarchy);
        Assert.IsFalse(subMenu1ButtonRoot.activeInHierarchy);
        Assert.IsTrue(subMenu2Root.activeInHierarchy);
        Assert.IsTrue(subMenu2SubMenu1Root.activeInHierarchy);
        Assert.IsFalse(subMenu2SubMenu1ButtonRoot.activeInHierarchy);
        Assert.IsFalse(subMenu1Button.enabled);
        Assert.IsTrue(subMenu2Button.enabled);
        Assert.IsFalse(subMenu2SubMenu1Button.enabled);
    }
}
