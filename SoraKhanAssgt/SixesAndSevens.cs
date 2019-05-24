using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
/*
    Sora Khan - 3197198
    INFT2012 - Applications Programming Assignment
    Start   : 10 May 2019 - Friday
    Due     : 26 May 2019 - Sunday

    This program...
 */
namespace SoraKhanAssgt
{
    public partial class frmSixesAndSevens : Form
    {
        private bool player1Turn,  playerVsComp, gameEnded, changed, someoneWon; //player1Won, player2Won,
        private int runScoreP1, runScoreP2;
        private int cmlScoreP1, cmlScoreP2;
        private int overallP1, overallP2, overallP3;
        private int leftDiceNum, rightDiceNum, total, goal;
        private String player1, player2, mode;

        public frmSixesAndSevens()
        {
            InitializeComponent();
            resetGameScore();  // initialise -in game score
            overallP1 = 0; overallP2 = 0; overallP3 = 0; // nobody has won a game yet
            player1 = "PLAYER 1"; 
            player2 = "COMPUTER";
            playerVsComp = true;
            mode = ""; // mode is not set initially. This is only activated once games starts and the mode is then set to keep track of any changes in any continuous gameplay
            changed = false; // the first mode ever is the original mode you change back to always - initial mode picked has not been changed
        }

        // ------------------------------------------------------------------------------ //
        // ------------------------------ G A M E P L A Y ------------------------------- //
        // ------------------------------------------------------------------------------ //

        private void playGame()
        {
            determineStarter();
            pnlGame.Show();
            hide("playButton");
            gameEnded = false; // game has not ended yet
            btnQuit.Width = pnlGameDetails.Width * 90 / 100; // changes the QUIT BUTTON's size to fill up the group box as PLAY BUTTON is removed
            btnQuit.Left = 10;
            checkGame(); // checks to see whether it is the computer's turn .. if the game starts with the computer in turn, and it is the Player vs. Computer mode!
        }

        private void quitGame()
        {
            resetGameScore();
            gameEnded = true; // to prevent computer from continuously rolling if quitting game
            hide("game");

            // if any player has won at least one game, a game has been played, so don't show the starting menu
            if (overallP1 != 0 || overallP2 != 0 || overallP3 != 0) show("playAgain");
            else show("main");

            btnQuit.Width = btnPlay.Width; // returns size and location of QUIT BUTTON back to original
            btnQuit.Left = btnPlay.Width + 25;
        }

        private void setGameMode()
        {
            checkPlayerMode();

            // if player is against another user and not computer
            if (!playerVsComp) player2 = "PLAYER 2";
            else player2 = "COMPUTER";

            updateInterface();

            // show the Group Box to set Goal Score if a mode has been selected only
            if (rdbtnPlayerComp.Checked || rdbtn2Player.Checked) pnlSetGoal.Show();
        }

        private void passTurn()
        {
            changeStatus("pass");
            slowDown(1000);
            changePlayerTurn();
            checkGame(); // whichever player's turn to either roll or pass
        }


        // This method changes a player's turn, to the opposite player's turn. 
        // Resets the running score of the player before turn is passed on to the other player
        private void changePlayerTurn()
        {

            if (player1Turn)
            {
                cmlScoreP1 += runScoreP1; // pass current running score to cumpulative score
                runScoreP1 = 0; // reset running score
            }
            else
            {
                cmlScoreP2 += runScoreP2;
                runScoreP2 = 0;
            }

            player1Turn = !player1Turn; // sets whether player 1 turn or not depending on the turns . . . changes to other player turn
            updateInterface();
            changeStatus("playerTurn");
        }

        // ------------------- ROLL DICE ------------------- //
        private void rollDice()
        {
            Random rand = new Random();
            leftDiceNum = rand.Next(1, 7);
            rightDiceNum = rand.Next(1, 7);
            total = leftDiceNum + rightDiceNum;
            changeStatus("rollingDice");

            passRollButtons(false); // stops user from trying to click too  many times

            slowDown(2000);
            createDice("left", leftDiceNum);
            createDice("right", rightDiceNum);
            updateInterface();

            if (!hasRolledSix() && !hasRolledSeven())
            {
                addToRunningScore(total); // if no 6 has been rolled in either or both of the dice, then only we set the running score
                hasReachedGoal(); // every time player rolls, checks whether goal has been reached
            }
            else
            {   // otherwise total will be 0
                if (hasRolledSeven()) // sets the cumulative score to 0 depending on whose turn it is.
                {
                    resetCumulativeScore();
                    changeStatus("rolledSeven");

                }
                else if (hasRolledSix())
                    changeStatus("rolledSix");

                slowDown(1500); // slow down to display message status
                changePlayerTurn();
            }
            checkGame(); // check whether someone has won
            changeStatus("playerTurn"); // if game not ended, this will show which player's turn it is
        }

