# How everything works

This document is designed to give explanations how components in this package work, and some justifications why I have choosed to implement them like I have.
Each subsection contains information about a certain script, and the sections are grouped by the game object that the scripts share.

## Ground

## Sea Manager

### Display Sea.cs

### Sea Generator.cs

#### Sea Mesh Generator.cs

### Create Sea Chunks.cs


## Player

### Player Movement.cs

We check if the difference in position is larger than largestLevelOfDetail+1 
because cubes of a LOD largestLevelOfDetail chunk are largestLevelOfDetail+1 times larger than
cubes of a LOD 0 chunk.
This requires that the largest LOD must be placed last in the detailLevels struct.


## Wave Manager

### Wave Manager.cs


## Canvas / WindDirectionBackground

### Drag Wave Direction.cs

I implemented player input for wave direction as a UI arrow that can be rotated by dragging on it.
However, I did not make the arrow update its rotation when something changes the wave direction, as I did not want to spend more time
on creating more player controls for changing the wave direction.

