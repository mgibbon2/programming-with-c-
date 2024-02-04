using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MGibbonsBattleship {

    public class Game {

        private bool cheatsEnabled;
        private List<Ship> shipCollection;
        private GameBoard gameBoard;
        private int sunkenShips;
        private bool gameWon;

        /// <summary>
        /// The main logic is a do while loop that will continue making a new board and a new list of ships
        /// so long as the user wants to keep playing.
        /// </summary>
        public Game() {
            // While user wants to play more.
            bool started = promptForStart();
            do {
                if (started) {
                    cheatsEnabled = false;
                    shipCollection = new List<Ship>();
                    gameBoard = new GameBoard();
                    sunkenShips = 0;
                    gameWon = false;
                    CreateShips();
                    PlaceShip();
                    while (!gameWon) {
                        // If cheats are enabled it shouldn't prompt the user to enter cheats again.
                        if (cheatsEnabled) {
                            Console.Write("Enter coordinates for where you would like to fire: ");
                        }
                        else {
                            Console.Write("Enter coordinates for where you would like to fire. To enable cheats type \"" + Constants.CHEAT_CODE + "\": ");
                        }
                        string? userInput = Console.ReadLine();
                        // If the user only enters a new line character.
                        if (String.IsNullOrEmpty(userInput)) {
                            Console.WriteLine("\nYou did not input coordinates. Nice try.");
                        }
                        else {
                            VerifyInput(userInput);
                        }
                    }
                }
            }
            // while user keeps saying yes to play again.
            while (promptForEnd(started));
        }

        /// <summary>
        /// Prompts the user for if they want to start the game or not, also verifies that user input is
        /// either 'y' or 'n' or the upper case versions. If user doesn't want to start game then it ends
        /// the program.
        /// </summary>
        /// <returns>true if user specifices 'y' or 'Y'.</returns>
        private bool promptForStart() {
            bool userInputSuccesfull = false;
            bool yesOrNo = false;
            while (!userInputSuccesfull) {
                Console.Write("Start the game? (y/n): ");
                string? userInput = Console.ReadLine();
                if (String.IsNullOrEmpty(userInput)) {
                    Console.WriteLine("\nYou did not input a 'y' or an 'n'. Nice try.");
                }
                else if (Char.ToLower(userInput[0]) == 'n') {
                    userInputSuccesfull = true;
                }
                else if (Char.ToLower(userInput[0]) == 'y') {
                    userInputSuccesfull = true;
                    yesOrNo = true;
                }
                else {
                    Console.WriteLine("\nYou did not input a 'y' or an 'n'.");
                }
            }
            return yesOrNo;
        }

        /// <summary>
        /// Creates the ships given the amount of certain types of ships but uses hard coded names.
        /// </summary>
        private void CreateShips() {
            for (int i = 0; i < Constants.NUM_OF_DESTROYERS; i++) {
                Ship tempShip = new Ship("Destroyer");
                shipCollection.Add(tempShip);
            }
            for (int i = 0; i < Constants.NUM_OF_SUBMARINES; i++) {
                Ship tempShip = new Ship("Submarine");
                shipCollection.Add(tempShip);
            }
            for (int i = 0; i < Constants.NUM_OF_BATTLESHIPS; i++) {
                Ship tempShip = new Ship("Battleship");
                shipCollection.Add(tempShip);
            }
            for (int i = 0; i < Constants.NUM_OF_CARRIERS; i++) {
                Ship tempShip = new Ship("Carrier");
                shipCollection.Add(tempShip);
            }
        }

        /// <summary>
        /// Iterates through ship collection creating random coordinates for each ship, if the coordinates
        /// are already occupied or will end up in the ship falling off the edge then it will look for a
        /// new set of random coordinates. It's not very efficient but it's good enough, I hope.
        /// </summary>
        private void PlaceShip() {
            // Foreach to go through ship collection backwords starting with largest ship.
            foreach (Ship ship in shipCollection.AsEnumerable().Reverse()) {
                Random random = new Random();
                bool shipPlaced = false;
                while (!shipPlaced) {
                    // 0 for horizontal 1 for vertical.
                    // Had to use 2 for the second parameter because "0 is the only integer that is within the half-closed interval [0, 1). So, if you
                    // are actually interested in the integer values 0 or 1, then use 2 as upper bound." -> user Golo Roden on stack overflow.
                    int direction = random.Next(0, 2);
                    int randomX = random.Next(0, Constants.NUM_OF_COLUMNS);
                    int randomY = random.Next(0, Constants.NUM_OF_ROWS);
                    int shipLength = ship.GetLength();
                    if (checkIfInBounds(shipLength, direction, randomX, randomY)) {
                        if (checkOverlapping(shipLength, direction, randomX, randomY)) {
                            placeShipFromCoord(ship, shipLength, direction, randomX, randomY);
                            shipPlaced = true;
                        }
                    }
                }
            }
            gameBoard.PrintGameBoard();
        }

        /// <summary>
        /// Checks if the ship that is about to be placed is overflowing off of the edge of the board or not.
        /// </summary>
        /// <param name="shipLength">Length of the ship</param>
        /// <param name="direction">Either 0 or 1, for horizontal or vertical.</param>
        /// <param name="xCoord">X-coordinate of the front of the ship.</param>
        /// <param name="yCoord">Y-coordinate of the front of the ship.</param>
        /// <returns></returns>
        private bool checkIfInBounds(int shipLength, int direction, int xCoord, int yCoord) {
            bool inBounds = false;
            if (direction == 0) {
                // Needed + 1 on num of columns because the horizontal ships never made it to the furthest column.
                if (xCoord + shipLength < Constants.NUM_OF_COLUMNS + 1) {
                    inBounds = true;
                }
            }
            else {
                // Same reason for + 1, the vertical ships never made it to the furthest row.
                if (yCoord + shipLength < Constants.NUM_OF_ROWS + 1) {
                    inBounds = true;
                }
            }
            return inBounds;
        }

        /// <summary>
        /// Checks if a ship that is about to be placed is going to be overlapping with an already placed ship
        /// or not.
        /// </summary>
        /// <param name="shipLength">Length of the ship</param>
        /// <param name="direction">Either 0 or 1, for horizontal or vertical.</param>
        /// <param name="xCoord">X-coordinate of the front of the ship.</param>
        /// <param name="yCoord">Y-coordinate of the front of the ship.</param>
        /// <returns></returns>
        private bool checkOverlapping(int shipLength, int direction, int xCoord, int yCoord) {
            bool notOverlapped = true;
            for (int i = 0; i < shipLength; i++) {
                // If diection is horizontal.
                if (direction == 0) {
                    if (gameBoard.battleshipBoard[xCoord + i, yCoord] == Constants.SHIP_CHAR) {
                        notOverlapped = false;
                    }
                }
                else {
                    if (gameBoard.battleshipBoard[xCoord, yCoord + i] == Constants.SHIP_CHAR) {
                        notOverlapped = false;
                    }
                }
            }
            return notOverlapped;
        }

        /// <summary>
        /// Places and asigns the values for the bow and stern for a ship from the length, direction,
        /// and coordinates.
        /// </summary>
        /// <param name="ship">Ship to be able to set bow and stern with coordinates.</param>
        /// <param name="shipLength">Length of specific ship.</param>
        /// <param name="direction">Either 0 or 1, for horizontal or vertical.</param>
        /// <param name="xCoord">X-coordinate for the front of the ship.</param>
        /// <param name="yCoord">Y-coordinate for the front of the ship.</param>
        private void placeShipFromCoord(Ship ship, int shipLength, int direction, int xCoord, int yCoord) {
            ship.SetDirection(direction);
            // If direction is horizontal.
            if (direction == 0) {
                ship.SetBow(xCoord, yCoord);
                for (int i = 0; i < shipLength; i++) {
                    gameBoard.battleshipBoard[xCoord + i, yCoord] = Constants.SHIP_CHAR;
                    if (i == shipLength - 1) {
                        ship.SetStern(xCoord + i, yCoord);
                    }
                }
            }
            else {
                ship.SetBow(xCoord, yCoord);
                for (int i = 0; i < shipLength; i++) {
                    gameBoard.battleshipBoard[xCoord, yCoord + i] = Constants.SHIP_CHAR;
                    if (i == shipLength - 1) {
                        ship.SetStern(xCoord, yCoord + i);
                    }
                }
            }
        }

        /// <summary>
        /// Parses a string into coordinates and then verifies that it is in the range of the board. 
        /// Then calls the method UpdateBoard with the parsed coordinates.
        /// </summary>
        /// <param name="userInput">a raw string that will look something like A4 or J10</param>
        public void VerifyInput(string userInput) {
            // Tests that the input string is something like A12 and not A1b.
            bool validInput = true;
            for (int i = 1; i < userInput.Length; i++) {
                if (!Char.IsNumber(userInput[i])) {
                    validInput = false;
                }
            }
            // If cheats are currently disabled and user input is the cheat code.
            if (userInput == Constants.CHEAT_CODE && cheatsEnabled == false) {
                cheatsEnabled = true;
                gameBoard.EnableCheats();
                Console.WriteLine("\nCheats are now enabled.");
                gameBoard.PrintGameBoard();
            }
            else if (!validInput | userInput.Length < 2) {
                Console.WriteLine("\nYou did not input correct coordinates. Such as \"A1\".");
            }
            // If xcoord < total columns & ycoord < total rows but both are >= 0.
            else if (Convert.ToInt32(userInput.Trim(userInput[0])) - 1 < Constants.NUM_OF_COLUMNS && userInput[0] - 65 < Constants.NUM_OF_ROWS &&
                     Convert.ToInt32(userInput.Trim(userInput[0])) - 1 >= 0 && userInput[0] - 65 >= 0) {
                int yCoord = Convert.ToInt32(userInput.Trim(userInput[0])) - 1;
                int xCoord = userInput[0] - 65;
                // If current char is hit or missed then it has already been guessed.
                if (gameBoard.battleshipBoard[xCoord, yCoord] == Constants.HIT_CHAR | gameBoard.battleshipBoard[xCoord, yCoord] == Constants.MISS_CHAR) {
                    Console.WriteLine("\nYou have already guessed this coordinate.");
                }
                else {
                    UpdateBoard(xCoord, yCoord);
                }
            }
            else {
                Console.WriteLine("\nThe coordinates entered are out of range.");
            }
        }

        /// <summary>
        /// Updates the board with a different character for if it hit or missed.
        /// </summary>
        /// <param name="xCoord">X-coordinate of guess</param>
        /// <param name="yCoord">Y-coordinate of guess</param>
        private void UpdateBoard(int xCoord, int yCoord) {
            if (gameBoard.battleshipBoard[xCoord, yCoord] == Constants.SHIP_CHAR) {
                gameBoard.battleshipBoard[xCoord, yCoord] = Constants.HIT_CHAR;
                if (!DidShipSink(xCoord, yCoord)) {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Hit!");
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
            else {
                gameBoard.battleshipBoard[xCoord, yCoord] = Constants.MISS_CHAR;
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("Miss!");
                Console.ForegroundColor = ConsoleColor.White;
            }
            gameBoard.PrintGameBoard();
        }

        /// <summary>
        /// The method is pretty much two parts, if the ship is vertical or horizontal. It iterates through
        /// the ship collection and compares the bow and stern values to see which type of ship it is.
        /// </summary>
        /// <param name="xCoord">X-coordinate of the last hit</param>
        /// <param name="yCoord">Y-corrdinate of the last hit</param>
        /// <returns>false if ship a ship didn't sink</returns>
        private bool DidShipSink(int xCoord, int yCoord) {
            bool shipSunk = false;
            foreach (Ship ship in shipCollection) {
                // If ship direction is horizontal.
                if (ship.GetDirection() == 0) {
                    if (ship.GetBowX() <= xCoord && ship.GetSternX() >= xCoord && ship.GetBowY() == yCoord) {
                        ship.incrementHits();
                        if (ship.IsSunk()) {
                            shipSunk = true;
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("Hit and you have sunken a " + ship.GetName() + "!");
                            Console.ForegroundColor = ConsoleColor.White;
                            sunkenShips++;
                            TestForWinCondition();
                        }
                    }
                }
                else {
                    if (ship.GetBowY() <= yCoord && ship.GetSternY() >= yCoord && ship.GetBowX() == xCoord) {
                        ship.incrementHits();
                        if (ship.IsSunk()) {
                            shipSunk = true;
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("Hit and you have sunken a " + ship.GetName() + "!");
                            Console.ForegroundColor = ConsoleColor.White;
                            sunkenShips++;
                            TestForWinCondition();
                        }
                    }
                }
            }
            return shipSunk;
        }

        /// <summary>
        /// If the total amount of sunken ships is equal to the total number of ships then win game and display
        /// the win message.
        /// </summary>
        private void TestForWinCondition() {
            if (sunkenShips == Constants.TOTAL_NUM_SHIPS) {
                gameWon = true;
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("You have won the game!");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }
        /// <summary>
        /// Prompts the user for if they want to play another game or not, also verifies that user input is
        /// either 'y' or 'n' or the upper case versions. 
        /// </summary>
        /// <param name="started">for making sure the user did not say no to starting the game if it wasn't started then
        /// this whole method will be skipped and will return false.</param>
        /// <returns></returns>
        private bool promptForEnd(bool started) {
            bool yesOrNo = false;
            if (started) {
                bool userInputSuccesfull = false;
                while (!userInputSuccesfull) {
                    Console.Write("Play another game? (y/n): ");
                    string? userInput = Console.ReadLine();
                    if (String.IsNullOrEmpty(userInput)) {
                        Console.WriteLine("\nYou did not input a 'y' or an 'n'. Nice try.");
                    }
                    else if (Char.ToLower(userInput[0]) == 'n') {
                        userInputSuccesfull = true;
                    }
                    else if (Char.ToLower(userInput[0]) == 'y') {
                        userInputSuccesfull = true;
                        yesOrNo = true;
                    }
                    else {
                        Console.WriteLine("\nYou did not input a 'y' or an 'n'.");
                    }
                }
            }
            return yesOrNo;
        }
    }   
}
