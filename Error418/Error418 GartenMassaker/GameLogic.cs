using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Error418_GartenMassaker {
	internal class GameLogic {

		private const int breite = 10;
        private Random rdm = new Random();
		private char[,] localField;
		private List<int> enemyLivingObjects;
		private Point lastMove;

		public GameLogic() {
			localField = new char[breite,breite];
		}

        public Point nextMove() {

			return new Point(rdm.Next(breite - 1) + 1, rdm.Next(breite - 1) + 1);
		}

		private Direction onHitBestDirection() {
			return Direction.Oben;
		}

		private void onKillBlocksAround() {

		}

		private Point determineNextDiagonalMove() {
			return new Point();
		}

		private bool checkObjectDestroyed() {
			return false;
		}

	}

	public enum Direction {
		Oben = 0,
		Unten = 1,
		Links = 2,
		Rechts = 3
	}
}
