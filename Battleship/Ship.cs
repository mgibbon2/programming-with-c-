using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGibbonsBattleship {
    public class Ship {
        private string type;
        private int length;
        private int life;
        private int hits = 0;

        // 0 is horizontal 1 is vertical.
        private int direction;

        private int bowXCoord;
        private int bowYCoord;
        private int sternXCoord;
        private int sternYCoord;
        
        /// <summary>
        /// Sets length and life equal to a the length and life of the hardcoded type value.
        /// </summary>
        /// <param name="type">Hardcoded value which is either "Destroyer", "Submarine", "Battleship", or "Carrier"</param>
        public Ship(string type) {
            this.type = type;
            switch (type) {
                case "Destroyer":
                    length = Constants.DESTROYER_LENGTH;
                    life = Constants.DESTROYER_LENGTH;
                    break;
                case "Submarine":
                    length = Constants.SUBMARINE_LENGTH;
                    life = Constants.SUBMARINE_LENGTH;
                    break;
                case "Battleship":
                    length = Constants.BATTLESHIP_LENGTH;
                    life = Constants.BATTLESHIP_LENGTH;
                    break;
                case "Carrier":
                    length = Constants.CARRIER_LENGTH;
                    life = Constants.CARRIER_LENGTH;
                    break;
            }
        }

        public int GetLength() {
            return length;
        }

        public string GetName() {
            return type;
        }

        public int GetDirection() {
            return direction;
        }

        public int GetBowX() {
            return bowXCoord;
        }

        public int GetBowY() {
            return bowYCoord;
        }

        public int GetSternX() {
            return sternXCoord;
        }

        public int GetSternY() {
            return sternYCoord;
        }

        public void SetDirection(int direction) {
            this.direction = direction;
        }

        public void SetBow(int xCoord, int yCoord) {
            bowXCoord = xCoord;
            bowYCoord = yCoord;
        }
        public void SetStern(int xCoord, int yCoord) {
            sternXCoord = xCoord;
            sternYCoord = yCoord;
        }

        public void incrementHits() {
            hits++;
        }

        /// <summary>
        /// If amount of items a ship has been hit is equal to the life of a ship then the ship has been sunk.
        /// </summary>
        /// <returns>returns true if ship was sunk</returns>
        public bool IsSunk() {
            bool sunk = false;
            if (hits == life) {
                sunk = true;
            }
            return sunk;
        }
    }
}
