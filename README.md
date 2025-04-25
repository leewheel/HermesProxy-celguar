## Work in progress
This project is in very early stage of development. Currently it allows only to log in and see character list (which is broken).


## Description
This project continues HermesProxy work to enable play on legacy WoW servers cores the modern clients.

There are 4 major components to the application:
- The modern BNetServer (Supposedly updated).
- The legacy AuthClient (Supposedly updated). 
- The modern WorldServer (Totally broken). 
- The legacy WorldClient (Probably requires updates to support new features) (testing on AzerothCore).

Working to support those versions.

| Modern Versions | Legacy Versions |
|-----------------|-----------------|
| 3.4.3           | 3.3.5a          |

## Ingame Settings
Note: Keep `Optimize Network for Speed` **enabled** (it's under `System` -> `Network`), otherwise you will get kicked every now and then.

## Usage Instructions

- Edit the app's config to specify the exact versions of your game client and the remote server, along with the address.
- Go into your game folder, in the Classic or Classic Era subdirectory, and edit WTF/Config.wtf to set the portal to 127.0.0.1.
- Download [Arctium Launcher](https://github.com/Arctium/WoW-Launcher/releases/tag/latest) into the main game folder, and then run it  
  `--staticseed --version=Classic` for WotLK.
- Start the proxy app and login through the game with your usual credentials.

## Chat commands
HermesProxy provides some internal chat commands:

| Command                    | Description                                                                  |
|----------------------------|------------------------------------------------------------------------------|
| `!qcomplete <questId>`     | Manually marks a quest as already completed (useful for quest helper addons) |
| `!quncomplete <questId>`   | Unmarks a quest as completed                                                 |

## Start Arguments
 - `--config <filename>` to specify a config (default `HermesProxy.config`)
 - `--set <key>=<value>` to overwrite a specific config value (example `--set ServerAddress=logon.example.com`)
 - `--no-version-check`  to disable the check for newer versions

## Acknowledgements
- [CypherCore](https://github.com/CypherCore/CypherCore)
- [CypherCoreClassicWOTLK](https://github.com/RioMcBoo/CypherCoreClassicWOTLK)
- [BotFarm](https://github.com/jackpoz/BotFarm)
