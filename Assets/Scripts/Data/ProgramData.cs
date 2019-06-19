using UnityEngine;

// class for storing general information about the program
public class ProgramData
{
    /*** INNER CLASSES ***/
    internal enum State
    {
        ChooseBoardDim,
        ChooseGame,
        ChooseRuleArea,
        ChooseWinCondArea,
        Intro,
        MakeBoard,
        MakeGame,
        MakePiece,
        MakeRule,
        MakeWinCond,
        PaintBoard,
        PanelRule,
        PlayGame,
        RelativeRule
    }





    /*** STATIC VARIABLES ***/
    // path to folder where all games made with/used in this program can be found
    //   (files with .gam extensions)
    internal static string gamesFolderPath; //assigned in SetupHandler

    // current state of the program
    internal static State currentState;
}
