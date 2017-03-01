# unity-menustack

A way to track unity menu navigation that doesn't suck too much.

## What

Managing the navigation of menu systems in unity is sucky. __It shouldn't be__.
`unity-menustack` provides simple menu state tracking behaviours.

## How

+ Include [Assets/Scripts](./Assets/Scripts) in your project
+ Add a `MenuRoot` to some root level menu object
+ Prefix the name of all child menus with `menu`
+ Call `this.GetComponentInParent<MenuRoot>().Open(Menu)` or ``this.GetComponentInParent<MenuRoot>().Close()` from any submenu to navigate
+ [Optional] Listen for `MenuRoot.Opened` and `MenuRoot.Closed` events to know when things change

See the generated docs ([raw](./docs) or [hosted](https://bengreenier.github.io/Unity-MenuStack)) for more info

## License

MIT