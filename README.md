# dCrawler
2D Unity Rogue-Like 


# TODO
1. Finish Implementing Enemy Movement
	* On TileGeneration, when EnemyPrefabs are generated, populate list of ActiveEnemies
	* Order ActiveEnemies by Priority
	* Each Enemy moves according to its priority
		* For Enemies of Equal Priority, the first one encountered gets the move

1. Implement Enemy Attacks
	* Enemies should always choose to attack over moving if possible (?)

1. Enemy OnHit should only occur once at start of Turn


# Behavioral Bugs


# Visual Bugs
1. Sword animation isn't correctly timed with Turn duration

# Ideas
* Instead of the Player having the WeaponAnimator, what if we made the Weapons class
have its own animator and then make the weapons prefabs?
	* You could then give them their own sprites and just draw it ontop of the player, and have it rotate/flip
with the player

