# Spawn-of-Cthulhu
 
This is the entire unity project im working on at the moment.<br>Currently I'm implementig basic features I want the game to have.<br>It's using an isometric projection giving it a 3D look despite being entirely 2D.<br>All the graphical assets were completely made from scratch using either <a href="https://www.blender.org/">Blender </a> or <a href="https://www.aseprite.org/">Aseprite</a>.<br>I used sprite sheets for all the tile sets and character animations in order to improve the performance.<br>These sprite sheets are also used for the scriptable tiles to make sure the correct tiles are displayed depending on the position or abscence of neighbouring tiles.<br>The random tilemap generation itself is based on the <a href="https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life">Conway's Game of Life </a> the parameters for this algorythm can be changed depending on how the generated map should look.<br>By using the <a href="https://arongranberg.com/astar/">A* Pathfinding Project </a> I was provided with the tools necessary to produce reliable pathfinding within the tilemap borders.<br> This means on every generated tilemap the walkable area will automatically be set correctly, in addition shadow casters will also automatically be created.<br>Because I'm using the universal render pipeline's light system I decided to also add <a href="https://de.wikipedia.org/wiki/Normal_Mapping">Normal Maps </a> to most of the textures in the scene.

## Summary
 <ul>
  <li>Isometric Projection</li>
  <li>Procedural Map Generation</li>
  <li>Scriptable Tiles</li>
  <li>A* Pathfinding </li>
  <li>Shadow Caster Generation</li>
  <li>Normal Maps</li>
  <li>Scriptable Tiles</li>
  <li>Character Animation</li>
  <li>DOTween</li>
  <li>Multiple Layers</li>
  <li>Animated Line Renderer</li>
</ul> 

## Location of Scripts

Spawn-of-Cthulhu/Assets/Scripts/

## Demo

![Cthulhu](https://user-images.githubusercontent.com/78089013/162851004-ab7398ed-4087-4277-a5a2-9fdd5c3da0f2.gif)


![mapgeneration](https://user-images.githubusercontent.com/78089013/162851014-9e9dc58b-9c53-4ebb-8c05-ecfd81c1ce3e.gif)



