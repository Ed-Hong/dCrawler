# dCrawler
2D Unity Rogue-Like 


# TODO
1. Enemy OnHit should only occur once at start of Turn

1. Implement Enemy Attacks
	* Enemies should always choose to attack over moving if possible (?)

1. Improve Enemy Movement AI
	* Currently, if two enemies calculate their next positions to be the same, the one with lower priority just doesn't move.
		* Ideally, the enemy with lower priority should attempt to move somewhere else.



# Behavioral Bugs
1. Player can be knocked-back into a space occupied by an enemy
	* Possible solutions:
		* Player's last position (the position they will be knocked back to) can never be occupied by an enemy
		* Knockbacks can chain - player being knocked back can then knockback an enemy

# Visual Bugs
1. Sword animation isn't correctly timed with Turn duration

# Ideas
* Instead of the Player having the WeaponAnimator, what if we made the Weapons class
have its own animator and then make the weapons prefabs?
	* You could then give them their own sprites and just draw it ontop of the player, and have it rotate/flip
with the player

