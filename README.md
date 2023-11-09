# Dunelands
 This is an old project I did when I was in high school to experiment with procedural generation and UI in Unity. It generates a primarily desert world, with polar ice caps and jungles in between and has a test 4X inspired UI for handling production and research for a faction.

![Screenshot of a procedurally generated desert world on a plane, with it divided into distinct biomes and a city in the jungle](/FolioImages/world_showcase.png)

![Screenshot of the code behind some of the procedural generation, showing some of the math and relationship with temperature](/FolioImages/generation_code.png)

## Features:
 - Temperature dependant procedural generation
 - Chunking
 - Marching Squares
 - Detailed UI

 ![Screenshot of some of the chunk object code, including references to the Marching Squares algorithm](/FolioImages/chunk_code.png)

 ## Challenges Faced
 - Performance struggled greatly to begin with, had to learn optimization techniques such as chunking in order to get the world to generate and be played on
 - My experience with UI was still limited, after Red Riches I had to completely rethink how I approached and structured UI
 - I'd never played with meshes, pre-made or generated, had to learn concepts of 3D & how to construct meshes from the ground up
 - This was my first practise with procedural generation, had to learn difficult maths to drive the relationship between temperature and the game environment and generation techniques

![Screenshot of a sample of the world generation, highlighting the temperature value used to generate it](/FolioImages/world_gen_showcase_1.png)

![Another screenshot of a different sample of the world generation, highlighting the temperature value used to generate it](/FolioImages/world_gen_showcase_2.png)

 ## What I Learned
 - Optimization techniques, in order to fix the performance I developed a chunking method which massively reduced the load
 - Mesh generation & marching squares, I learnt the fundamentals of 3D objects and how they're rendered and how to construct a nice looking mesh using some custom interpolation
 - UI structures, how better to organize and communicate information to a more complex UI as well as the flow for opening different panels and elements
 - Procedural generation, I used a tile based method generated on a Sine wave with some custom smoothing applied to create the biomes

![Screenshot of a close up on the city, with the UI expanded showing details about the city and different production options](/FolioImages/ui_showcase_2.png)