        // ------------------------------------------------------------------------------ //
        // ------------------------------ C O M P U T E R ------------------------------- //
        // ------------------------------------------------------------------------------ //

        private void computer()
        {
            String[] commands = { "roll", "pass" };

            while (player1Turn == false && !gameEnded)
            {
                slowDown(1200);
                rollDice();
                checkGame(); // in case the game is quit while computer rolling dice
                
                // but then check score and what not to see if needed to roll again
            }
        }

        // ------------------------------------------------------------------------------ //
        // -------------------------------- R E S E T S --------------------------------- //
        // ------------------------------------------------------------------------------ //

        private void resetGameScore()
        {
            player1Turn = false;  gameEnded = false; someoneWon = false;// player1Won = false; player2Won = false;
            runScoreP1 = 0; runScoreP2 = 0; cmlScoreP1 = 0; cmlScoreP2 = 0;
            leftDiceNum = 0; rightDiceNum = 0; total = 0; goal = 0;
            updateInterface();
        }

        // resets all players overall score on the scoreboard
        private void resetOverallScore()
        {
            overallP1 = 0;
            overallP2 = 0;
            overallP3 = 0;
        }

        ////////////
        private void resetDice()
        {
            // so that when the numbers on display and of the dice are 0, does not show any dots
            picbxLeftDice.CreateGraphics().FillRectangle(Brushes.SteelBlue, 0, 0, 78, 78);
            picbxRightDice.CreateGraphics().FillRectangle(Brushes.SteelBlue, 0, 0, 78, 78);
        }

        private void resetCumulativeScore()
        {
            if (player1Turn) cmlScoreP1 = 0;
            else cmlScoreP2 = 0;
        }

        private void resetRunningScore()
        {
            if (player1Turn) runScoreP1 = 0;
            else runScoreP2 = 0;
        }
        
        // ------------------------------------------------------------------------------ //
        // -------------------------------- C H E C K S --------------------------------- //
        // ------------------------------------------------------------------------------ //

        // ------------------- CHECK IF REACHED GOAL ------------------- //

        // This method allows for recognising when the active player’s cumulative score plus running score has
        // reached a predefined total, which could be >= to the goal score
        private void hasReachedGoal()
        {
            if (cmlScoreP1 + runScoreP1 >= goal)
            {
                someoneWon = true;
                overallP1 += 1;
                lblCongrats.Text = "Congratulations, " + player1;
            }
            else if (cmlScoreP2 + runScoreP2 >= goal)
            {
                someoneWon = true;
                // scoreboard score setting depending on whether game status has been changed in between, to cater for 3rd player
                if (changed) overallP3 += 1;
                else overallP2 += 1;
                lblCongrats.Text = "Congratulations, " + player2;
            }
            checkGame(); // checks if game is won
        }

        // This method gets called throughout various points in the game to enable buttons, 
        // or check the game status (if a game has been won or not and if it is computer's turn
        private void checkGame()
        {
            if (someoneWon)
            {
                playAgainMenu();
                gameEnded = true;
            }
            else // a game hasn't ended yet
            {
                // if it is not the player's turn and Player vs Computer . . . The computer must play
                if (!player1Turn && playerVsComp) computer();

                if (player1Turn || !gameEnded || someoneWon) // player1Turn || !playerVsComp || !gameEnded || player1Won || player2Won
                {
                    passRollButtons(true);
                }
            }
        }

        // This method checks whether goal inputted by user is valid and then sets the goal score.
        // Starts gameplay when valid
        // Called when btnSetGoal is pressed!
        private void checkGoal()
        {
            String goalInput = txtbxGoal.Text;
            if (goalInput.Equals("") || Convert.ToInt32(goalInput) <= 0) MessageBox.Show("Enter appropriate goal! Should be > 0");
            else
            {
                // must only accept numbers! INT! --- [ ] and numbers > 0
                lblGoalInfo.Text = goalInput;
                goal = Convert.ToInt32(goalInput);
                beginGame();
            }
        }

        // ------------------- HAS ROLLED SIX / SEVEN ------------------- //

        // This method checks whether either dice has a 6
        // and depending on which player's turn it is, resets their RUNNING SCORE
        private bool hasRolledSix()
        {
            if (leftDiceNum == 6 || rightDiceNum == 6)
            {
                resetRunningScore();
                return true;
            }
            return false;
        }

        // This method checks whether a 7 has been rolled 6,1 or 1,6
        // and depending on which player's turn it is, resets their CUMULATIVE and RUNNING SCORES
        private bool hasRolledSeven()
        {
            if (leftDiceNum == 6 && rightDiceNum == 1 || leftDiceNum == 1 && rightDiceNum == 6)
            {
                resetRunningScore();
                resetCumulativeScore();
                return true;
            }
            return false;
        }

