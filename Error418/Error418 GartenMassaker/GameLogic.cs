using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Error418_GartenMassaker {
	internal class GameLogic {

		//The width (and hight) of the playfield
		private const int breite = 10;

		//Randomizer that is to be deleted when not used anymore
        private Random rdm = new Random();

		//A local variable for our own playfield, its a 2-dimensional char array
		private char[,] localField;

		//A list that tracks the remaining objects from the enemy
		private List<int> enemyLivingObjects;

		//A variable that remebers the last Move that was done
		private Point lastMove;

		//This Object is responsible for all the GameLogic, which is everything that is using for playing the Game itself.
		//It computes the best possible next Move to be done and remebers the Playfield seperately.
		public GameLogic() {
			localField = new char[breite,breite];
		}

		//Main Method for the Game that determines the next Move.
		//Every other Method from this Class gets called from here.
		//RETURNS: the next Coordinate that is to be shot at.
        public Point nextMove() {

			return new Point(rdm.Next(breite - 1) + 1, rdm.Next(breite - 1) + 1);
		}

		//If a "Ship" has been hit, returns the best possible Direction where to fire next.
		//If the longest Ship ist the 5 long one, and from the last shot you can only go 2 steps top,
		//while you can go 4 steps down, the direction "down" will be returned because its the most probable to hit another tile.
		//RETURNS: the best possible direction to go next, after a "ship" has been hit and it is not destroyed
		private Direction onHitBestDirection() {
			return Direction.Oben;
		}

		//When a "Ship" is destroyed successfully, the localField blocks all the blocks around the ship because its impossible that there is another
		private void onKillBlocksAround() {

		}

		//If the last shot didnt hit, or a ship is completely destroyed, this Method determines the next Tile to shot at.
		//It may use a private new Playfield that already has a Chess-Pattern to follow. Chess Pattern is the best for this bc of reasons.
		//RETURNS: the next random Point to shoot at
		private Point determineNextDiagonalMove() {
			return new Point();
		}

		//Checks if the ship, that has been hit, is now completely destroyed (x is now X).
		//Returns wheather that is the case or not
		private bool checkObjectDestroyed() {
			return false;
		}

	}

	//An Enum to represent Directions
	public enum Direction {
		Oben = 0,
		Unten = 1,
		Links = 2,
		Rechts = 3
	}
}
