using LegallyNotWordle.Model;
using Microsoft.VisualBasic.Logging;
using System.Data.Common;
using System.Drawing;
using System.Runtime.InteropServices;

namespace LegallyNotWordle {
    public partial class WordleWindow : Form {

        TextBox[,] theGrid;
        WordleLogic logic;
        int currentGuessNum = 0;
        bool deletingLetters = false;

        public WordleWindow() {
            this.BackColor = Constants.NICE_BACKGROUND;
            InitializeComponent();
            InitializeTitle();
            ResetBoard();
        }

        /// <summary>
        /// initializes the title label and the guess button
        /// </summary>
        public void InitializeTitle() {
            Label titleBox = new Label();
            titleBox.Location = new Point(0, 0);
            titleBox.AutoSize = false;
            titleBox.Size = new Size(Constants.WORD_LENGTH * Constants.TEXT_DIMENSION + 400, Constants.TEXT_DIMENSION);
            titleBox.BackColor = Constants.NICE_BACKGROUND;
            titleBox.ForeColor = Constants.NICE_FOREGROUND;
            titleBox.Font = new Font(Constants.FONT, Constants.PRIMARY_FONT_SIZE);
            titleBox.TextAlign = ContentAlignment.MiddleCenter;
            titleBox.Text = "wordle";
            this.Controls.Add(titleBox);

            Button guessButton = new Button();
            guessButton.Location = new Point(Constants.WORD_LENGTH * Constants.TEXT_DIMENSION, Constants.TEXT_DIMENSION);
            guessButton.Size = new Size(400, Constants.TEXT_DIMENSION);
            guessButton.BackColor = Constants.NICE_FOREGROUND;
            guessButton.ForeColor = Constants.NICE_SECONDARY;
            guessButton.Click += Guess_Button_Clicked;
            guessButton.Text = "GUESS";
            guessButton.TextAlign = ContentAlignment.MiddleCenter;
            guessButton.Font = new Font(Constants.FONT, Constants.PRIMARY_FONT_SIZE / 2);
            this.Controls.Add(guessButton);
        }

        /// <summary>
        /// resets the board, and calls other methods to initialize other things
        /// </summary>
        private void ResetBoard() {
            logic = new WordleLogic();
            theGrid = new TextBox[Constants.GUESS_LENGTH, Constants.WORD_LENGTH];
            InitializeTxtBoxes();
            // comment out this line if you don't want cheats
            InitializeCheats();
        }

        /// <summary>
        /// fills the grid with textboxes and changes a lot of aspects for each textbox
        /// </summary>
        public void InitializeTxtBoxes() {
            Size txtBoxSize = new Size(Constants.TEXT_DIMENSION, Constants.TEXT_DIMENSION);
            for (int row = 0; row < theGrid.GetLength(0); row++) {
                for (int col = 0; col < theGrid.GetLength(1); col++) {
                    TextBox tempBox = new TextBox();
                    tempBox.AutoSize = false;
                    tempBox.Size = txtBoxSize;
                    tempBox.BackColor = Constants.NICE_BACKGROUND;
                    tempBox.ForeColor = Constants.NICE_SECONDARY;
                    tempBox.Location = new Point(col * txtBoxSize.Width, row * txtBoxSize.Height + Constants.TEXT_DIMENSION);
                    tempBox.TextAlign = HorizontalAlignment.Center;
                    tempBox.TextChanged += Text_Changed;
                    tempBox.GotFocus += Box_GotFocus;
                    tempBox.Font = new Font(Constants.FONT, Constants.PRIMARY_FONT_SIZE);
                    theGrid[row, col] = tempBox;
                    this.Controls.Add(theGrid[row, col]);
                }
            }
            this.ActiveControl = theGrid[0, 0];
        }

        /// <summary>
        /// creates a new label with the random word exposed to the user
        /// </summary>
        public void InitializeCheats() {
            Label testBox = new Label();
            string randomWord = logic.GetRandomWord();
            testBox.Location = new Point(Constants.WORD_LENGTH * Constants.TEXT_DIMENSION, Constants.TEXT_DIMENSION * 2);
            testBox.AutoSize = false;
            testBox.Size = new Size(400, Constants.TEXT_DIMENSION / 2);
            testBox.BackColor = Constants.NICE_BACKGROUND;
            testBox.ForeColor = Constants.NICE_FOREGROUND;
            testBox.Font = new Font(Constants.FONT, Constants.PRIMARY_FONT_SIZE / 2);
            testBox.TextAlign = ContentAlignment.MiddleCenter;
            testBox.Text = "Cheats: ";
            testBox.Text += randomWord;
            this.Controls.Add(testBox);
        }

        // methods

        /// <summary>
        /// calls GetCurrentCharNum to get the current char num and subtracts 1
        /// </summary>
        /// <returns>the index of the previous textbox</returns>
        private int GetPreviousCharNum() {
            return GetCurrentCharNum() - 1;
        }

        /// <summary>
        /// iterates through the current guess row and sees if they are filled or not
        /// </summary>
        /// <returns>the index of the next textbox to be filled</returns>
        private int GetCurrentCharNum() {
            int intToReturn = 0;
            for (int i = 0; i < Constants.WORD_LENGTH; i++) {
                if (theGrid[currentGuessNum, i].Text.Length > 0) {
                    intToReturn++;
                }
            }
            return intToReturn;
        }

        /// <summary>
        /// turns all textboxes in the current guess row to readonly
        /// </summary>
        private void DisableCurrentGuessBoxes() {
            for (int i = 0; i < Constants.WORD_LENGTH; i++) {
                theGrid[currentGuessNum, i].ReadOnly = true;
            }
        }

