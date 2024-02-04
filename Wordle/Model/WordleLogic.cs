using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegallyNotWordle.Model {
    internal class WordleLogic {

        string[] allWords;
        string randomWord;
        bool gameWon = false;
        int guessCounter = 0;

        public WordleLogic() {
            allWords = new string[GetWordCount(Constants.FILE_NAME)];
            FillAllWords(Constants.FILE_NAME);
            SetRandomWord();
        }

        /// <summary>
        /// gets the line count of the file
        /// </summary>
        /// <param name="fileName">the literal name of the file with the words in it</param>
        /// <returns>the integer value of the line count in the file</returns>
        public int GetWordCount(string fileName) {
            int lineCount = File.ReadLines(fileName).Count();
            return lineCount;
        }

        /// <summary>
        /// populates the array all words with all of the lines in the file
        /// </summary>
        /// <param name="fileName">the literal name of the file with the words in it</param>
        public void FillAllWords(string fileName) {
            allWords = File.ReadAllLines(fileName);
        }

        /// <summary>
        /// sets the value of random word to a new random word
        /// </summary>
        private void SetRandomWord() {
            Random rng = new Random();
            randomWord = allWords[rng.Next(0, allWords.GetLength(0))];
        }

        /// <summary>
        /// getter for the gui to access the random word instance variable
        /// </summary>
        /// <returns>value of random word</returns>
        public string GetRandomWord() {
            return randomWord;
        }

        /// <summary>
        /// uses other methods to find what character instruction correlates to each character
        /// in the string entered
        /// </summary>
        /// <param name="guess">string entered by user</param>
        /// <returns>char array with instructions for the gui to change textbox colors</returns>
        public char[] Guess(string guess) {
            guessCounter++;
            int greenCount = 0;
            char[] guiColorInstructions = new char[Constants.WORD_LENGTH];
            for (int i = 0; i < guess.Length; i++) {
                if (CharRightSpot(guess[i], i)) {
                    guiColorInstructions[i] = 'g';
                    greenCount++;
                }
                else if (CharIsIncluded(guess[i])) {
                    guiColorInstructions[i] = 'y';
                }
                else {
                    // r means gray because g was taken and g'r'ay
                    guiColorInstructions[i] = 'r';
                }
            }
            if (greenCount == Constants.WORD_LENGTH) {
                gameWon = true;
            }
            return guiColorInstructions;
        }

        /// <summary>
        /// iterates through the word and returns true if char is in word at the same index
        /// </summary>
        /// <param name="chara">character to test</param>
        /// <param name="charIndex">index of the character</param>
        /// <returns>true if char is in the random word at the same index</returns>
        private bool CharRightSpot(char chara, int charIndex) {
            bool boolToReturn = false;
            if (randomWord[charIndex] == chara) {
                boolToReturn = true;
            }
            return boolToReturn;
        }

        /// <summary>
        /// iterates through the word and returns true if char is in word
        /// </summary>
        /// <param name="chara">character to test</param>
        /// <returns>true if char is in the random word</returns>
        private bool CharIsIncluded(char chara) {
            bool boolToReturn = false;
            for (int i = 0; i < randomWord.Length; i++) {
                if (randomWord[i] == Char.ToLower(chara) || randomWord[i] == Char.ToUpper(chara)) {
                    boolToReturn = true;
                }
            }
            return boolToReturn;
        }

        /// <summary>
        /// was going to use this method to only highlight a single character yellow, if a word with one e is selected
        /// and the user enters "eagle" then only one e in eagle should be highlighted yellow
        /// </summary>
        /// <returns>the amount each character is repeated in a word</returns>
        private int[] RepeatedLettersInWord() {
            int[] arrayToReturn = new int[Constants.WORD_LENGTH];
            for (int i = 0; i < arrayToReturn.GetLength(0); i++) {
                arrayToReturn[i] = 0;
                for (int k = 0; k < arrayToReturn.GetLength(0); k++) {
                    if (randomWord[k] == randomWord[i]) {
                        arrayToReturn[i]++;
                    }
                }
            }
            return arrayToReturn;
        }

        /// <summary>
        /// uses .Contains to test if the string is in the string array for all words
        /// </summary>
        /// <param name="word">string to test if in array</param>
        /// <returns>true if word is in array and false if otherwise</returns>
        public bool WordIsInList(string word) {
            return allWords.Contains(word);
        }

        /// <summary>
        /// sees if the words in the file, based off the first letter is upper case or lower case
        /// </summary>
        /// <returns>true if the words are lowercase and false if they are upper case</returns>
        public bool FileWordsAreLowerCase() {
            bool boolToReturn = false;
            string tempString = allWords[0];
            if (Char.IsLower(tempString[0])) {
                boolToReturn = true;
            }
            return boolToReturn;
        }

        /// <summary>
        /// returns word but all uppercase
        /// </summary>
        /// <param name="word">any string</param>
        /// <returns>a purely uppercase string</returns>
        public string CapitilizeString(string word) {
            string stringToReturn = "";
            for (int i = 0; i < word.Length; i++) {
                stringToReturn += Char.ToUpper(word[i]);
            }
            return stringToReturn;
        }

        /// <summary>
        /// returns word but all lowercase
        /// </summary>
        /// <param name="word">any string</param>
        /// <returns>a purely lowercase string</returns>
        public string LowerCaseifyString(string word) {
            string stringToReturn = "";
            for (int i = 0; i < word.Length; i++) {
                stringToReturn += Char.ToLower(word[i]);
            }
            return stringToReturn;
        }

        /// <summary>
        /// chooses a char to return for the current game condition
        /// </summary>
        /// <returns>' ' if game is still going, 'w' if game won and 'l' if game is lost</returns>
        public char TestForGameOver() {
            char charToReturn = ' ';
            if (gameWon) {
                charToReturn = 'w';
            }
            else if (guessCounter == Constants.GUESS_LENGTH) {
                charToReturn = 'l';
            }
            return charToReturn;
        }
    }
}
