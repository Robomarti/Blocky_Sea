# How everything works

This document is designed to give explanations how components in this package work, and some justifications why I have choosed to implement them like I have.
Each subsection contains information about a certain script, and the sections are grouped by the game object that the scripts share.


## Ground

The ground object and its children do not have any scripts, but they are included to show how the sea
material looks when close to a shore.

![Ground Block Gameobject](https://github.com/Robomarti/Blocky_Sea/blob/main/documentation/Images/playerCloseToGroundBlock.png)


## Sea Manager

![Sea Manager Gameobject](https://github.com/Robomarti/Blocky_Sea/blob/main/documentation/Images/SeaManager.png)


### Display Sea.cs

### Sea Generator.cs

#### Sea Mesh Generator.cs

### Create Sea Chunks.cs


## Player

The player gameobject is a simple cube that can be moved around. Its main purpose is to showcase movement and how the generated
sea chunks are moved with it.

The player gameobject has a Player Input component and a Player Movement script.
A small project like this one doesn't really need the new input system, but I used it to get practise with it.
The movement script has a reference to the Sea Manager, since all the new sea chunks are created as its children, so
we can use the Sea Manager to move them all at the same time.

![Player Gameobject](https://github.com/Robomarti/Blocky_Sea/blob/main/documentation/Images/Player.png)


### Player Movement.cs

We move the sea chunks only if the player has moved a full integer, and the difference of the last position of the sea chunks
and the current full integer position of the player is larger than movementIncrement to avoid sea chunks jittering when moved.
The variable movementIncrement is based on the Level of Detail of the last element on the detailLevels struct, so it is
required that the largest LOD must be placed last in the detailLevels struct.

The jitter that can be seen when two sea chunks with different level of details are near each other isn't fixed, but if the camera is positioned
so that the player only sees one type of sea chunk, the movement of sea chunks is unnoticeable. However, having only one level of detail in the scene 
makes the level of detail system quite useless, but I was not able to find a nice solution during the project.


## Wave Manager

![Wave Manager Gameobject](https://github.com/Robomarti/Blocky_Sea/blob/main/documentation/Images/WaveManager.png)


### Wave Manager.cs


## Canvas / WindDirectionBackground

![Wind Direction Background Gameobject](https://github.com/Robomarti/Blocky_Sea/blob/main/documentation/Images/WindDirectionDragger.png)


### Drag Wave Direction.cs

I implemented player input for wave direction as a UI arrow that can be rotated by dragging on it.
However, I did not make the arrow update its rotation when something changes the wave direction, as I did not want to spend more time
on creating more player controls for changing the wave direction.

![Initial wave direction](https://github.com/Robomarti/Blocky_Sea/blob/main/documentation/Images/InitialWindDirection.png)
![Changed wave direction](https://github.com/Robomarti/Blocky_Sea/blob/main/documentation/Images/ChangedWindDirection.png)