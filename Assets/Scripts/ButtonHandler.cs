using UnityEngine.UI;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

// TODO
// REFACTOR code into smaller chunks
// Class that handles what happens when any button is pressed
public class ButtonHandler : MonoBehaviour
{
    /*** PRIVATE TYPES ***/
    // describes the current state of the program
    public enum ProgramState
    {
        Intro,
        ChoosingDim,
        ChoosingGame,
        MakingGame,
        MakingPiece,
        MakingBoard,
        PaintingBoard,
        Playing
    }

    /*** STATIC VARIABLES ***/
    // path to folder which stores all data about playable games 
    private string gamesFolderPath;
    // describes what's being done in the current state of the program
    public ProgramState currentProgramState = ProgramState.Intro;
    // temporary testing variables
    private static int tempId = 0; //TODO


    /*** INSTANCE VARIABLES ***/

    // Important items (objects, variables, etc.) that will be affected
    public Canvas introCanvas;
    // corresponding script on this object
    public GameHandler gameHandler;
    // the game currently being made, null if not in process of creation
    public Game gameBeingMade;


    // Items for choosing a custom game to play or edit
    public Button gameButtonTemplate; // prefab button, used as template 
    public Button deleteAllGamesButton; // be very careful! 
    public byte numTimesDeleteAllGamesClickedSinceDeletion;
    public Canvas chooseGameCanvas;
    public ScrollRect chooseGameScrView;
    public Text areYouSureText;


    // Items for playing a custom game
    public Button playGameButton;
    public Canvas playGameCanvas;
    public GameObject pieceSpawningSlot;


    // Items for the 'Make A Game' process
    public Button makeGameButton;
    public Button doneGameButton;
    public InputField enterGameNameInputField;
    public Canvas makeGameCanvas;

    // Items for entering the dimensions (of board) specification process
    public Canvas chooseDimCanvas;

    // Items for starting piece creation process
    public Button makePieceButton;
    public Button donePieceButton;
    public InputField enterPieceNameInputField;
    public Canvas makePieceCanvas;
    public ScrollRect selectPieceScrView;
    public GameObject pieceBuildingSlot;
    public PieceCreationPanel pieceCreationPanel;
    public float buildSlotSize = 1;

    // Items for starting the board creation process
    public Button makeBoardButton;
    public Button doneBoardButton;
    public Button pieceButtonTemplate;
    public Button removePieceButton;
    public Canvas makeBoardCanvas;
    public Slider gapBetweenSlider;
    public BoardCreationPanel boardCreationPanel;
    public float boardSquareSize = 1f;

    // Items from the board dimensions setup screen 
    public Button useTheseDimsButton;
    public InputField numRowsInputField;
    public InputField numColsInputField;
    public InputField pceResField;



    /*** AWAKE - CALLED BEFORE START ***/
    private void Awake()
    {
        // Application.persistentDataPath links to a folder which contains
        //  information about the gamess
        //  It can only be accessed in the Start or Awake method for MonoBehaviours
        gamesFolderPath = Application.persistentDataPath + "/games";
    }



    /*** ACTIVATES AT START OF PROGRAM ***/
    // Start is called before the first frame update
    // assigns handler to every single button
    private void Start()
    {
        // Add handlers to buttons   
        playGameButton.onClick.AddListener(PlayGame);


        makeGameButton.onClick.AddListener(MakeGame);
        useTheseDimsButton.onClick.AddListener(UseTheseDims);
        doneGameButton.onClick.AddListener(DoneGame);

        deleteAllGamesButton.onClick.AddListener(DeleteAllGames);

        makeBoardButton.onClick.AddListener(MakeBoard);
        removePieceButton.onClick.AddListener(RemovePiece);
        doneBoardButton.onClick.AddListener(DoneBoard);

        makePieceButton.onClick.AddListener(MakePiece);
        donePieceButton.onClick.AddListener(DonePiece);
    }






    /*** INSTANCE METHODS ***/ 
    // Most of these are functions that activates on button press //

