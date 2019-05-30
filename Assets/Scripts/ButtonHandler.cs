using UnityEngine.UI;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

// Class that handles what happens when any button is pressed
public class ButtonHandler : MonoBehaviour
{
    /*** STATIC VARIABLES ***/
    // path to folder which stores all data about playable games 
    private string gamesFolderPath;
    // temporary testing variables
    private static int tempId = 0;


    /*** INSTANCE VARIABLES ***/

    // Important items (objects, variables, etc.) that will be affected
    public Canvas introCanvas;
    // corresponding script on this object
    public GameHandler gameHandler;
    // the game currently being made, null if not in process of creation
    public Game gameBeingMade;


    // Items for choosing a custom game to play
    public Button choosableGameButton; // prefab button, used as template 
    public Canvas chooseGameCanvas;
    public ScrollRect chooseGameScrView;


    // Items for playing a custom game
    public Button playGameButton;
    public Canvas playGameCanvas;
    public GameObject pieceSpawningSlot;


    // Items for the 'Make A Game' process
    public Button makeGameButton;
    public Button doneGameButton;
    public Canvas makeGameCanvas;

    // Items for entering the dimensions (of board) specification process
    public Canvas chooseDimCanvas;

    // Items for starting piece creation process
    public Button makePieceButton;
    public Button donePieceButton;
    public Canvas makePieceCanvas;
    public GameObject pieceBuildingSlot;
    public PieceCreationPanel pieceCreationPanel;
    public float buildSlotSize = 1;

    // Items for starting the board creation process
    public Button makeBoardButton;
    public Button doneBoardButton;
    public Canvas makeBoardCanvas;
    public Slider gapBetweenSlider;
    public BoardCreationPanel boardCreationPanel;
    public float spawnSlotSize = 0.5f;

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
        makeBoardButton.onClick.AddListener(MakeBoard);
        makePieceButton.onClick.AddListener(MakePiece);

        useTheseDimsButton.onClick.AddListener(UseTheseDims);

