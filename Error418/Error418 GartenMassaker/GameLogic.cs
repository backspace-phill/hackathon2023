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

		//Randomizer that is used for nextDiagonalChessFieldMove determination
        private Random rdm = new Random();

		//A local variable for our own playfield, its a 2-dimensional char array
		private char[,] localField;

		//A local field that makes a diagonal Chess board to go over.
		private char[,] chessField;

		//A list that tracks the remaining objects from the enemy
		private List<int> enemyLivingObjects;

		//A variable that remebers the last Move that was done
		private Point lastMove;

		private char resultFromLastMove;

		private Point? lastHit;
        private int countForLenghtOfKilledShip;
		private Direction afterHitDirection;
		private bool killingSpree;

		//This Object is responsible for all the GameLogic, which is everything that is using for playing the Game itself.
		//It computes the best possible next Move to be done and remebers the Playfield seperately.
		public GameLogic() {
			localField = new char[breite,breite];
			countForLenghtOfKilledShip = 0;
			enemyLivingObjects = new List<int> {5,4,3,3,2};
			killingSpree = false;
			lastHit = null;
			resultFromLastMove = ' ';
		}

		//Main Method for the Game that determines the next Move.
		//Every other Method from this Class gets called from here.
		//RETURNS: the next Coordinate that is to be shot at.
        public Point nextMove(char[,] resultSetField) {

			Point nextMove = new Point();

			if(!resultFromLastMove.Equals(' ')) {
				resultFromLastMove = resultSetField[lastMove.X,lastMove.Y];
				localField[lastMove.X, lastMove.Y] = resultFromLastMove;
				chessField[lastMove.X, lastMove.Y] = '.';

				if (resultFromLastMove.Equals('.')) {
					if (killingSpree) {
						lastMove = (Point)lastHit;
						afterHitDirection = onHitBestDirection();
						nextMove = getNextFreeTileInDirection(afterHitDirection);
					}else {
						nextMove = determineNextDiagonalMove();
					}
				}
				else if (resultFromLastMove.Equals('x')) {
					killingSpree = true;
					lastHit = lastMove;
					afterHitDirection = onHitBestDirection();
					nextMove = getNextFreeTileInDirection(afterHitDirection);
				}else if (resultFromLastMove.Equals('X')) {
					killingSpree = false;
					lastHit = null;
					onKillBlocksAround();
					nextMove = determineNextDiagonalMove();
				}

			}
			else {
				nextMove = determineNextDiagonalMove();
			}

			if (nextMove.IsEmpty) {
				nextMove = emergencyGetFreeTile();
			}
			lastMove = nextMove;
            return lastMove;
		}

		private Point? getNextFreeTileInDirection(Direction direction) {

			bool dot = false;
			int moves = 0;
			Point returnPoint = lastMove;

			while (!dot) {

				//oben
                if (direction == Direction.Oben) {
                    if (!localField[lastMove.X, lastMove.Y + moves - 1].Equals('.') && lastMove.Y-1<0) {
						moves++;
					}
					returnPoint = new Point(lastMove.X, lastMove.Y - moves);
                }

				//unten
                if (direction == Direction.Unten) {
                    if (!localField[lastMove.X, lastMove.Y + moves + 1].Equals('.') && lastMove.Y + 1 > breite-1) {
                        moves++;
                    }
                    returnPoint = new Point(lastMove.X, lastMove.Y + moves);
                }

				//links
                if (direction == Direction.Links) {
                    if (!localField[lastMove.X - moves -1, lastMove.Y].Equals('.') && lastMove.X - 1 < 0) {
                        moves++;
                    }
                    returnPoint = new Point(lastMove.X - moves, lastMove.Y);
                }

                if (direction == Direction.Rechts) {
                    if (!localField[lastMove.X + moves + 1, lastMove.Y].Equals('.') && lastMove.X + 1 > breite-1) {
                        moves++;
                    }
                    returnPoint = new Point(lastMove.X + moves, lastMove.Y);
                }
            }

			if (returnPoint == lastMove) {
				return null;
			}

			return returnPoint;
		}

		private Point emergencyGetFreeTile() {

            //This gets a list of all Free Points of the ChessField
            List<Point> freeTiles = new List<Point>();
            for (int i = 0; i < breite; i++) {
                for (int k = 0; k < breite; k++) {

                    if (localField[i, k].Equals(' '))
                        freeTiles.Add(new Point(i, k));
                }
            }

            //This picks one of them at random
            Point nextPoint = freeTiles[rdm.Next(0, freeTiles.Count)];
            localField[nextPoint.X, nextPoint.Y] = '.';

            return nextPoint;
		}

		//If a "Ship" has been hit, returns the best possible Direction where to fire next.
		//If the longest Ship ist the 5 long one, and from the last shot you can only go 2 steps top,
		//while you can go 4 steps down, the direction "down" will be returned because its the most probable to hit another tile.
		//RETURNS: the best possible direction to go next, after a "ship" has been hit and it is not destroyed
		private Direction onHitBestDirection() {
			Direction bestDirection;
			int bestDirectionFreeTiles = 0;
			int freeTilesInDirection = 0;

			bool dot = false;

			//oben
			while (!dot) {
				if (lastMove.Y > 0 && localField[lastMove.X, lastMove.Y - freeTilesInDirection - 1].Equals(' ')) {
					freeTilesInDirection++;
				}
				else {
					dot = true;
				}
			}
			bestDirection = Direction.Oben;
			bestDirectionFreeTiles = freeTilesInDirection;

			//unten
			dot = false;
			freeTilesInDirection = 0;
            while (!dot) {
                if (lastMove.Y < breite-1 && localField[lastMove.X, lastMove.Y + freeTilesInDirection + 1].Equals(' ')) {
                    freeTilesInDirection++;
                }
            }

			if(freeTilesInDirection > bestDirectionFreeTiles) {
				bestDirectionFreeTiles = freeTilesInDirection;
				bestDirection = Direction.Unten;
			}

            //links
            dot = false;
            freeTilesInDirection = 0;
            while (!dot) {
                if (lastMove.X > 0 && localField[lastMove.X - freeTilesInDirection - 1, lastMove.Y].Equals(' ')) {
                    freeTilesInDirection++;
                }
            }

            if (freeTilesInDirection > bestDirectionFreeTiles) {
                bestDirectionFreeTiles = freeTilesInDirection;
                bestDirection = Direction.Unten;
            }

            //rechts
            dot = false;
            freeTilesInDirection = 0;
            while (!dot) {
                if (lastMove.X < breite-1 && localField[lastMove.X + freeTilesInDirection + 1, lastMove.Y].Equals(' ')) {
                    freeTilesInDirection++;
                }
            }

            if (freeTilesInDirection > bestDirectionFreeTiles) {
                bestDirection = Direction.Unten;
            }

            afterHitDirection = bestDirection;
            return bestDirection;
		}

		//When a "Ship" is destroyed successfully, the localField blocks all the blocks around the ship because its impossible that there is another
		private void onKillBlocksAround() {

			//This goes through the whole Field and when he finds an X he sets all around it, except Xs, to '.'
			for (int zeile = 0; zeile < breite; zeile++) {
				for (int spalte = 0; spalte < breite; spalte++) {
					if (localField[zeile,spalte].Equals('X')) {
						if (zeile > 0 && !localField[zeile-1,spalte].Equals('X')) {
                            chessField[zeile - 1, spalte] = '.';
                            localField[zeile - 1, spalte] = '.';
						}
						if (zeile < breite-1 && !localField[zeile + 1, spalte].Equals('X')) {
                            chessField[zeile + 1, spalte] = '.';
                            localField[zeile + 1, spalte] = '.';
						}

                        if (spalte > 0 && !localField[zeile, spalte-1].Equals('X')) {
                            chessField[zeile, spalte - 1] = '.';
                            localField[zeile, spalte - 1] = '.';
                        }
                        if (zeile < breite - 1 && !localField[zeile, spalte + 1].Equals('X')) {
                            chessField[zeile, spalte + 1] = '.';
                            localField[zeile, spalte + 1] = '.';
                        }
                    }
				}
			}
		}

		//If the last shot didnt hit, or a ship is completely destroyed, this Method determines the next Tile to shot at.
		//It may use a private new Playfield that already has a Chess-Pattern to follow. Chess Pattern is the best for this bc of reasons.
		//RETURNS: the next random Point to shoot at
		private Point determineNextDiagonalMove() {

			//This gets a list of all Free Points of the ChessField
			List<Point> freeTiles = new List<Point>();
			for (int i = 0; i < breite; i++) {
				for (int k = 0; k < breite; k++) {

					if (chessField[i, k].Equals(' '))
						freeTiles.Add(new Point(i, k));
				}
			}

			//This picks one of them at random
			Point nextPoint = freeTiles[rdm.Next(0,freeTiles.Count)];
			chessField[nextPoint.X, nextPoint.Y] = '.';

            return nextPoint;
		}

		//Checks if the ship, that has been hit, is now completely destroyed (x is now X).
		//Returns wheather that is the case or not
		private bool checkObjectDestroyed() {

			if (resultFromLastMove.Equals('X')) {
				return true;
			}
			return false;

		}


		//This method creates a new Board that only Tracks the randomTiles to go through, Chess-Strat is the best
		private void createChessBoard() {
			this.chessField = new char[breite, breite];

			for (int i = 0; i< breite; i++){
                for (int k = 0; k < breite; k++) {
					chessField[i, k] = ' ';
                }
            }

			for (int zeile = 0; zeile < breite; zeile++) {
                for (int spalte = 0; spalte < breite; spalte = spalte+2) {
					if(zeile%2 == 0) {
						chessField[zeile, spalte] = '.';
					}
					else {
						chessField[zeile, spalte+1] = '.';
					}
                }
            }
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