    // Choose a custom game to play 
    // NOTE: play game button takes user to the choose game canvas
    public void PlayGame() 
    {
        // centers camera 100 units above origin
        Camera.main.transform.position = new Vector3(0, 100, 0);

        // updates state
        currentProgramState = ProgramState.ChoosingGame;

        // switch canvas to the 'choose a game' canvas
        introCanvas.gameObject.SetActive(false);
        chooseGameCanvas.gameObject.SetActive(true);


        // TODO replace this with unified method

        // ensures games folder exists
        //  will not create/overwrite if folder already exists
        Directory.CreateDirectory(gamesFolderPath);

        // clear previous list of games 
        foreach (Button b in chooseGameScrView.content.GetComponentsInChildren<Button>())
        {
            if (!b.Equals(gameButtonTemplate)) 
            {
                Destroy(b.gameObject);
            }
        }

        // populates list of playable games with clickable buttons
        IEnumerable<string> gameNames = Directory.EnumerateFiles(gamesFolderPath, "*.gam");
        foreach (string gmPath in gameNames) 
        {
            // recover name from path
            int nameStart = gmPath.LastIndexOf('/') + 1;
            int nameEnd = 
                gmPath.IndexOf(".gam", System.StringComparison.Ordinal);
            string gmName = gmPath.Substring(nameStart, nameEnd - nameStart);
            // recover display name (player inputted) by putting spaces back in
            gmName = gmName.Replace('_', ' ');

            // puts a button with the game's name under the displayed scroll view
            // switch canvas and starts game when button is clicked
            Button gameButton = 
                Utility.CreateButton(gameButtonTemplate, chooseGameScrView.content, gmName, 
                delegate
                {
                    // switch canvas 
                    chooseGameCanvas.gameObject.SetActive(false);
                    playGameCanvas.gameObject.SetActive(true);

                    // retrieve game information from games folder
                    BinaryFormatter bf = new BinaryFormatter();
                    FileStream gameFile = File.Open(gmPath, FileMode.Open);
                    GameInfo gmInfo = (GameInfo)(bf.Deserialize(gameFile));
                    gameFile.Close();


                    // assign and start the game
                    gameHandler.game = new Game(gmInfo);
                    gameHandler.Play();
                });


        }




        // TODO
    }



    // enters process of creating a custom game
    // Note that the MakeGame button takes users to the choosing dimension canvas
    public void MakeGame()
    {
        // updates state
        currentProgramState = ProgramState.ChoosingDim;

        // switches displayed canvas to appropriate one
        introCanvas.gameObject.SetActive(false);
        chooseDimCanvas.gameObject.SetActive(true);
    }



    // enters the process of creation a custom board
    public void MakeBoard()
    {
        // updates the current program state
        currentProgramState = ProgramState.MakingBoard;

        // switches displayed canvas to appropriate one
        makeGameCanvas.gameObject.SetActive(false);
        makeBoardCanvas.gameObject.SetActive(true);

        // populates the scroll view with buttons labeled with piece names
        for (byte index = 0; index < gameBeingMade.info.pieces.Count; index++)
        {
            // when clicked change colours of buttons
            //  and assigns piece associated to be current piece selected

            // index of the associated piece 
            //  index should not be used directly, as it *will* change
            //  after this iteration of the loop ends
            // indexAssocPiece is kind of like an upvalue in Lua
            byte indexAssocPiece = index;
            PieceInfo pce = gameBeingMade.info.pieces[index];

            Button pceButton = 
                Utility.CreateButton(pieceButtonTemplate, selectPieceScrView.content, 
                pce.pieceName, 
                (btn) => delegate 
                {
                    // retrieve all buttons under the piece selection scrollview
                    Button[] buttons =
                        selectPieceScrView.content.GetComponentsInChildren<Button>();
                    // changes colour of all piece selection buttons back
                    foreach (Button b in buttons)
                    {
                        b.GetComponent<Image>().color = Color.white;
                    }

                    // change colour of remove piece button
                    removePieceButton.GetComponent<Image>().color = Color.white;



                    // TODO 
                    // TEMP
                    // DEBUG
                    // index of piece selected
                    Debug.Log("INDEX OF PIECE SELECTED: " + indexAssocPiece);


                    // changes piece selected
                    boardCreationPanel.pieceSelected = indexAssocPiece;

                    // changes this button's colour
                    btn.GetComponent<Image>().color =
                        BoardCreationPanel.selectedPieceColour;
                });
        }

        // default 'piece (to be placed) selected' is 'no piece'
        //   (due to possibility of there being, well, no pieces made)
        boardCreationPanel.pieceSelected = PosInfo.noPiece;


        // retrieves game info, finds position to start tiling
        GameInfo gmInf = gameBeingMade.info;
        BoardInfo startBoard = gmInf.boardAtStart;
        Vector3 start = new Vector3(-startBoard.width/2, 10, -startBoard.height/2);

        // tiles and assigns appropriate variables to piece spawning slots
        float spawnSlotSize = boardSquareSize / gmInf.pieceResolution;
        Utility.TileAct(start, pieceSpawningSlot, spawnSlotSize,
            gmInf.numOfRows, gmInf.numOfCols, gmInf.pieceResolution,
            gapBetweenSlider.value,
            (slot, boardR, boardC, pieceR, pieceC) =>
            { 
                // assigns variables
                PieceSpawningSlot spawnSlotScr =
                    slot.GetComponent<PieceSpawningSlot>();
                spawnSlotScr.game = gameBeingMade;
                spawnSlotScr.rowPos = pieceR;
                spawnSlotScr.colPos = pieceC;
                spawnSlotScr.boardRow = boardR;
                spawnSlotScr.boardCol = boardC;

                // notes that this spawning slot is currently used
                gameBeingMade.spawningsSlots[boardR, boardC].Add(spawnSlotScr); 

                // spawns cube at the corresponding position relative to the piece
                spawnSlotScr.Spawn();

                // adds object to list of item to destroy after creation process
                Utility.objsToDelete.Add(slot); 
            });
    }


