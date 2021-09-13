# Edge of Kuiper

```
 ┌────────────────────────────────────┐
 │                                    │
 │   ┌───────────┐     ┌───────────┐  │
 │   │   Combat  │     │  Missions │  │
 │   └───────────┘     └───────────┘  │
 │                                    │
 │   ┌───────────┐     ┌───────────┐  │
 │   │  Mining   │     │ Exploring │  │
 │   └───────────┘     └───────────┘  │
 │                                    │
 │   ┌───────────┐     ┌───────────┐  │
 │   │  Trading  │     │ Investing │  │
 │   └───────────┘     └───────────┘  │
 │                                    │
 │           Edge-of-Kuiper           │
 │      A Real-Time SpaceSim RPG      │
 └────────────────────────────────────┘
```

This game puts you into the captains chair of your very own spaceship in the Sol system. Your purpose is to make money and retire.

* Trade
* Invest
* Mine asteroids
* Take missions
* Combat
* Interact with random events
* Hire crew
* Outfit your ship

It's an async realtime game experience where time since your last visit is accelerated and happenings are generated.

The game is entirely based on Console commands, text and perhaps some nice ASCII graphics. The core of the game should be adaptable to discord bots or web. 

## Lore

The year is 2078, fusion-cores have been minimized to the point where they fit into bulky spaceships. The fusion-cores power torchdrives that make it possible to be under near constant thrust within our solar system. Trips to Mars are frequent and fast, taking no more than a week or so. 

Space is still big, and not much exists beyond the Kuiper belt which exists as an informal barrier between the inner solarsystem and the outer. The innersystem is largely controlled from earth, where as the outersystem is less civilized, less orderly and more akin to the classic wild west North America.

Kuiper belt contains untold riches in ice-water, iron, gold and rare elements.

The perfect time for someone to make a fortune.

## Getting started

### Prerequisites 

* [.NET 5](https://dotnet.microsoft.com/download)
* [JetBrains Rider](https://www.jetbrains.com/rider/)
* [Visual Studio (Code)](https://visualstudio.microsoft.com/)

Recommended plugins:
* [C# Omnisharp](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csharp)
* [GitLens](https://marketplace.visualstudio.com/items?itemName=eamodio.gitlens)
* [Coverage Gutters](https://marketplace.visualstudio.com/items?itemName=ryanluker.vscode-coverage-gutters)

### Build and run

CLI:
* dotnet restore
* dotnet run --project kuiper-game/kuiper-game.csproj
* dotnet test

IDE: 
* Press the F5 button
* Observe the Terminal output
