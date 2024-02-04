using System.Timers;
using System.Windows.Forms.VisualStyles;

namespace mgibbon2Minesweeper {
    public partial class MainWindow : Form {

        System.Timers.Timer timer = new System.Timers.Timer(1000);
        int amountOfSeconds;
        bool gameLost;
        bool gameWon;
        int winCount;
        int lossCount;
        string averageTime;
        bool firstPlay;
        int revealedBlocks;
        Button[,] theGrid;
        string[,] theCharGrid;
        PictureBox[,] theImageGrid;
        Label[,] theLabelGrid;
        Label timerLabel;

        public MainWindow() {
            InitializeComponent();
            ResetGameBoard();
        }

        /// <summary>
        /// cleans/wipes the game board and re-places mines
        /// </summary>
        public void ResetGameBoard() {
            amountOfSeconds = 0;
            gameLost = false;
            gameWon = false;
            InitializeMenuStrip();
            firstPlay = true;
            theGrid = new Button[Constants.BOARD_DIMENSION, Constants.BOARD_DIMENSION];
            for (int row = 0; row < theGrid.GetLength(0); row++) {
                for (int col = 0; col < theGrid.GetLength(1); col++) {
                    theGrid[row, col] = new Button();
                    theGrid[row, col].Size = new Size(Constants.BUTTON_DIMENSION, Constants.BUTTON_DIMENSION);
                    theGrid[row, col].Location = new Point(row * Constants.BUTTON_DIMENSION, col * Constants.BUTTON_DIMENSION);
                    theGrid[row, col].Click += Button_Click;
                    this.Controls.Add(theGrid[row, col]);
                }
            }
            theCharGrid = new string[Constants.BOARD_DIMENSION, Constants.BOARD_DIMENSION];
        }

        /// <summary>
        /// initializes the exit button, restart button, and the timer
        /// </summary>
        public void InitializeMenuStrip() {
            string[] scoreFile = File.ReadAllLines("score.txt");
            if (scoreFile.GetLength(0) > 0) {
                winCount = Int32.Parse(scoreFile[0]);
            }
            if (scoreFile.GetLength(0) > 1) {
                lossCount = Int32.Parse(scoreFile[1]);
            }
            if (scoreFile.GetLength(0) == 3) {
                averageTime = scoreFile[2];
            }
            Label infoLabel = new Label();
            infoLabel.AutoSize = true;
            double winRatio;
            if (lossCount == 0) {
                winRatio = 0;
            }
            else {
                winRatio = winCount / lossCount;
            }
            string avgTime;
            if (String.IsNullOrEmpty(averageTime)) {
                avgTime = "N/A";
            }
            else {
                avgTime = averageTime + "s";
            }
            InitializeOtherButtons(infoLabel, winRatio, avgTime);   
        }
        
        /// <summary>
        /// helper method for initializing the menu strip
        /// </summary>
        /// <param name="infoLabel">label for which displays the information</param>
        /// <param name="winRatio">double storing the win ratio</param>
        /// <param name="avgTime">string containing the average time</param>
        private void InitializeOtherButtons(Label infoLabel, double winRatio, string avgTime) {
            infoLabel.Text = "Win/Loss Ratio: " + winRatio + "\nAverage Time: " + avgTime;
            infoLabel.Location = new Point(0, Constants.BUTTON_DIMENSION * Constants.BOARD_DIMENSION);
            infoLabel.Font = new Font("Microsoft Sans Serif", 30);
            this.Controls.Add(infoLabel);
            Button restartButton = new Button();
            restartButton.Click += Restart_Click;
            restartButton.Size = new Size(Constants.BUTTON_DIMENSION * 2, Constants.BUTTON_DIMENSION);
            restartButton.Text = "RESTART";
            restartButton.Font = new Font("Microsoft Sans Serif", 10);
            restartButton.Location = new Point(Constants.BUTTON_DIMENSION * Constants.BOARD_DIMENSION, Constants.BUTTON_DIMENSION * Constants.BOARD_DIMENSION);
            this.Controls.Add(restartButton);
            Button exitButton = new Button();
            exitButton.Click += Restart_Click;
            exitButton.Size = new Size(Constants.BUTTON_DIMENSION * 2, Constants.BUTTON_DIMENSION);
            exitButton.Text = "EXIT";
            exitButton.Font = new Font("Microsoft Sans Serif", 10);
            exitButton.Location = new Point(Constants.BUTTON_DIMENSION * Constants.BOARD_DIMENSION, Constants.BUTTON_DIMENSION * Constants.BOARD_DIMENSION + Constants.BUTTON_DIMENSION);
            this.Controls.Add(exitButton);
            timerLabel = new Label();
            timerLabel.Font = new Font("Microsoft Sans Serif", 20);
            timerLabel.Text = "Timer\n0";
            timerLabel.AutoSize = true;
            timerLabel.Location = new Point(Constants.BUTTON_DIMENSION * Constants.BOARD_DIMENSION, Constants.BUTTON_DIMENSION * Constants.BOARD_DIMENSION - (2 * Constants.BUTTON_DIMENSION));
            this.Controls.Add(timerLabel);
        }

