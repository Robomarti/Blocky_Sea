# How everything works

This document is designed to give explanations how components in this package work.
Each subsection contains information about a certain script, and the sections are grouped by the game object that the scripts share.


## Ground

The ground object and its children do not have any scripts, but they are included to show how the sea
material looks when close to a shore.

![Ground Block Gameobject](https://github.com/Robomarti/Blocky_Sea/blob/main/documentation/Images/playerCloseToGroundBlock.png)


## Sea Manager

This gameobject is used to control the settings of sea chunk creation, and to create sea chunks manually.

![Sea Manager Gameobject](https://github.com/Robomarti/Blocky_Sea/blob/main/documentation/Images/SeaManager.png)


### Display Sea.cs

This script doesn't necessarily need to be in its own file since it only has a small public function, 
and is not necessary for the main part of the project to work.
This script is responsible for updating a sample chunk object, called SeaMesh. It is a child of the Sea Manager object, and it is automatically disabled, but it can be
enabled to view changes to sea chunk settings.


### Sea Generator.cs

This script contains settings used for sea chunk generation. The Auto Update option is used to automatically displaying setting changes with the SeaMesh object.

This script also uses the GenerateSeaMesh function of the SeaMeshGenerator.cs file to construct the mesh for the sea chunks.
GenerateSeaMesh must generate a certain kind of mesh for the top faces to be independent. Otherwise the shader can't animate the faces independently, and
we don't get the blocky effect for the sea.

Instead of creating multiple independent cubes, GenerateSeaMesh creates a bottom plane which vertices the upper faces can share, since only the top part of the mesh
has to move when animated. This way we only need 1 lower vertice for each 4 top layer vertex, instead of 4 lower layer vertices for 4 top layer vertices.

### Create Sea Chunks.cs

This component is the most similar to Sebastian Lague's tutorial. I have not needed to modify it much since it worked well for me.
This component creates sea chunks of different level of details depending on how far they are from the player.
This makes it possible to automatically create lower quality sea chunks far away from the player where extra detail is not needed.


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

The Wave Manger object can be used to modify the properties of the sea shader. I have added basic controls such as amplitude and period for waves, but also
randomness chance and strength, which add variation to the waves. I have also added smoothing iterations, 
which apply different scales of waves to change the effect of other properties. The code for smoothing iterations is in the 
[IterateWaves.hlsl](https://github.com/Robomarti/Blocky_Sea/blob/main/Assets/Shaders/IterateWaves.hlsl) file.
Different properties can be seen in the image below.

![Wave Manager Gameobject](https://github.com/Robomarti/Blocky_Sea/blob/main/documentation/Images/WaveManager.png)


## Canvas / WindDirectionBackground

The object for changing the wind direction is simply the background for the arrow sprite I created.

![Wind Direction Background Gameobject](https://github.com/Robomarti/Blocky_Sea/blob/main/documentation/Images/WindDirectionDragger.png)


### Drag Wave Direction.cs

I implemented player input for wave direction as a UI arrow that can be rotated by dragging on it.
However, I did not make the arrow update its rotation when something changes the wave direction, as I did not want to spend more time
on creating more player controls for changing the wave direction.

![Initial wave direction](https://github.com/Robomarti/Blocky_Sea/blob/main/documentation/Images/InitialWindDirection.png)
![Changed wave direction](https://github.com/Robomarti/Blocky_Sea/blob/main/documentation/Images/ChangedWindDirection.png)