    // change piece selected to 'No Piece' (and also change button colours)
    //   This is clickable during the board creation process
    public void RemovePiece() 
    {
        // sets piece selected to no piece
        boardCreationPanel.pieceSelected = PosInfo.noPiece;

        // changes the backgrounds of all the other piece buttons to white
        Button[] buttons =
            selectPieceScrView.content.GetComponentsInChildren<Button>();
        foreach (Button b in buttons)
        {
            b.GetComponent<Image>().color = Color.white;
        }

        // make the 'no piece' button change colour
        removePieceButton.GetComponent<Image>().color =
            BoardCreationPanel.selectedPieceColour;
    }


    // generates a board of size numOfRows x numOfCols with no pieces
    //  centers camera 100 units above origin, then enters creation process
    public void UseTheseDims()
    {
        // NOTE: variables can be declared right as they are used in C#
        //   so byte b; f(b); ~ f(byte b);    That's pretty neat
        //   also, 'out' just means the variable is passed/returned by reference
        if (byte.TryParse(numRowsInputField.text, out byte numRows) &&
            byte.TryParse(numColsInputField.text, out byte numCols) &&
            byte.TryParse(pceResField.text, out byte pceRes))
        {

            // gets numRows x numCols board with no pieces coloured 0x000000
            BoardInfo defBoard = BoardInfo.DefaultBoard(numRows, numCols,
                boardSquareSize, gapBetweenSlider.value, new PosInfo.RGBData(0, 0, 0));

            // sets up a skeleton for the game being created 
            gameBeingMade = new Game(defBoard, new List<PieceInfo>());

            // assigns specified piece resolution
            gameBeingMade.info.pieceResolution = pceRes;

            // updates state
            currentProgramState = ProgramState.MakingGame;

            // enters creation process
            chooseDimCanvas.gameObject.SetActive(false);
            makeGameCanvas.gameObject.SetActive(true);

            // centers camera 100 units above origin
            Camera.main.transform.position = new Vector3(0, 100, 0);
        }
    }



    // enters the process of creating a custom piece
    public void MakePiece()
    {
        // updates state
        currentProgramState = ProgramState.MakingPiece;

        // switch to appropriate canvas
        makeGameCanvas.gameObject.SetActive(false);
        makePieceCanvas.gameObject.SetActive(true);

        byte pceRes = gameBeingMade.info.pieceResolution;
        // 2D array to store information about piece 
        PosInfo[,] pieceVisRep = PosInfo.NothingMatrix(pceRes, pceRes);

        // store information about piece
        pieceCreationPanel.pieceInfo = new PieceInfo("UNNAMED", pieceVisRep);

        // scale slot to correct size
        pieceBuildingSlot.transform.localScale = 
            new Vector3(buildSlotSize, buildSlotSize, buildSlotSize);

        // calculate start position
        // reminder: planes in Unity are 10 units by 10 units
        float sideLength = pceRes * buildSlotSize * 10 + // size of slots
            (pceRes - 1) * buildSlotSize; //(* 0.1f * 10) // size of gaps
        Vector3 start = new Vector3(-sideLength / 2, 10, -sideLength / 2);

        // tiles temporary "board" representing square to place piece on
        Utility.TileAct(start, pieceBuildingSlot, buildSlotSize, 
                        pceRes, pceRes, 1,
                        buildSlotSize * 0.1f,
                (slot, pieceR, pieceC, _, _1) =>
                {
                    // associates panel with slot,
                    //  and assigns co-ordinates to slot corresponding to its "position 
                    //  in the array"
                    slot.GetComponent<PieceBuildingSlot>().associatedPanel =
                        pieceCreationPanel;
                    slot.GetComponent<PieceBuildingSlot>().rowPos = pieceR;
                    slot.GetComponent<PieceBuildingSlot>().colPos = pieceC;

                    // add to list of building slots used for building the piece
                    Utility.objsToDelete.Add(slot);

                    // temp. debug variable
                    slot.GetComponent<PieceBuildingSlot>().slotId = tempId;
                    tempId++;
                });
    }