        /// <summary>
        /// iterates through the amount of mines to be placed and finds a random location for them
        /// </summary>
        /// <param name="clickedLocation">the first click location</param>
        public void GenerateMines(Point clickedLocation) {
            Random random = new Random();
            for (int i = 0; i < Constants.AMOUNT_OF_MINES; i++) {
                int randomRow = random.Next(0, Constants.BOARD_DIMENSION);
                int randomCol = random.Next(0, Constants.BOARD_DIMENSION);
                Point randomPoint = new Point(randomRow, randomCol);
                if (ValidMineLocation(clickedLocation, randomPoint)) {
                    theCharGrid[randomRow, randomCol] = "m";
                }
                else {
                    i--;
                }
            }
        }

        /// <summary>
        /// checks if the mine location is valid compared to the clicked point
        /// </summary>
        /// <param name="clickedLocation">the first click location</param>
        /// <param name="randomPoint">the point where the mine wants to be</param>
        /// <returns>true or false if the spot is valid</returns>
        private bool ValidMineLocation(Point clickedLocation, Point randomPoint) {
            bool boolToReturn = true;
            if (theCharGrid[randomPoint.X, randomPoint.Y] != "m" && clickedLocation != randomPoint) {
                Point[] indicesAroundPoint = FindIndicesAroundPoint(clickedLocation);
                for (int i = 0; i < indicesAroundPoint.GetLength(0); i++) {
                    if (randomPoint == indicesAroundPoint[i]) {
                        boolToReturn = false;
                    }
                }
            }
            else {
                boolToReturn = false;
            }
            return boolToReturn;
        }

        /// <summary>
        /// will call different methods to return a list of points to check that are circled around the specific point
        /// </summary>
        /// <param name="location">the location of the checked point</param>
        /// <returns>the array of points to check</returns>
        private Point[] FindIndicesAroundPoint(Point location) {
            Point[] arrayToReturn;
            if (location.X == 0) {
                arrayToReturn = FindLeftIndices(location);
            }
            else if (location.X == Constants.BOARD_DIMENSION - 1) {
                arrayToReturn = FindRightIndices(location);
            }
            else if (location.Y == 0) {
                arrayToReturn = FindTopIndices(location);
            }
            else if (location.Y == Constants.BOARD_DIMENSION - 1) {
                arrayToReturn = FindBottomIndices(location);
            }
            else {
                int amountOfBorders = 8;
                arrayToReturn = new Point[amountOfBorders];
                arrayToReturn[0] = new Point(location.X + 1, location.Y - 1);
                arrayToReturn[1] = new Point(location.X - 1, location.Y);
                arrayToReturn[2] = new Point(location.X - 1, location.Y + 1);
                arrayToReturn[3] = new Point(location.X, location.Y - 1);
                arrayToReturn[4] = new Point(location.X, location.Y + 1);
                arrayToReturn[5] = new Point(location.X - 1, location.Y - 1);
                arrayToReturn[6] = new Point(location.X + 1, location.Y);
                arrayToReturn[7] = new Point(location.X + 1, location.Y + 1);

            }
            return arrayToReturn;
        }

