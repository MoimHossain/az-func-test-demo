

# Quick win in PowerShell 7
Only works on PS7

## PowerShell 7

Kill a process that using a port
```
kill $(Get-NetTCPConnection -LocalPort 34698 -ErrorAction Ignore).OwningProcess
```

# For other PS versions

## Find process that blocks a port

Run the following powershell command
```

Get-NetTCPConnection -State Listen | Select-Object -Property *, @{'Name' = 'ProcessName';'Expression'={(Get-Process -Id $_.OwningProcess).Id}} | select ProcessName,LocalAddress,LocalPort

```


## Kill the process

Kill by ID

```
Stop-Process -Id <PID>
```

