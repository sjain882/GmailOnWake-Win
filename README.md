# Get email notifications (via Gmail) when a Windows system resumes from sleep

*No support provided*

# Prerequisites

- .NET 9 SDK
- Windows 10+ (may work on earlier versions)

# Building

In the project root directory:

`dotnet restore`

`dotnet build --configuration Release`

Then, find the compiled binaries in:

```
GmailOnWake-Win\bin\Release\net9.0-windows\
GmailOnWake-Win-Launcher\bin\Release\net9.0-windows\
```

Copy them to your desired directory, then configure options before running (see below).

# Configuring

You can find the options in `GmailOnWake-Win-Launcher.dll.config` and `GmailOnWake-Win.dll.config` for each program's binary. You must review them before running.

# Configurable options

- Delay before sending first email
- Log wake events to file on disk
- Max retry attempts for sending email
- Global logfile toggle
- Global program toggle (to easily disable without needing to mess with PowerTriggersV2)
- Gmail Address
- Gmail App Password
- Wake email subject prefix

# Example wake email contents

Only subject is included:

`HTPC Wake - 2025-11-03 21:20`

Format is `yyyy-MM-dd HH:mm`.

# Intended for use with PowerTriggersV2

Full details on this program and how to set it up can be found in this repo: https://github.com/sjain882/Ethernet-ForWakeOnLanOnly-Win

First, configure the path to the built `GmailOnWake-Win.exe` in `GmailOnWake-Win-Launcher.dll.config`.

Then, add `GmailOnWake-Win-Launcher.exe` to Resume tasks instead of Suspend tasks: https://github.com/sjain882/Ethernet-ForWakeOnLanOnly-Win?tab=readme-ov-file#how-to-use

# Use case

My father often wakes up the HTPC to watch streams, but forgets to close them, which prevents the PC from sleeping. Additionally, other programs like OBS and/or VPN could also keep the computer awake. Its important for me to remotely check the system and sleep it after he has used it, to prevent wasting energy.