        /// <summary>
        /// used when the location is on the left column to make sure not to iterate past the bounds
        /// </summary>
        /// <param name="location">the location of the checked point</param>
        /// <returns>the array of points to check</returns>
        private Point[] FindLeftIndices(Point location) {
            Point[] arrayToReturn;
            if (location.Y == 0) {
                int amountOfBorders = 3;
                arrayToReturn = new Point[amountOfBorders];
                arrayToReturn[0] = new Point(0, 1);
                arrayToReturn[1] = new Point(1, 0);
                arrayToReturn[2] = new Point(1, 1);
            }
            else if (location.Y == Constants.BOARD_DIMENSION - 1) {
                int amountOfBorders = 3;
                arrayToReturn = new Point[amountOfBorders];
                arrayToReturn[0] = new Point(0, Constants.BOARD_DIMENSION - 2);
                arrayToReturn[1] = new Point(1, Constants.BOARD_DIMENSION - 2);
                arrayToReturn[2] = new Point(1, Constants.BOARD_DIMENSION - 1);
            }
            else {
                int amountOfBorders = 5;
                arrayToReturn = new Point[amountOfBorders];
                arrayToReturn[0] = new Point(location.X, location.Y - 1);
                arrayToReturn[1] = new Point(location.X + 1, location.Y - 1);
                arrayToReturn[2] = new Point(location.X + 1, location.Y);
                arrayToReturn[3] = new Point(location.X + 1, location.Y + 1);
                arrayToReturn[4] = new Point(location.X, location.Y + 1);
            }
            return arrayToReturn;
        }

        /// <summary>
        /// used when the location is on the right column to make sure not to iterate past the bounds
        /// </summary>
        /// <param name="location">the location of the checked point</param>
        /// <returns>the array of points to check</returns>
        private Point[] FindRightIndices(Point location) {
            Point[] arrayToReturn;
            if (location.Y == 0) {
                int amountOfBorders = 3;
                arrayToReturn = new Point[amountOfBorders];
                arrayToReturn[0] = new Point(Constants.BOARD_DIMENSION - 1, 0);
                arrayToReturn[1] = new Point(Constants.BOARD_DIMENSION - 2, 1);
                arrayToReturn[2] = new Point(Constants.BOARD_DIMENSION - 1, 1);
            }
            else if (location.Y == Constants.BOARD_DIMENSION - 1) {
                int amountOfBorders = 3;
                arrayToReturn = new Point[amountOfBorders];
                arrayToReturn[0] = new Point(Constants.BOARD_DIMENSION - 2, Constants.BOARD_DIMENSION - 2);
                arrayToReturn[1] = new Point(Constants.BOARD_DIMENSION - 2, Constants.BOARD_DIMENSION - 1);
                arrayToReturn[2] = new Point(Constants.BOARD_DIMENSION - 1, Constants.BOARD_DIMENSION - 2);
            }
            else {
                int amountOfBorders = 5;
                arrayToReturn = new Point[amountOfBorders];
                arrayToReturn[0] = new Point(location.X, location.Y - 1);
                arrayToReturn[1] = new Point(location.X - 1, location.Y - 1);
                arrayToReturn[2] = new Point(location.X - 1, location.Y);
                arrayToReturn[3] = new Point(location.X - 1, location.Y + 1);
                arrayToReturn[4] = new Point(location.X, location.Y + 1);
            }
            return arrayToReturn;
        }

        /// <summary>
        /// used when the location is on the top row to make sure not to iterate past the bounds
        /// </summary>
        /// <param name="location">the location of the checked point</param>
        /// <returns>the array of points to check</returns>
        private Point[] FindTopIndices(Point location) {
            int amountOfBorders = 5;
            Point[] arrayToReturn = new Point[amountOfBorders];

            arrayToReturn[0] = new Point(location.X - 1, location.Y);
            arrayToReturn[1] = new Point(location.X - 1, location.Y + 1);
            arrayToReturn[2] = new Point(location.X, location.Y + 1);
            arrayToReturn[3] = new Point(location.X + 1, location.Y + 1);
            arrayToReturn[4] = new Point(location.X + 1, location.Y);

            return arrayToReturn;
        }

