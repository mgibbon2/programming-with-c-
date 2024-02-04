using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGibbonsBattleship {
    public class GameBoard {

        public char[,] battleshipBoard = new char[Constants.NUM_OF_ROWS, Constants.NUM_OF_COLUMNS];

        public bool cheatsEnabled = false;

        public GameBoard() {
            fillGameBoard();
        }
        /// <summary>
        /// Fills the gameboard with empty space characters.
        /// </summary>
        private void fillGameBoard() {
            for (int row = 0; row < battleshipBoard.GetLength(0); row++) {
                for (int column = 0; column < battleshipBoard.GetLength(1); column++) { 
                    battleshipBoard[row, column] = Constants.EMPTY_CHAR;
                }
            }
        }

        public void EnableCheats() {
            this.cheatsEnabled = true;
        }

        /// <summary>
        /// Prints the gameboard by iterating through it with headers labeling each row and column.
        /// </summary>
        public void PrintGameBoard() {
            int currentInt = 1;
            Console.Write("   ");
            for (int headerColumn = 0; headerColumn < battleshipBoard.GetLength(1); headerColumn++) {
                Console.Write(currentInt + "  ");
                currentInt++;
            }
            Console.WriteLine();
            // Starting at A in the utc-8.
            int currentChar = 65;
            for (int row = 0; row < battleshipBoard.GetLength(0); row++) {
                Console.Write((char)currentChar + " ");
                for (int column = 0; column < battleshipBoard.GetLength(1); column++) {
                    if (cheatsEnabled) {
                        Console.Write("[" + battleshipBoard[row, column] + "]");
                    }
                    else {
                        if (battleshipBoard[row, column] == Constants.SHIP_CHAR) {
                            Console.Write("[" + Constants.EMPTY_CHAR + "]");
                        }
                        else {
                            Console.Write("[" + battleshipBoard[row, column] + "]");
                        }
                    }
                }
                currentChar++;
                Console.WriteLine();
            }
        }
    }
}
