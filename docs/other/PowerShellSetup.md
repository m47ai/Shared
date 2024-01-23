# ⚠️ Check execution policy for the local computer

`Powershell` runs under **execdution policies**, we need to ensure that our local user have execution policy to run scripts

```powershell
Get-ExecutionPolicy -List
```

The expected or correct output should be something like that (for `CurrentUser`)

```powershell
        Scope ExecutionPolicy
        ----- ---------------
MachinePolicy       Undefined
   UserPolicy       Undefined
      Process       Undefined
  CurrentUser    RemoteSigned
 LocalMachine       AllSigned
```

If you get this output, you must change the execution policy for the `CurrentUser` **scope**.

```powershell
        Scope ExecutionPolicy
        ----- ---------------
MachinePolicy       Undefined
   UserPolicy       Undefined
      Process       Undefined
  CurrentUser       Undefined
 LocalMachine       Undefined
```

To set execution policy then we should execute the following command:

```powershell
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
```

Ensure that now the output is following the previous expected output.

```powershell
Get-ExecutionPolicy -List
```

Useful links: <https://learn.microsoft.com/en-us/powershell/module/microsoft.powershell.core/about/about_execution_policies?view=powershell-7.3>