        // This helper method checks the mode of the game, whether it has been changed or if the mode the user is playing in is continuous and not changed
        // This is to help add scores to the correct candidate on the scoreboard 
        private void checkPlayerMode()
        {
            String oldMode = "";

            if (mode != "") oldMode = mode;


            if (rdbtnPlayerComp.Checked)
            {
                playerVsComp = true;
                mode = "pvc"; // player vs. computer

            }
            else if (rdbtn2Player.Checked)
            {
                playerVsComp = false;
                mode = "pvp";
            }

            // this will become true when the game's mode is changed from the original first one picked to another mode
            if (mode != oldMode && oldMode != "")
                changed = !changed;
        }
        
        // This method determines which player starts the game. This is only called when the PLAY button has been clicked on the first time
        private void determineStarter()
        {
            Random rand = new Random();
            if (rand.Next(1, 3) == 1) player1Turn = true;
            else player1Turn = false;

            changeStatus("playerTurn"); // prints out whose turn it is
        }

        // ------------------------------------------------------------------------------ //
        // ----------------------------- I N T E R F A C E ------------------------------ //
        // ------------------------------------------------------------------------------ //

        // This method displays the in-game menu that consists of the goal score 
        // the user also has the option to quit the on-going game
        private void beginGame()
        {
            lblGoalInfo.Text = txtbxGoal.Text;
            hide("gameSettings");
            show("inGameDetails");
            btnQuit.Width = btnPlay.Width; // changes the QUIT BUTTON's size to fill up the group box as PLAY BUTTON is removed
            btnQuit.Left = 100; //
        }

        // ------------------- SHOW PLAY AGAIN MENU ------------------- //
        // This menu is different to the main one at the start of the game.
        // This is for if the game has been played at least once.
        private void playAgainMenu()
        {
            lblP1Score.Text = Convert.ToString(overallP1);
            lblP2Score.Text = Convert.ToString(overallP2);
            if (changed) lblP3Score.Text = Convert.ToString(overallP3);
            hide("game");
            show("playAgain");
            resetGameScore(); // resets all the cumulative and running scores in the game, and nobody has won again (someoneWon = false)
        }

        // updates any label to its correct values to be displayed
        private void updateInterface()
        {
            // running and cumulative scores
            lblRunScoreP1.Text = Convert.ToString(runScoreP1);
            lblCmlScoreP1.Text = Convert.ToString(cmlScoreP1);
            lblRunScoreP2.Text = Convert.ToString(runScoreP2);
            lblCmlScoreP2.Text = Convert.ToString(cmlScoreP2);
            // player details
            lblPlayer1Name.Text = player1;
            lblPlayer2Name.Text = player2;
            lblP1Name.Text = player1;

            txtbxGoal.Text = ""; // resets the text box to be blank, because when quitting game and starting, it has the old game's values
            if (!changed) lblP2Name.Text = player2; // prevents from changing the middle player's score (whichever the oppenent first was
            else lblP3Name.Text = player2;
        }

        // This method creates the dice to be displayed according to the random numbers that represent the dice numbers
        private void createDice(String theDice, int num)
        {
            Graphics dice = picbxLeftDice.CreateGraphics();
            if (theDice == "right")
                dice = picbxRightDice.CreateGraphics();

            dice.FillRectangle(Brushes.SteelBlue, 0, 0, 78, 78);

            int pos = 5;
            int middle = pos * 4;
            int end = pos * 7;

            if (num == 0)
            {
                dice.FillRectangle(Brushes.SteelBlue, 0, 0, 78, 78);
                dice.FillRectangle(Brushes.SteelBlue, 0, 0, 78, 78);
            }
            else
            {
                if (num == 1 || num == 5 || num == 3) // gets the middle spot for these values
                    dice.FillEllipse(Brushes.WhiteSmoke, middle, middle, 10, 10);

                if (num != 1)
                {
                    dice.FillEllipse(Brushes.WhiteSmoke, end, pos, 10, 10); // bottom left
                    dice.FillEllipse(Brushes.WhiteSmoke, pos, end, 10, 10);
                }


                if (num == 4 || num == 5 || num == 6)
                {
                    dice.FillEllipse(Brushes.WhiteSmoke, pos, pos, 10, 10); // top left
                    dice.FillEllipse(Brushes.WhiteSmoke, end, end, 10, 10); // bottom right
                }

                if (num == 6)
                {
                    dice.FillEllipse(Brushes.WhiteSmoke, pos, middle, 10, 10); // mid left
                    dice.FillEllipse(Brushes.WhiteSmoke, end, middle, 10, 10); // mid right
                }
            }
        }

        
        // ------------------------------------------------------------------------------ //
        // ----------------------------- IN-GAME MODIFIERS ------------------------------ //
        // ------------------------------------------------------------------------------ //