        /// <summary>
        /// used when the location is on the bottom row to make sure not to iterate past the bounds
        /// </summary>
        /// <param name="location">the location of the checked point</param>
        /// <returns>the array of points to check</returns>
        private Point[] FindBottomIndices(Point location) {
            int amountOfBorders = 5;
            Point[] arrayToReturn = new Point[amountOfBorders];

            arrayToReturn[0] = new Point(location.X - 1, location.Y);
            arrayToReturn[1] = new Point(location.X - 1, location.Y - 1);
            arrayToReturn[2] = new Point(location.X, location.Y - 1);
            arrayToReturn[3] = new Point(location.X + 1, location.Y - 1);
            arrayToReturn[4] = new Point(location.X + 1, location.Y);

            return arrayToReturn;
        }

        /// <summary>
        /// will call different methods to return a list of points to check that are circled around the specific point without the diagonal corners
        /// </summary>
        /// <param name="location">the location of the checked point</param>
        /// <returns>the array of points to check</returns>
        private Point[] FindIndicesAroundPointNoCorners(Point location) {
            Point[] arrayToReturn;
            if (location.X == 0) {
                arrayToReturn = FindLeftIndicesNoCorners(location);
            }
            else if (location.X == Constants.BOARD_DIMENSION - 1) {
                arrayToReturn = FindRightIndicesNoCorners(location);
            }
            else if (location.Y == 0) {
                arrayToReturn = FindTopIndicesNoCorners(location);
            }
            else if (location.Y == Constants.BOARD_DIMENSION - 1) {
                arrayToReturn = FindBottomIndicesNoCorners(location);
            }
            else {
                int amountOfBorders = 4;
                arrayToReturn = new Point[amountOfBorders];
                arrayToReturn[0] = new Point(location.X - 1, location.Y);
                arrayToReturn[1] = new Point(location.X, location.Y - 1);
                arrayToReturn[2] = new Point(location.X, location.Y + 1);
                arrayToReturn[3] = new Point(location.X + 1, location.Y);

            }
            return arrayToReturn;
        }

        /// <summary>
        /// used when the location is on the left column to make sure not to iterate past the bounds without the diagonal corners
        /// </summary>
        /// <param name="location">the location of the checked point</param>
        /// <returns>the array of points to check</returns>
        private Point[] FindLeftIndicesNoCorners(Point location) {
            Point[] arrayToReturn;
            if (location.Y == 0) {
                int amountOfBorders = 2;
                arrayToReturn = new Point[amountOfBorders];
                arrayToReturn[0] = new Point(0, 1);
                arrayToReturn[1] = new Point(1, 0);
            }
            else if (location.Y == Constants.BOARD_DIMENSION - 1) {
                int amountOfBorders = 2;
                arrayToReturn = new Point[amountOfBorders];
                arrayToReturn[0] = new Point(0, Constants.BOARD_DIMENSION - 2);
                arrayToReturn[1] = new Point(1, Constants.BOARD_DIMENSION - 1);
            }
            else {
                int amountOfBorders = 3;
                arrayToReturn = new Point[amountOfBorders];
                arrayToReturn[0] = new Point(location.X, location.Y - 1);
                arrayToReturn[1] = new Point(location.X + 1, location.Y);
                arrayToReturn[2] = new Point(location.X, location.Y + 1);
            }
            return arrayToReturn;
        }

