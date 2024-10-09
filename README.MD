![Copyright ev0lve Digital](https://img.shields.io/badge/Copyright-ev0lve%20Digital-blue) ![GitHub License](https://img.shields.io/github/license/imi-tat0r/CSSharpUtils) ![Issues](https://img.shields.io/github/issues/imi-tat0r/CSSharpUtils) ![Stars](https://img.shields.io/github/stars/imi-tat0r/CSSharpUtils)

# CSSharpUtils
CSSharpUtils is an extension library for CounterStrikeSharp that simplifies player management, message formatting, configuration updates, and more. It provides a set of extensions and utilities to enhance the development experience for CounterStrikeSharp game mods.

# Installation
Run `dotnet add package CSSharpUtils` or download the latest release from the [releases page](https://github.com/imi-tat0r/CSSharpUtils/releases) and add it as a reference to your project.

## Extensions
- **Config**: Simplifies the process of updating and managing plugin configurations by automatically handling version updates and serialization.
- **Player**: Offers methods for common player actions such as kicking, moving to a team, setting armor, and getting the eye position.
- **GameRules**: Enhances game rule interactions, including calculating the remaining time in the current round.

## Utils
- **Chat**: Provides methods for formatting chat messages with predefined color codes and sending messages to specific teams.
- **CsTeam**: Utilities for team management, including counting players, alive players, combined health, and selecting random teams.
- **Game**: Contains methods for managing game states such as starting and ending warmup, pausing, and unpausing matches, and retrieving game rules.
- **Server**: Utilities for retrieving server information such as the IP.

## Examples

### Config

```csharp
using CSSharpUtils.Extensions;

// updates the config file on disk if the version is outdated
config.Update();

// reloads the config file from disk
OnConfigParsed(new MyPluginConfig().Reload());

// get the plugin config path
Console.WriteLine($"Config path: {ConfigExtensions.ConfigPath}");
```

### Player
```csharp
using CSSharpUtils.Extensions;

// this is true for connected, valid, human players
if (!playerController.IsPlayer())
    return;

// name and clan tag
playerController.SetName("imi-tat0r");
playerController.SetClantag("imi-tat0r.net");
playerController.SetClantag(); // removes the clantag

// setting armor + helmet, not heavy
playerController.SetArmor(100, true, false);

// setting health
playerController.SetHealth(69); // sets health to 69 
playerController.SetHealth(420); // sets health to 420
playerController.SetHealth(1337, false); // sets health to 100 (clamped)

// setting money
playerController.SetMoney(1337);

// checking for permission
playerController.HasPermission("@css/generic");

// freeze and unfreeze the player
playerController.Freeze();
playerController.Unfreeze();

// NOTE: CS2 does not display the kick message to the player
playerController.Kick("You have been kicked.");

playerController.MoveToTeam(CsTeam.Terrorists);

var EyePos = playerController.GetEyePosition();
```

### GameRules
```csharp
using CSSharpUtils.Utils;
using CSSharpUtils.Extensions;

var gameRules = GameUtils.GetGameRules(); // this should be cached appropriately
var remainingRoundTime = gameRules.GetRemainingRoundTime();
```

### Chat
```csharp
using CSSharpUtils.Utils;

var message = "Hello {LightBlue}World."

player.PrintToChat(ChatUtils.FormatMessage(message)); // will print "Hello World" as a colored message
player.PrintToChat(ChatUtils.CleanMessage(message)); // will print "Hello World" in full white

// Send a message to all players in the Terrorist team
ChatUtils.PrintToTeam(CsTeam.Terrorist, "This message is for Terrorists only.");
```

### Team
```csharp
using CSSharpUtils.Utils;

// Get the number of alive players in the Counter-Terrorist team
int aliveCTs = CsTeamUtils.GetAlivePlayerCount(CsTeam.CounterTerrorist);
Console.WriteLine($"Alive Counter-Terrorists: {aliveCTs}");

// Select a random team
CsTeam randomTeam = CsTeamUtils.GetRandomTeam();
Console.WriteLine($"Randomly selected team: {randomTeam}");
```

### Game
```csharp
using CSSharpUtils.Utils;

// Start a warmup period of 30 seconds
GameUtils.StartWarmup(30);

// End the warmup
GameUtils.EndWarmup(10); // in 10 seconds
GameUtils.EndWarmup(); // immediately

// Pause the match
GameUtils.PauseMatch();

// Unpause the match
GameUtils.UnpauseMatch();

// Retrieve the current game rules
var gameRules = GameUtils.GetGameRules();
if (gameRules != null)
{
    Console.WriteLine("Game rules retrieved.");
}
else
{
    Console.WriteLine("Failed to retrieve game rules.");
}
```

### Server
```csharp
using CSSharpUtils.Utils;

Console.WriteLine($"Server IP: {ServerUtils.GetServerIp()}");
```

## Credits
- [CounterStrikeSharp](https://github.com/roflmuffin/CounterStrikeSharp)