        // this method adds the points achieved from a roll of the dice to the respective player in turn
        private void addToRunningScore (int total)
        {
            if (player1Turn) runScoreP1 += total;
            else runScoreP2 += total;
            updateInterface();
        }

        // This method gets the name of the player who is in turn
        private String getCurrentPlayer()
        {
            if (player1Turn) return player1;
            return player2;
        }

        // enables/disables pass and roll button
        private void passRollButtons(bool enable)
        {
            if (enable)
            {
                btnRoll.Enabled = true;
                btnPass.Enabled = true;
            }
            else
            {
                btnRoll.Enabled = false;
                btnPass.Enabled = false;
            }
        }

        // ------------------------------------------------------------------------------ //
        // ------------------------------ STATUS & CONTROL ------------------------------ //
        // ------------------------------------------------------------------------------ //

        // Helper method to slow the game down to view messages that could be missed, etc. 
        private void slowDown(int iMillisec)
        {
            Application.DoEvents();
            System.Threading.Thread.Sleep(iMillisec);
        }

        // This method allows for changes in the messages displayed depending on the game's status
        private void changeStatus(String status)
        {
            switch (status)
            {
                case "pass":
                    lblGameStatus.Text = getCurrentPlayer() + " has passed their turn."; break;
                case "rolledSix":
                    lblGameStatus.Text = getCurrentPlayer() + " rolled 6.\nRunning score reset."; break;
                case "rolledSeven":
                    lblGameStatus.Text = getCurrentPlayer() + " rolled 7.\nPoints reset."; break;
                case "rollingDice":
                    lblGameStatus.Text = getCurrentPlayer() + " is rolling the dice..."; break;
                case "playerTurn":
                    lblGameStatus.Text = getCurrentPlayer() + "\'s turn."; break;
                case "default":
                    lblGameStatus.Text = "Game has started!"; break;

            }
        }

        // This method to help easily specify which components to hide without having to remember what the button names are throughout development
        private void hide(String option)
        {
            switch (option)
            {
                case "main":
                    pnlMain.Hide(); break;
                case "gameSettings":
                    pnlPlayMode.Hide();
                    pnlSetGoal.Hide(); break;
                case "inGameDetails":
                    pnlGameDetails.Hide(); break;
                case "playButton":
                    btnPlay.Hide(); break;
                case "game": // if user quits the game they are playing, the game and in game detail group boxes are hidden. 
                    pnlGameDetails.Hide();
                    pnlGame.Hide();
                    hide("gameSettings"); break;
                case "playAgain":
                    pnlReplay.Hide();break;
            }
        }

        private void show(String option)
        {
            switch (option)
            {
                case "main":
                    pnlMain.Show(); break;
                case "inGameDetails":
                    pnlGameDetails.Show();
                    btnPlay.Show(); break;
                case "playButton":
                    btnPlay.Show(); break;
                case "game": // if user quits the game they are playing, the game and in game detail group boxes are hidden. 
                    pnlGameDetails.Show();
                    pnlGame.Show();
                    show("gameSettings"); break;
                case "playAgain":
                    pnlReplay.Show(); break;
            }
        }

        // ------------------------------------------------------------------------------ //
        // -------------------------- EVENT HANDLERS / BUTTONS -------------------------- //
        // ------------------------------------------------------------------------------ //

        private void btnRoll_Click(object sender, EventArgs e)
        {
            rollDice();
        }

        private void btnPass_Click(object sender, EventArgs e)
        {
            passTurn();
        }

        private void btnStartGame_Click(object sender, EventArgs e)
        {
            pnlPlayMode.Show();
            hide("main");
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnExit2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnScoreBoard_Click(object sender, EventArgs e)
        {
            pnlScoreBoard.Show();
        }
        
        private void btnPlayAgain_Click(object sender, EventArgs e)
        {
            resetGameScore();
            pnlPlayMode.Show();
            hide("playAgain");
            lblCongrats.Text = "";
            pnlScoreBoard.Hide();
        }
        
        // ------------------- GAME SETTINGS ------------------- //
        // Set Mode button
        private void btnSetMode_Click(object sender, EventArgs e)
        {
            setGameMode();
        }

        // sets the game's goal score
        private void btnSetGoal_Click(object sender, EventArgs e)
        {
            checkGoal();
        }

        // ------------------- IN-GAME MENU ------------------- //
        // PLAY BUTTON - this will show the group box consisting of the gameplay (where dice, cumulative and running scores present
        private void btnPlay_Click(object sender, EventArgs e)
        {
            playGame();
        }
        // QUIT BUTTON - quits game --------------------------------------- reset game details [ ]
        private void btnQuit_Click(object sender, EventArgs e)
        {
            quitGame();
        }

       


    }
}