        doneGameButton.onClick.AddListener(DoneGame);
        doneBoardButton.onClick.AddListener(DoneBoard);
        donePieceButton.onClick.AddListener(DonePiece);
        
    }






    /*** INSTANCE METHODS ***/ 
    // Most of these are functions that activates on button press //

    // Choose a custom game to play 
    public void PlayGame() 
    {
        // centers camera 100 units above origin
        Camera.main.transform.position = new Vector3(0, 100, 0);

        // switch canvas to the 'choose a game' canvas
        introCanvas.gameObject.SetActive(false);
        chooseGameCanvas.gameObject.SetActive(true);


        // ensures games folder exists
        //  will not create/overwrite if folder already exists
        Directory.CreateDirectory(gamesFolderPath);


        // populates list of playable games with clickable buttons
        IEnumerable<string> gameNames = Directory.EnumerateFiles(gamesFolderPath, "*.gam");
        foreach (string gmPath in gameNames) 
        {
            // recover name from path
            int nameStart = gmPath.LastIndexOf('/') + 1;
            int nameEnd = 
                gmPath.IndexOf(".gam", System.StringComparison.Ordinal);
            string gmName = gmPath.Substring(nameStart, nameEnd - nameStart);

            // puts a button with the game's name under the displayed scroll view
            Button gameButton = Instantiate(choosableGameButton); 
            gameButton.transform.SetParent(chooseGameScrView.content.transform, false);
            gameButton.GetComponentInChildren<Text>().text = gmName;
            gameButton.gameObject.SetActive(true);

            // switch canvas and starts game when button is clicked
            gameButton.onClick.AddListener(delegate {
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
    public void MakeGame()
    {
        // switches displayed canvas to appropriate one
        introCanvas.gameObject.SetActive(false);
        chooseDimCanvas.gameObject.SetActive(true);
    }



    // enters the process of creation a custom board
    public void MakeBoard()
    {
        // switches displayed canvas to appropriate one
        makeGameCanvas.gameObject.SetActive(false);
        makeBoardCanvas.gameObject.SetActive(true);

        // TEMP
        Vector3 tempstart = new Vector3(-40, 10, -40);
        GameInfo gmInf = gameBeingMade.info;
        // randomly place pieces on board at start. For debugging
        gmInf.boardAtStart = gmInf.RandomPiecePlacements(gmInf.boardAtStart);

        // tiles and assigns appropriate variables to piece spawning slots
        Utility.TileAct(tempstart, pieceSpawningSlot, spawnSlotSize,
            gmInf.numOfRows, gmInf.numOfCols, gmInf.pieceResolution,
            gapBetweenSlider.value,
            (slot, boardC, boardR, pieceC, pieceR) =>
            {
                // assigns variables
                PieceSpawningSlot spawnSlotScr =
                    slot.GetComponent<PieceSpawningSlot>();
                spawnSlotScr.game = gameBeingMade;
                spawnSlotScr.rowPos = pieceR;
                spawnSlotScr.colPos = pieceC;
                spawnSlotScr.boardRow = boardR;
                spawnSlotScr.boardCol = boardC;
                spawnSlotScr.Spawn();

                // adds object to list of item to destroy after creation process
                Utility.objsToDelete.Add(slot); 
            });
    }


    // generates a board of size numOfRows x numOfCols with no pieces
    //  centers camera 100 units above origin, then enters creation process
    public void UseTheseDims()
    {
        // NOTE: variables can be declared right as they are used in C#
        //   so byte b; f(b); ~ f(byte b);    That's pretty neat
        if (byte.TryParse(numRowsInputField.text, out byte numRows) &&
            byte.TryParse(numColsInputField.text, out byte numCols) &&
            byte.TryParse(pceResField.text, out byte pceRes))
        {

            // gets numRows x numCols board with no pieces coloured 0x000000
            BoardInfo defBoard = BoardInfo.DefaultBoard(numRows, numCols,
                new PosInfo.RGBData(0, 0, 0));

            // sets up a skeleton for the game being created 
            gameBeingMade = new Game(defBoard, new PieceInfo[255]);

            // assigns specified piece resolution
            gameBeingMade.info.pieceResolution = pceRes;

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
        // switch to appropriate canvas
        makeGameCanvas.gameObject.SetActive(false);
        makePieceCanvas.gameObject.SetActive(true);

        byte pceRes = gameBeingMade.info.pieceResolution;
        // 2D array to store information about piece 
        PosInfo[,] pieceVisRep = PosInfo.NothingMatrix(pceRes, pceRes);

        // store information about piece
        pieceCreationPanel.pieceInfo = new PieceInfo("TEMP: TEST", pieceVisRep);

        // scale slot to correct size
        pieceBuildingSlot.transform.localScale = 
            new Vector3(buildSlotSize, buildSlotSize, buildSlotSize);

        // TEMP
        Vector3 start = new Vector3(-20, 10, -20); // needs to be made relative

        Utility.TileAct(start, pieceBuildingSlot, buildSlotSize, 
                        pceRes, pceRes, 1,
                        spawnSlotSize * 0.1f,
                (slot, pieceC, pieceR, _, _1) =>
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



    // appends information about game just created to a file which 
    //  stores information about all playable games
    public void DoneGame() 
    {
        // using .gam extension to stand for 'game' since there
        //  are no obvious conventions... come on people, it's been >30 years!



        // TEMP
        // NOTE: NAME CANNOT BE EMPTY/BLANK! Ensure there's atleast 1 char.
        // name file according to game, prepare to put in games folder
        string gamePath = gamesFolderPath +
            "/TEMP_" + (tempId++) + ".gam";


        // Check if file (game with same name) already exists
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


        // switches back to main screen
        makeGameCanvas.gameObject.SetActive(false);
        introCanvas.gameObject.SetActive(true);

    }


    // ends the procree of creating a custom piece
    public void DonePiece() 
    {
        // switch back to makeGame screen
        makePieceCanvas.gameObject.SetActive(false);
        makeGameCanvas.gameObject.SetActive(true);


        // add created piece to game
        GameInfo gmInf = gameBeingMade.info;
        gmInf.pieces[gmInf.numOfPieces] = pieceCreationPanel.pieceInfo;
        gmInf.numOfPieces++;

        // destroy all game objects generated for creating piece
        Utility.DeleteQueuedObjects();
    }


    // finishes board creation process and returns to game creation screen
    public void DoneBoard() 
    {
        // switch canvas
        makeBoardCanvas.gameObject.SetActive(false);
        makeGameCanvas.gameObject.SetActive(true);

        // delete used objects
        Utility.DeleteQueuedObjects();
    }




}