        /// <summary>
        /// if game condition is 'w' then display game won message, if it isn't w display game lost message
        /// </summary>
        /// <param name="gameCondition">char which is either 'w', 'l', or ' '</param>
        private void GameOver(char gameCondition) {
            if (gameCondition != ' ') {
                string messageText;
                if (gameCondition == 'w') {
                    messageText = "You won!";
                }
                else {
                    messageText = "You lost!\nThe word was: " + logic.GetRandomWord();
                }
                MessageBox.Show(messageText);
            }
        }

        /// <summary>
        /// method that changes the colors of the textboxes based on a char array
        /// </summary>
        /// <param name="instructions">instruction array for how to change the gui colors</param>
        public void ChangeGuiColors(char[] instructions) {
            for (int i = 0; i < instructions.GetLength(0); i++) {
                switch (instructions[i]) {
                    case 'g':
                        theGrid[currentGuessNum, i].BackColor = Constants.NICE_GREEN;
                        break;
                    case 'y':
                        theGrid[currentGuessNum, i].BackColor = Constants.NICE_YELLOW;
                        break;
                    case 'r':
                        theGrid[currentGuessNum, i].BackColor = Constants.NICE_GRAY;
                        break;
                }
            }
        }

        /// <summary>
        /// used in the logic section to return the string entered by the user in the gui
        /// </summary>
        /// <returns>a string being the guess entered</returns>
        public string GetCurrentGuess() {
            string currentGuess = "";
            for (int col = 0; col < Constants.WORD_LENGTH; col++) {
                currentGuess += theGrid[currentGuessNum, col].Text[0];
            }
            return currentGuess;
        }

        // events

        /// <summary>
        /// this method does two main things, iterates the focused box for seemless typing
        /// and it also iterates the focused box backwards for seemless backspace deletion
        /// </summary>
        /// <param name="sender">will be a textbox</param>
        /// <param name="e">event args</param>
        private void Text_Changed(object sender, EventArgs e) {
            char firstChar = ' ';
            char secondChar = ' ';
            if (((TextBox)sender).Text.Length == 2) {
                firstChar = ((TextBox)sender).Text[0];
                secondChar = ((TextBox)sender).Text[1];
                ((TextBox)sender).Text = "" + firstChar;
                deletingLetters = false;
                // testing if char is last in word
                bool lastCharInWord = false;
                for (int col = 0; col < theGrid.GetLength(1); col++) {
                    if ((TextBox)sender == theGrid[currentGuessNum, Constants.WORD_LENGTH - 1]) {
                        lastCharInWord = true;
                    }
                }
                if (!lastCharInWord) {
                    int currentCharNum = GetCurrentCharNum();
                    theGrid[currentGuessNum, currentCharNum].Text = "" + secondChar;
                    theGrid[currentGuessNum, currentCharNum].DeselectAll();
                    theGrid[currentGuessNum, currentCharNum].SelectionStart = 1;
                    theGrid[currentGuessNum, currentCharNum].Focus();
                }
            }
            else {
                deletingLetters = true;
                // if sender isn't the first letter
                if (((TextBox)sender) != theGrid[currentGuessNum, 0]) {
                    int currentCharNum = GetPreviousCharNum();
                    theGrid[currentGuessNum, currentCharNum].DeselectAll();
                    theGrid[currentGuessNum, currentCharNum].SelectionStart = 1;
                    theGrid[currentGuessNum, currentCharNum].Focus();
                }
            }
        }

        /// <summary>
        /// if the box that should be focused isnt focused then focus it aka focus the box that makes
        /// sense for the next character to go
        /// </summary>
        /// <param name="sender">will be a textbox</param>
        /// <param name="e">event args</param>
        private void Box_GotFocus(object sender, EventArgs e) {
            // if box is not on current row
            bool onCurrentRow = false;
            for (int i = 0; i < Constants.WORD_LENGTH; i++) {
                if (((TextBox)sender) == theGrid[currentGuessNum, i]) {
                    onCurrentRow = true;
                }
            }
            if (GetCurrentCharNum() < Constants.WORD_LENGTH) {
                if (((TextBox)sender) != theGrid[currentGuessNum, GetCurrentCharNum()] && !deletingLetters && !onCurrentRow) {
                    theGrid[currentGuessNum, GetCurrentCharNum()].Focus();
                }
            }
        }

        /// <summary>
        /// if all textboxes in current guess are full then proceed with guess and test for win condition
        /// </summary>
        /// <param name="sender">will be a textbox</param>
        /// <param name="e">event args</param>
        private void Guess_Button_Clicked(object sender, EventArgs e) {
            bool allBoxesFull = true;
            for (int i = 0; i < Constants.WORD_LENGTH; i++) {
                if (theGrid[currentGuessNum, i].Text.Length <= 0) {
                    allBoxesFull = false;
                }
            }
            if (allBoxesFull) {
                string guessedString = GetCurrentGuess();
                if (logic.FileWordsAreLowerCase()) {
                    guessedString = logic.LowerCaseifyString(guessedString);
                }
                else {
                    guessedString = logic.CapitilizeString(guessedString);
                }
                if (logic.WordIsInList(guessedString)) {
                    ChangeGuiColors(logic.Guess(guessedString));
                    DisableCurrentGuessBoxes();
                    currentGuessNum++;
                }
                else {
                    MessageBox.Show("not in word list");
                    theGrid[currentGuessNum, Constants.WORD_LENGTH - 1].Focus();
                }
            }
            char gameCondition = logic.TestForGameOver();
            GameOver(gameCondition);
        }
    }
}