# Prerequisite Packages

There are some packages you will need to install manually.

## FMOD

You can get the FMOD package from
the [Unity Asset Store](https://assetstore.unity.com/packages/tools/audio/fmod-for-unity-161631).

#### FMOD Studio

If you would like to edit the FMOD project, you will need to download FMOD Studio from
the [FMOD website](https://www.fmod.com/download#fmodstudio).

### Setup

After importing the package into Unity, FMOD will open the setup wizard:

1. Open the Sample Scene in `Assets/_Project/Scenes/Sample Scene.unity`.
2. Open the wizard again (`FMOD > Setup Wizard`).
3. In the **Linking** section, click **FMOD Studio Project**, and navigate to `FMOD/FMOD.fspro`.
4. Run through the rest of the wizard, and you're good to go!

## Odin

You can get Odin via the [educational license](https://odininspector.com/educational/ontario-tech-university) or
from the Unity Asset Store ([Inspector & Serializer](https://assetstore.unity.com/packages/tools/utilities/odin-inspector-and-serializer-89041) and [Validator](https://assetstore.unity.com/packages/tools/utilities/odin-validator-227861)).

You will need the inspector, serializer and validator

### Setup

There's minimal setup. After opening the project, simply import the ```.unitypackage``` and go through the basic setup

## PrimeTween
Pick it up from [here](https://assetstore.unity.com/packages/tools/animation/primetween-high-performance-animations-and-sequences-252960), and import it from the package manager (Window > Package Manager > My Assets > Search PrimeTween).

# Contributions
## Members
Adam Tam | 100868600
## Contributions
Adam Tam: 100%

- All design patterns
- Scene setup
- Readme/Documentation

# Third Party Resources
This includes every asset store package listed in the Prerequisite Packages section.
However, I also used Adam Myhre's [Event Bus](https://github.com/adammyhre/Unity-Event-Bus) and [Service Locator](https://github.com/adammyhre/Unity-Service-Locator)
An important note: None of the third party assets were used to assist in any of the patterns created. If they do exist in the scripts I mention, they are there for the purpose of the project, not to assist in the patterns.

# Video Report
https://youtu.be/8KefkCAfrqM
The required "first slide" information is available in the description of the video

# Interactive Media Scenario Information
Ostinato is a roguelike rhythm game where you cast magical spells in the form of musical rhythms to fight corrupted enemies
These magical spells are called incantations, made up of a cast pattern and an attack pattern
The game determines which incantation you cast based on the cast pattern, and the attack pattern is used to deal damage
You must play these incantations to the beat of the song to deal maximum damage.
After you cast your incantation, it will be the enemies turn. During this time, you cannot cast, and must dodge to the enemy's attack pattern. You will know this based on the cast pattern they play

![Cast pattersn.png](Assets%2F_Project%2FArt%2FCast%20pattersn.png)
This is what you will see when playing the game. The notes on the left side is the cast pattern, and the notes on the right side is the attack pattern. 
You are expected to know how to read sheet music to play the game, but will be tutorialized in the future. For now, here's what the notes are:

![note examples.png](Assets%2F_Project%2FArt%2Fnote%20examples.png)
## Controls
- **WASD** to dash 
- **J/K** to cast
## Legend
![legend.png](Assets%2F_Project%2FArt%2Flegend.png)
# Singleton
My singleton implementation is located at Assets/_Project/_Scripts/Utility/Singleton/Singleton.cs 
It is a generic class that is inherited from MonoBehaviour and can also be inherited from to create a singleton. It is lazy loaded.
It also cleans itself up on destroy, so you don't have to worry about memory leaks.
## UML
![Singleton.png](Assets%2F_Project%2FUML%2FSingleton.png)
## Implementation
I used this for the **Music Manager** (Assets/_Project/_Scripts/Game Manager/MusicManager.cs). As I stated earlier, all I have to do is inherit from the Singleton class and I have a singleton.
I made the **Music Manager** the singleton because in a rhythm game, there should only be one instance of music playing at a time. This way, I can ensure that the music manager is always the same instance, and I can control the music from anywhere in the game.

# Command
My command implementation is located at Assets/_Project/_Scripts/Incantation/Castables/Caster/ICastCommand.cs
It is an interface that is used to create different commands that can be executed depending on if the player is doing a cast, attack, or is already completed/failed so it should return a null command. 
## UML
![Command.png](Assets%2F_Project%2FUML%2FCommand.png)
## Implementation
I used this for the **Casting System** (Assets/_Project/_Scripts/Incantation/Castables/Caster/IncantationCaster.cs) In the Cast() method, it queries a list of strategies (strategy pattern, won't be talking about it in this document) to get the correct command to execute. This way, I can easily add new commands to the game without having to change the code in the **Casting System**. 
This is then passed over to the **Combat System** (Assets/_Project/_Scripts/Game Manager/Combat/CombatManager.cs) to cast
# Factory
My factory implementation is located at (Assets/_Project/_Scripts/Incantation/Incantation.cs)
I used a builder implementation to create the incantation. I used it over a default factory because eventually, the player is able to create their own incantations, but not all information is known right away to enforce a constructor with all the information.
## UML
![Factory.png](Assets%2F_Project%2FUML%2FFactory.png)a
## Implementation
I used this for the **Incantation** (Assets/_Project/_Scripts/Incantation/Incantation.cs) class. The **Incantation** class contains a factory/builder that creates the incantation. If you look inside the script, I also enforced a private constructor to ensure that the incantation is created through the factory.
For now, the incantations are being made beforehand in the **Configuration System** (Assets/_Project/_Scripts/Incantation/IncantationConfig.cs)
## Observer
My observer implementation is located at Assets/_Project/_Scripts/Input System/Player/PlayerInputProcessor.cs)
I used this pattern mainly in the **Casting System**, for input receiving.
## UML
![Observer.png](Assets%2F_Project%2FUML%2FObserver.png)
## Implementation
In order to make this, I had to inherit from IFightActions and IPassiveActions, both of which are autogenerated by Unity's Input System. By doing this, I am able to register callbacks to my processor, and then call actions based on the input. One could say that this is a facade pattern too.
The general implementation consists of callbacks calling the required methods according to the interface, and that does checks to make sure the input is pressed and not released/other important checks, then delegates them off to the action.

