using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Error418_GartenMassaker
{
	internal class GameLogicV2
	{

		//The width (and hight) of the playfield
		private const int fieldSize = 10;

		//Randomizer that is used for nextDiagonalChessFieldMove determination
		private Random rnd = new Random();

		//A local variable for our own playfield, its a 2-dimensional char array
		private char[,] localField;

		//A local field that makes a diagonal Chess board to go over.
		private char[,] chessField;

		//A list that tracks the remaining objects from the enemy
		private List<int> enemyLivingObjects;

		//A variable that remebers the last Move that was done
		private Point lastMove;

		private char lastAction;

		private Point? lastHit;
		private bool firstHitTaken;
		private List<Point>? nextToHitList;
		private int countForLenghtOfKilledShip;
		private DirectionV2 currentDirection;
		private bool hunt;

		//This Object is responsible for all the GameLogic, which is everything that is using for playing the Game itself.
		//It computes the best possible next Move to be done and remebers the Playfield seperately.
		public GameLogicV2()
		{
			localField = new char[fieldSize, fieldSize];
			createLocalField();
			countForLenghtOfKilledShip = 0;
			enemyLivingObjects = new List<int> { 5, 4, 3, 3, 2 };
			hunt = false;
			lastHit = null;
			firstHitTaken = false;
			nextToHitList = null;
			lastAction = ' ';
			createChessBoard();
		}

		//Main Method for the Game that determines the next Move.
		//Every other Method from this Class gets called from here.
		//RETURNS: the next Coordinate that is to be shot at.
		public Point nextMove(char[,] updatedField)
		{

			Point nextMove = new Point();

			lastAction = updatedField[lastMove.X, lastMove.Y];
			localField[lastMove.X, lastMove.Y] = lastAction;
			chessField[lastMove.X, lastMove.Y] = '.';

			if (!lastAction.Equals(' '))
			{
				switch (lastAction)
				{
					case '.':
						if (hunt)
						{
							if (firstHitTaken)
							{
								nextMove = FindSecondTarget(currentDirection);
							}
							else
							{
								FindPossibleShipPositionsByHits();
								nextMove = nextToHitList.First();
							}
						}
						else
						{
							nextMove = determineNextDiagonalMove();
						}
						break;
					case 'x':
						hunt = true;
						lastHit = lastMove;
						if (!firstHitTaken)
						{
							firstHitTaken = true;
							nextMove = FindSecondTarget(currentDirection);
						}
						else
						{
							firstHitTaken = false;
							FindPossibleShipPositionsByHits();
							nextMove = nextToHitList.First();
						}
						break;
					case 'X':
						hunt = false;
						lastHit = null;
						firstHitTaken = false;
						UpdateDestroyedShips(updatedField);
						OnKillBlocksAround();
						currentDirection = DirectionV2.Up;
						nextMove = determineNextDiagonalMove();
						break;
				}
			}
			else
			{
				nextMove = determineNextDiagonalMove();
			}

			lastMove = nextMove;
			return nextMove;
		}
		// Find all possible points to next hit and then assign them to the hitlist
		private void FindPossibleShipPositionsByHits() //NEEEDS FIXING //DOWN FOR TODO COMMENT
		{
			List<Point> hits = new List<Point>();
			List<Point> misses = new List<Point>();
			List<Point> points = new List<Point>();
			string axis = string.Empty;
			int axisId = 0;

			//intialize all current open hits
			for (int i = 0; i < fieldSize; i++)
			{
				for (int j = 0; j < fieldSize; j++)
				{
					if (localField[i, j].Equals('x')) hits.Add(new Point(i, j));
					if (localField[i, j].Equals('.') || localField[i, j].Equals('X')) misses.Add(new Point(i, j));
				}
			}

			//if the first two hits are On the X axis
			if (hits[0].X == hits[1].X)
			{
				axis = "x";
				axisId = hits[0].X;
			}
			//if the first two hits are On the Y axis
			if (hits[0].Y == hits[1].Y)
			{
				axis = "y";
				axisId = hits[0].Y;
			}

			//add all hits on axis and order them by distance from last hit
			switch (axis)
			{
				case "x":
					for (int i = 0; i < fieldSize; i++)
					{
						if (hits.FindAll(hit => hit.X == axisId).Any(hit => hit.Y == i)) continue;
						if (misses.FindAll(miss => miss.X == axisId).Any(miss => miss.Y == i)) continue;
						points.Add(new Point(axisId, i));
					}
					points = points.OrderBy(p => Math.Abs(p.X - ((Point)lastHit).X)).ToList();
					break;
				case "y":
					for (int i = 0; i < fieldSize; i++)
					{
						if (hits.FindAll(hit => hit.Y == axisId).Any(hit => hit.X == i)) continue;
						if (misses.FindAll(miss => miss.Y == axisId).Any(miss => miss.X == i)) continue;
						points.Add(new Point(i, axisId));
					}
					points = points.OrderBy(p => Math.Abs(p.X - ((Point)lastHit).Y)).ToList();
					break;
			}

			nextToHitList = points;
		}
		private Point FindSecondTarget(DirectionV2 direction)
		{
			Point returnPoint = new Point(0, 0);
			bool targetFound = false;
			DirectionV2 facing = direction;
			Point firstHit = (Point)lastHit;
			int tries = 0;

			while (!targetFound)
			{
				tries++;
				if (tries >= 5)
				{
					returnPoint = determineNextDiagonalMove();
					break;
				}
				if (facing == DirectionV2.Up)
				{
					if (firstHit.Y == 0)
					{
						facing = RotateDirection(facing);
						continue;
					}
					if (localField[firstHit.X, firstHit.Y - 1].Equals(' '))
					{
						returnPoint = new Point(firstHit.X, firstHit.Y - 1);
						targetFound = true;
					}
					else
					{
						facing = RotateDirection(facing);
						continue;
					}
				}
				if (facing == DirectionV2.Right)
				{
					if (firstHit.X == 9)
					{
						facing = RotateDirection(facing);
						continue;
					}
					if (localField[firstHit.X + 1, firstHit.Y].Equals(' '))
					{
						returnPoint = new Point(firstHit.X + 1, firstHit.Y);
						targetFound = true;
					}
					else
					{
						facing = RotateDirection(facing);
						continue;
					}
				}
				if (facing == DirectionV2.Down)
				{
					if (firstHit.Y == 9)
					{
						facing = RotateDirection(facing);
						continue;
					}
					if (localField[firstHit.X, firstHit.Y + 1].Equals(' '))
					{
						returnPoint = new Point(firstHit.X, firstHit.Y + 1);
						targetFound = true;
					}
					else
					{
						facing = RotateDirection(facing);
						continue;
					}
				}
				if (facing == DirectionV2.Left)
				{
					if (firstHit.X == 0)
					{
						facing = RotateDirection(facing);
						continue;
					}
					if (localField[firstHit.X - 1, firstHit.Y].Equals(' '))
					{
						returnPoint = new Point(firstHit.X - 1, firstHit.Y);
						targetFound = true;
					}
					else
					{
						facing = RotateDirection(facing);
						continue;
					}
				}
			}
			currentDirection = facing;
			return returnPoint;
		}
		// Rotates 90°
		private DirectionV2 RotateDirection(DirectionV2 direction)
		{
			if (direction == DirectionV2.Up)
			{
				return DirectionV2.Right;
			}
			if (direction == DirectionV2.Right)
			{
				return DirectionV2.Down;
			}
			if (direction == DirectionV2.Down)
			{
				return DirectionV2.Left;
			}
			if (direction == DirectionV2.Left)
			{
				return DirectionV2.Up;
			}
			return direction;
		}

		//When a "Ship" is destroyed successfully, the localField blocks all the blocks around the ship because its impossible that there is another
		private void OnKillBlocksAround()
		{

			//This goes through the whole Field and when he finds an X he sets all around it, except Xs, to '.'
			for (int zeile = 0; zeile < fieldSize; zeile++)
			{
				for (int spalte = 0; spalte < fieldSize; spalte++)
				{
					if (localField[zeile, spalte].Equals('X'))
					{
						if (zeile > 0 && zeile < fieldSize && !localField[zeile - 1, spalte].Equals('X'))
						{
							chessField[zeile - 1, spalte] = '.';
							localField[zeile - 1, spalte] = '.';
						}
						if (spalte > 0 && spalte < fieldSize && !localField[zeile, spalte - 1].Equals('X'))
						{
							chessField[zeile, spalte - 1] = '.';
							localField[zeile, spalte - 1] = '.';
						}
					}
				}
			}
		}
		//If the last shot didnt hit, or a ship is completely destroyed, this Method determines the next Tile to shot at.
		//It may use a private new Playfield that already has a Chess-Pattern to follow. Chess Pattern is the best for this bc of reasons.
		//RETURNS: the next random Point to shoot at
		private Point determineNextDiagonalMove()
		{

			//This gets a list of all Free Points of the ChessField
			List<Point> freeTiles = new List<Point>();
			for (int i = 0; i < fieldSize; i++)
			{
				for (int k = 0; k < fieldSize; k++)
				{

					if (chessField[i, k].Equals(' '))
						freeTiles.Add(new Point(i, k));
				}
			}

			//This picks one of them at random
			Point nextPoint = freeTiles[rnd.Next(0, freeTiles.Count)];
			chessField[nextPoint.X, nextPoint.Y] = '.';

			return nextPoint;
		}

		//This method creates a new Board that only Tracks the randomTiles to go through, Chess-Strat is the best
		private void createChessBoard()
		{
			this.chessField = new char[fieldSize, fieldSize];

			for (int i = 0; i < fieldSize; i++)
			{
				for (int k = 0; k < fieldSize; k++)
				{
					chessField[i, k] = ' ';
				}
			}

			for (int zeile = 0; zeile < fieldSize; zeile++)
			{
				for (int spalte = 0; spalte < fieldSize; spalte = spalte + 2)
				{
					if (zeile % 2 == 0)
					{
						chessField[zeile, spalte] = '.';
					}
					else
					{
						chessField[zeile, spalte + 1] = '.';
					}
				}
			}
		}
		private void createLocalField()
		{
			for (int i = 0; i < fieldSize; i++)
			{
				for (int j = 0; j < fieldSize; j++)
				{
					localField[i, j] = ' ';
				}
			}
		}
		private void UpdateDestroyedShips(char[,] board)
		{
			for (int i = 0; i < fieldSize; i++)
			{
				for (int j = 0; j < fieldSize; j++)
				{
					if (board[i, j].Equals('X')) localField[i, j] = 'X';
				}
			}
		}

	}

	//An Enum to represent Directions
	public enum DirectionV2
	{
		Up = 0,
		Down = 1,
		Left = 2,
		Right = 3
	}
}
