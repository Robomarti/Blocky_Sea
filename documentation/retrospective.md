# Retrospective

## What I learned

## Further development

### Sea Foam


### Optimizing the mesh
We can reduce the vertex count of sea chunks by removing the UV channel that is currently used to store quad data to keep each vertex of a quad in the same height.

This makes each vertex of a quad have a different height, but it would reduce the vertex count by half.

Another way of ensuring that each vertex of a quad has the same height is to take the Vertex ID in the shader graph, substract 1 from it (because its indexing starts at a different number for some reason), divide it by 4 since the 4 vertices of a quad are next to each other in the mesh.vertices array, and finally floor the result with the Floor node so we get the same integer for each vertex of a quad.
We can use this integer to generate the same height for every vertex of a quad, but I haven't discovered a way to do that yet. 
Each sea chunk mesh has a different amount of vertices per side, depending on its level of detail and sea chunk size, so we cannot calculate a common sample point for each 4 vertex of a quad to calculate its height.

Another way of getting the same height for each vertice of a quad would be to get the length of the mesh.vertices array and take its square root to get the count of vertices per side, since all sea chunks are square. We could use this with the first approach to calculate a common sample point, but there is no easy way to get the length of the mesh.vertices array into the shader graph. We could probably get the length easily in a HLSL shader, but rewriting the whole shader graph into a HLSL file would take too much time.

However, I believe that the best way to achieve the original idea of blocky sea is to just use Unity's DOTS and ECS to render individual moving blocks, as it might very well be the fastest and easiest way to achieve my vision. An example of this was shown in Tarodev's video on Youtube https://www.youtube.com/watch?v=6mNj3M1il_c


### Sea color flashing when near ground blocks

If the ground blocks are perfectly on the grids, the renderer can't decide which one it should show so it flickers both the ground and sea blocks at the same time.