        /// <summary>
        /// used when the location is on the right column to make sure not to iterate past the bounds without the diagonal corners
        /// </summary>
        /// <param name="location">the location of the checked point</param>
        /// <returns>the array of points to check</returns>
        private Point[] FindRightIndicesNoCorners(Point location) {
            Point[] arrayToReturn;
            if (location.Y == 0) {
                int amountOfBorders = 2;
                arrayToReturn = new Point[amountOfBorders];
                arrayToReturn[0] = new Point(Constants.BOARD_DIMENSION - 1, 0);
                arrayToReturn[1] = new Point(Constants.BOARD_DIMENSION - 1, 1);
            }
            else if (location.Y == Constants.BOARD_DIMENSION - 1) {
                int amountOfBorders = 2;
                arrayToReturn = new Point[amountOfBorders];
                arrayToReturn[0] = new Point(Constants.BOARD_DIMENSION - 2, Constants.BOARD_DIMENSION - 1);
                arrayToReturn[1] = new Point(Constants.BOARD_DIMENSION - 1, Constants.BOARD_DIMENSION - 2);
            }
            else {
                int amountOfBorders = 3;
                arrayToReturn = new Point[amountOfBorders];
                arrayToReturn[0] = new Point(location.X, location.Y - 1);
                arrayToReturn[1] = new Point(location.X - 1, location.Y);
                arrayToReturn[2] = new Point(location.X, location.Y + 1);
            }
            return arrayToReturn;
        }

        /// <summary>
        /// used when the location is on the top row to make sure not to iterate past the bounds without the diagonal corners
        /// </summary>
        /// <param name="location">the location of the checked point</param>
        /// <returns>the array of points to check</returns>
        private Point[] FindTopIndicesNoCorners(Point location) {
            int amountOfBorders = 3;
            Point[] arrayToReturn = new Point[amountOfBorders];

            arrayToReturn[0] = new Point(location.X - 1, location.Y);
            arrayToReturn[1] = new Point(location.X, location.Y + 1);
            arrayToReturn[2] = new Point(location.X + 1, location.Y);

            return arrayToReturn;
        }

        /// <summary>
        /// used when the location is on the bottom row to make sure not to iterate past the bounds without the diagonal corners
        /// </summary>
        /// <param name="location">the location of the checked point</param>
        /// <returns>the array of points to check</returns>
        private Point[] FindBottomIndicesNoCorners(Point location) {
            int amountOfBorders = 3;
            Point[] arrayToReturn = new Point[amountOfBorders];

            arrayToReturn[0] = new Point(location.X - 1, location.Y);
            arrayToReturn[1] = new Point(location.X, location.Y - 1);
            arrayToReturn[2] = new Point(location.X + 1, location.Y);

            return arrayToReturn;
        }

        /// <summary>
        /// after the mines are placed, this method generates the numbers that correlate
        /// </summary>
        public void GenerateNumbers() {
            int[,] theNumberGrid = new int[Constants.BOARD_DIMENSION, Constants.BOARD_DIMENSION];
            // 2 for loops to iterate through the inner elements where the index will never go out of bounds
            for (int row = 0; row < theNumberGrid.GetLength(0); row++) {
                for (int col = 0; col < theNumberGrid.GetLength(1); col++) {
                    if (theCharGrid[row, col] != "m") {
                        theNumberGrid[row, col] = 0;
                        Point[] pointsToCheck = FindIndicesAroundPoint(new Point(row, col));
                        for (int i = 0; i < pointsToCheck.GetLength(0); i++) {
                            if (theCharGrid[pointsToCheck[i].X, pointsToCheck[i].Y] == "m") {
                                theNumberGrid[row, col]++;
                            }
                        }
                        theCharGrid[row, col] = Convert.ToString(theNumberGrid[row, col]);
                    }
                }
            }
        }

