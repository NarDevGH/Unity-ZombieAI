# Zombie_Proyect
Done(but needs refactoring)
- Go towards ramdom sound.
- Go Toward the highest priority sound.
- Chases the player when he sees him.
- Chase/attack the closest player.
- cant see player through walls or obstacles that are in the obstacle layer.

- can jump and crawl in specific areas. (Zombie will do this only if its in agro)
- can go up stairs

- keep looking for the player after stop seen him.
- try search for the player X Times

-  turn smoothly when changing direction

To Do:



Notes:
- The navmesh areas are used to avoid jumping or crawling if it's unnecessary
- the whole body of the zombie turns towards the corners of the path. Since its pivot is on the soles of its feet, it allows it to move without problems. but if its 
  somewhere else, the zombie would behave strangely.
 -The rotation is given by the path update time and the angularAmount