    // DELETE EVERY GAME STORED INSIDE OF THE GAMES FOLDER
    //  THAT IS WHERE ALL GAMES ARE LOCATED WHEN CREATED WITH THIS PROGRAM!
    public void DeleteAllGames() 
    {
        if (numTimesDeleteAllGamesClickedSinceDeletion > 0) 
        {
            // deletes all games, and resets the content of the scroll view
            Utility.DeleteAllSavedGames();

            // hide warning text
            areYouSureText.text = "";

            // reset num of times it has been clicked since last deletion
            numTimesDeleteAllGamesClickedSinceDeletion = 0;

            // change warning text back
            deleteAllGamesButton.GetComponentInChildren<Text>().text = 
                "DELETE ALL GAMES";

            // updates game state
            currentProgramState = ProgramState.Intro;

            // switches canvas back to main/intro canvas 
            //  (there's no game to choose to play anymore, why stay there?)
            chooseGameCanvas.gameObject.SetActive(false);
            introCanvas.gameObject.SetActive(true);
        } 
        else 
        {
            // asks, if has not asked already
            areYouSureText.text = "ARE YOU SURE?";
            deleteAllGamesButton.GetComponentInChildren<Text>().text = "YES!!!";

            numTimesDeleteAllGamesClickedSinceDeletion++; //increment count
        }
    }


    // appends information about game just created to a file which 
    //  stores information about all playable games
    public void DoneGame() 
    {
        // using .gam extension to stand for 'game' since there
        //  are no obvious conventions... 


        // Check if file (game with same name) already exists
        // check that name is proper (alphanum, non-empty)
        // TODO
        string gameName = enterGameNameInputField.text;
        enterGameNameInputField.text = ""; //reset text in input field

        // name file according to game, prepare to put in games folder
        // spaces are replaced with underscrolls for developpers' convenience
        //  shall be replaced back with spaces when displayed
        string gamePath = gamesFolderPath +
            "/" + (gameName.Replace(' ', '_')) + ".gam";



        // TODO
        // TEMP 
        // creates a file called TEMP_(id#) inside the games folder to 
        //  store all information about created game
        FileStream gameFile = File.Open(gamePath, FileMode.Create);

        // TEMP
        // serializes game data to file 
        //  temporarily tests serializing a '0'
        BinaryFormatter binFmt = new BinaryFormatter();
        binFmt.Serialize(gameFile, gameBeingMade.info);


        // closes the file
        gameFile.Close();

        // updates state
        currentProgramState = ProgramState.Intro;

        // switches back to main screen
        makeGameCanvas.gameObject.SetActive(false);
        introCanvas.gameObject.SetActive(true);

    }


    // ends the procree of creating a custom piece
    public void DonePiece() 
    {

        // get name from input field
        // TODO: add check, ensure name is alphanumeric, prevent name clashes
        //       make sure name not empty string
        string pceName = enterPieceNameInputField.text;
        enterPieceNameInputField.text = ""; // resets text field

        pieceCreationPanel.pieceInfo.pieceName = pceName;

        // add created piece to game
        GameInfo gmInf = gameBeingMade.info;
        gmInf.AddPiece(pieceCreationPanel.pieceInfo);

        // destroy all game objects generated for creating piece
        Utility.DeleteQueuedObjects();

        // update state
        currentProgramState = ProgramState.MakingGame;

        // switch back to makeGame screen
        makePieceCanvas.gameObject.SetActive(false);
        makeGameCanvas.gameObject.SetActive(true);
    }


    // finishes board creation process and returns to game creation screen
    public void DoneBoard() 
    {
        // delete used objects
        Utility.DeleteQueuedObjects();

        // update state
        currentProgramState = ProgramState.MakingGame;

        // switch canvas
        makeBoardCanvas.gameObject.SetActive(false);
        makeGameCanvas.gameObject.SetActive(true);
    }




}