        /// <summary>
        /// *UNUSED* for using the images instead of the characters 
        /// </summary>
        public void AddCharsToImages() {
            theImageGrid = new PictureBox[Constants.BOARD_DIMENSION, Constants.BOARD_DIMENSION];
            for (int row = 0; row < theCharGrid.GetLength(0); row++) {
                for (int col = 0; col < theCharGrid.GetLength(1); col++) {
                    Image imageToDisplay;
                    switch (theCharGrid[row, col]) {
                        case "m":
                            imageToDisplay = Image.FromFile("mine.png");
                            break;
                        case "1":
                            imageToDisplay = Image.FromFile("one.png");
                            break;
                        case "2":
                            imageToDisplay = Image.FromFile("two.png");
                            break;
                        case "3":
                            imageToDisplay = Image.FromFile("three.png");
                            break;
                        case "4":
                            imageToDisplay = Image.FromFile("four.png");
                            break;
                        case "5":
                            imageToDisplay = Image.FromFile("five.png");
                            break;
                        case "6":
                            imageToDisplay = Image.FromFile("six.png");
                            break;
                        case "7":
                            imageToDisplay = Image.FromFile("seven.png");
                            break;
                        case "8":
                            imageToDisplay = Image.FromFile("eight.png");
                            break;
                        default:
                            imageToDisplay = Image.FromFile("blank.png");
                            break;
                    }
                    theImageGrid[row, col] = new PictureBox();
                    theImageGrid[row, col].ClientSize = new Size(Constants.BUTTON_DIMENSION, Constants.BUTTON_DIMENSION);
                    theImageGrid[row, col].Image = imageToDisplay;
                    theImageGrid[row, col].Location = new Point(row * Constants.BUTTON_DIMENSION, col * Constants.BUTTON_DIMENSION);
                    this.Controls.Add(theImageGrid[row, col]);
                }
            }
        }

        /// <summary>
        /// fills the label grid with labels containing each char
        /// </summary>
        public void AddCharsToLabels() {
            theLabelGrid = new Label[Constants.BOARD_DIMENSION, Constants.BOARD_DIMENSION];
            for (int row = 0; row < theCharGrid.GetLength(0); row++) {
                for (int col = 0; col < theCharGrid.GetLength(1); col++) {
                    theLabelGrid[row, col] = new Label();
                    theLabelGrid[row, col].AutoSize = false;
                    theLabelGrid[row, col].Size = new Size(Constants.BUTTON_DIMENSION, Constants.BUTTON_DIMENSION);
                    theLabelGrid[row, col].Text += theCharGrid[row, col];
                    theLabelGrid[row, col].Location = new Point(row * Constants.BUTTON_DIMENSION, col * Constants.BUTTON_DIMENSION);
                    theLabelGrid[row, col].Font = new Font("Microsoft Sans Serif", 30);
                    this.Controls.Add(theLabelGrid[row, col]);
                }
            }
        }

        /// <summary>
        /// compares the amount of revelead blocks and the amount of total blocks - amount of mines
        /// </summary>
        /// <returns>true if game is won</returns>
        private bool CheckWinCondition() {
            bool boolToReturn = false;
            if (revealedBlocks == (Constants.BOARD_DIMENSION * Constants.BOARD_DIMENSION) - Constants.AMOUNT_OF_MINES) {
                boolToReturn = true;
            }
            return boolToReturn;
        }

        /// <summary>
        /// stops the timer and increments loss count and calls the method to update the file
        /// </summary>
        private void UpdateLoss() {
            timer.Stop();
            lossCount++;
            RevealAllBlocks();
            gameLost = true;
            UpdateFile();
        }

        /// <summary>
        /// iterates through the button array hiding all
        /// </summary>
        private void RevealAllBlocks() {
            for (int i = 0; i < theGrid.GetLength(0); i++) {
                for (int j = 0; j < theGrid.GetLength(1); j++) {
                    theGrid[i, j].Hide();
                }
            }
        }

        /// <summary>
        /// stops the timer and increments win count and then calls the method to update the file
        /// </summary>
        private void UpdateWin() {
            timer.Stop();
            winCount++;
            MessageBox.Show("You've won!");
            gameWon = true;
            UpdateFile();
        }

        /// <summary>
        /// updates the score file with the new values for win count, loss count, and average time
        /// </summary>
        private void UpdateFile() {
            StreamWriter fileWriter = new StreamWriter("score.txt");
            fileWriter.WriteLine(winCount);
            fileWriter.WriteLine(lossCount);
            int thisTime = amountOfSeconds;
            int avgTime = 0;
            if (gameWon && !string.IsNullOrEmpty(averageTime)) {
                avgTime = (Int32.Parse(averageTime) * (winCount + lossCount - 1) + thisTime) / (winCount + lossCount);
            }
            gameWon = false;
            if (avgTime > 0) {
                fileWriter.WriteLine(avgTime);
            }
            fileWriter.Close();
        }

