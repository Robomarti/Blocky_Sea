# Retrospective

This document notes why I started the project, what I learned during the project, and what I would do differently. It also contains a list of
things I would add if I had to continue this project.


## Why I created this project

Originally this project was supposed to be just a small part of a game. However, I quickly noticed how complicated shaders and mesh generation
are, so I decided to focus on just improving my knowledge on shaders and mesh generation in Unity. I set myself a time limit of 2 months for this
project because I also want to move on to other projects. This is also the reason why I won't be continuing this project further.


## What I learned

Firstly and most importantly I learned how mesh generation works in Unity. I learned the limits of mesh generation (for example a limit for the amount of vertices
a single mesh can have), as well as how shaders work in Unity and in general. I learned that while writing purely HLSL code for shaders is not banned by Unity, it is also
not very easy to implement, since Unity makes it pretty complicated. Unity seems to strongly want everyone to use Shader Graph. I also learned how to create shaders with Shader Graph,
and how to optimize shaders and which aspects affect shader performance.

My logical thinking and problem solving ability both improved during this project, since I have had many problems due to not having any knowledge of shaders or mesh
generation prior to this project. For example, Unity doesn't print an error if you try to create a triangle with a vertex which index is outside of the vertices array.
What I think happens instead is that Unity will create the triangle but connect it to one of the first vertices of the array. The end result looks weird, and only extensive
testing revealed that the sea chunk sizes were too large.


## Further development

Here is a list of further development that I would do if I had to continue this project. The key takeaway here is that
I probably would just start again and use DOTS and ECS to create a lot of cubes for sea chunks instead, because they are simpler to implement and maintain.


### Optimizing the meshes of sea chunks

We can reduce the vertex count of sea chunks by removing the UV channel that is currently used to store quad data to keep each vertex of a quad in the same height.

This makes each vertex of a quad have a different height, but it would reduce the vertex count by half.

Another way of ensuring that each vertex of a quad has the same height is to take the Vertex ID in the shader graph, substract 1 from it (because its indexing starts at a different number for some reason), divide it by 4 since the 4 vertices of a quad are next to each other in the mesh.vertices array, and finally floor the result with the Floor node so we get the same integer for each vertex of a quad.
We can use this integer to generate the same height for every vertex of a quad, but I haven't discovered a way to do that yet. 
Each sea chunk mesh has a different amount of vertices per side, depending on its level of detail and sea chunk size, so we cannot calculate a common sample point for each 4 vertex of a quad to calculate its height.

Another way of getting the same height for each vertice of a quad would be to get the length of the mesh.vertices array and take its square root to get the count of vertices per side, since all sea chunks are square. We could use this with the first approach to calculate a common sample point, but there is no easy way to get the length of the mesh.vertices array into the shader graph. We could probably get the length easily in a HLSL shader, but rewriting the whole shader graph into a HLSL file would take too much time.

However, I believe that the best way to achieve the original idea of a blocky sea is to just use Unity's DOTS and ECS to render individual moving blocks, as it might very well be the fastest and easiest way to achieve my vision. An example of this was shown in Tarodev's video on Youtube https://www.youtube.com/watch?v=6mNj3M1il_c


### Smooth sea chunk movement

Currently the sea chunk movement can look a bit janky when looking at two sea chunks of different level of detail. Because cubes / blocks of
different levels of detail are different sizes, I think they should be moved at different times for the movement to look smoother.
I had implemented a system of sea chunks of different levels of detail having different parents, so that the player could move
only the relevant sea chunks, but I scrapped it from the project because in the end it didn't look good because it left gaps between sea chunks of
different levels of detail.


### Sea Foam

I never had the time to add a sea foam effect, since I couldn't think of a nice formula for it since it is a bit complicated how sea foam forms in nature.
I tried to make a formula that displays a white color when the wave is in its maximum height, but did not work and got too complicated and messy to try to fix.


### Sea color flashing when near ground blocks

If the ground blocks are perfectly on the grids, the renderer can't decide which one it should show so it flickers both the ground and sea blocks at the same time.


### Weird shader effect on lower right corner of the screen

The shallow sea color appears more on the lower right corner of the screen for some reason.
I don't really know what causes this and how I would go on to fix it.