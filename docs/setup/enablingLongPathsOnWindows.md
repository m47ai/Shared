# Enabling Long Paths on Windows

If you are running **Windows** and need to enable support for long file paths, you can modify the Windows Registry. Follow these steps:

1. Open the Registry Editor by searching for **regedit** in the Start menu.
2. Navigate to **HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\FileSystem** in the Registry Editor.
3. Locate the **LongPathsEnabled** entry and set its value to **1**.

If you are using **Windows Pro or Enterprise**, you can also enable long paths using Local Group Policies:

1. Open the Local Group Policy Editor by searching for **gpedit.msc** in the Start menu.
2. Go to **Computer Configuration > Administrative Templates > System > Filesystem** in the Local Group Policy Editor.
3. Open the **Enable Win32 long paths** policy and set it to **Enabled**.

## Git Configuration

To ensure Git supports long file paths, you can set the `core.longpaths` configuration option using the following command:

```sh
git config --system core.longpaths true
```
