using MGibbonsBattleship;
// Starts the actual heart of the program.
Game niceGame = new Game();

/// <summary>
/// Purely to define public constants in the program's parameters (for maintainability).
/// </summary>
static class Constants {
    // Defines the game board grid.
    public const int NUM_OF_ROWS = 10;
    public const int NUM_OF_COLUMNS = 10;

    public const int DESTROYER_LENGTH = 2;
    public const int SUBMARINE_LENGTH = 3;
    public const int BATTLESHIP_LENGTH = 4;
    public const int CARRIER_LENGTH = 5;

    public const int TOTAL_NUM_SHIPS = 6;

    public const int NUM_OF_DESTROYERS = 2;
    public const int NUM_OF_SUBMARINES = 2;
    public const int NUM_OF_BATTLESHIPS = 1;
    public const int NUM_OF_CARRIERS = 1;

    // Which char to display on board.
    public const char EMPTY_CHAR = ' ';
    public const char HIT_CHAR = 'X';
    public const char MISS_CHAR = 'O';
    public const char SHIP_CHAR = 'S';

    // Cheat code to be entered during prompt for entering coordinates.
    public const string CHEAT_CODE = "Cheats";
}