        /// <summary>
        /// when the button gets clicked, hide it, and then if it's the first click then generate the rest of the board, making sure
        /// the first click is an empty space
        /// </summary>
        /// <param name="sender">standard event args</param>
        /// <param name="e">standard event args</param>
        private void Button_Click(object sender, EventArgs e) {
            ((Button)sender).Hide();
            this.ActiveControl = null;
            int clickedRow = 0;
            int clickedCol = 0;
            for (int row = 0; row < theGrid.GetLength(0); row++) {
                for (int col = 0; col < theGrid.GetLength(1); col++) {
                    if (((Button)sender) == theGrid[row, col]) {
                        clickedRow = row;
                        clickedCol = col;
                    }
                }
            }
            Point clickedPoint = new Point(clickedRow, clickedCol);
            if (firstPlay) {
                timer.Elapsed += Update_Timer;
                timer.Start();

                GenerateMines(clickedPoint);
                GenerateNumbers();
                // AddCharsToImages();
                AddCharsToLabels();
                firstPlay = false;
            }
            Button_Click_Part_2(clickedRow, clickedCol, clickedPoint);   
        }

        /// <summary>
        /// helper method for the Button_Click event
        /// </summary>
        /// <param name="clickedRow">row of clicked point</param>
        /// <param name="clickedCol">col of clicked point</param>
        /// <param name="clickedPoint">location of clicked point</param>
        private void Button_Click_Part_2(int clickedRow, int clickedCol, Point clickedPoint) {
            if (theCharGrid[clickedRow, clickedCol] == "0") {
                Point[] pointsToHide = FindIndicesAroundPointNoCorners(clickedPoint);
                for (int i = 0; i < pointsToHide.GetLength(0); i++) {
                    if (theCharGrid[pointsToHide[i].X, pointsToHide[i].Y] == "0") {
                        theGrid[pointsToHide[i].X, pointsToHide[i].Y].PerformClick();
                        Point[] morePointsToHide = FindIndicesAroundPointNoCorners(pointsToHide[i]);
                        for (int j = 0; j < morePointsToHide.GetLength(0); j++) {
                            if (theCharGrid[morePointsToHide[j].X, morePointsToHide[j].Y] != "m") {
                                theGrid[morePointsToHide[j].X, morePointsToHide[j].Y].PerformClick();
                            }
                        }
                    }
                }
            }
            if (theCharGrid[clickedRow, clickedCol] != "m") {
                revealedBlocks++;
                if (CheckWinCondition()) {
                    UpdateWin();
                }
            }
            else {
                MessageBox.Show("You've lost!");
                UpdateLoss();
            }
        }

        /// <summary>
        /// event args for the timer which happens every second, updates the label displaying the time
        /// </summary>
        /// <param name="sender">standard event args</param>
        /// <param name="e">standard event args</param>
        private void Update_Timer(object sender, ElapsedEventArgs e) {
            amountOfSeconds++;
            // weird cross thread error
            // timerLabel.Text = "Timer\n" + amountOfSeconds;
            timerLabel.Invoke((MethodInvoker)(() => timerLabel.Text = "Timer\n" + amountOfSeconds));
        }

        /// <summary>
        /// event for when the restart button gets clicked, calls the method to update losses and resets the game board
        /// </summary>
        /// <param name="sender">standard event args</param>
        /// <param name="e">standard event args</param>
        private void Restart_Click(object sender, EventArgs e) {
            // timer.Stop();
            if (!gameLost) {
                MessageBox.Show("You've given up!");
            }
            gameLost = false;
            UpdateLoss();
            ResetGameBoard();
        }

        /// <summary>
        /// event for when the exit button gets clicked, calls the method to update losses and then closes the application
        /// </summary>
        /// <param name="sender">standard event args</param>
        /// <param name="e">standard event args</param>
        private void Exit_Click(object sender, EventArgs e) {
            // timer.Stop();
            UpdateLoss();
            this.Hide();
        }
    }
}