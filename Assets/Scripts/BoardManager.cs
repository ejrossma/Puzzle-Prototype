using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public Player player;

    [Header("Board Data")]
    [Tooltip("Where the pieces will be placed in the hierarchy")]
    [SerializeField] Transform pieceContainer;
    [Tooltip("Size of the board (boardSize x boardSize)")]
    public int boardSize;
    [SerializeField] GameObject[] puzzlePieces;
        
    [Header("Board Visuals")]
    public Transform puzzleOrigin;
    public float horizontalSpacing;
    public float verticalSpacing;    
    [SerializeField] float pieceScale;
    [SerializeField] float scaleUpDuration;
    [SerializeField] float scaleDownDuration;

    //gameBoard is generated from bottom left up
    //(0,0) is the bottom left piece & (boardSize - 1, boardSize - 1) is the top right piece
    //first value is x coord (horizontal)
    //second value is y coord (vertical)
    
    //holds prefabs of pieces
    private GameObject[,] gameBoard;

    //holds values of pieces
    private int[,] intGameBoard;

    public struct Combo
    {

        public Combo(bool row, int i, int l, int e)
        {
            isRow = row;
            index = i;
            length = l;
            endIndex = e;
        }

        //is the combo a row or column combo
        public bool isRow;
        //the index of that row or column
        public int index;
        //the length of the combo
        public int length;
        //the index that the combo ends at
        public int endIndex;

        public override string ToString()
        {
            return $"Combo (isRow: {isRow}, index: {index}, length: {length}, endIndex: {endIndex})";
        }
    }
    
    void Awake()
    {
        gameBoard = new GameObject[boardSize, boardSize];
    }

    // Start is called before the first frame update
    void Start()
    {
        gameBoard = new GameObject[boardSize, boardSize];
        setupIntBoard();
        //generate the initial board
        generateBoard(boardSize);
    }

    //set to -1 to ensure that the values aren't conflicting with piece values for checkNeighbors()
    public void setupIntBoard()
    {
        intGameBoard = new int[boardSize, boardSize];
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                intGameBoard[i, j] = -1;
            }
        }
    }

    //generate a board that is n wide & n tall where n is boardSize
    public void generateBoard(int boardSize)
    {
        //internal piece data
            //TODO - Add weights to the different shapes OR maybe just the star so it gives more money and appears slightly less
        for (int i = 0; i < boardSize; i++)
        {
            int prev = -1;
            int n = -1;
            for (int j = 0; j < boardSize; j++)
            {
                //ensure not too many matching neighbors
                int matchingNeighbors = 100;
                while (matchingNeighbors > 1)
                {
                    n = Random.Range(0, puzzlePieces.Length);
                    matchingNeighbors = checkNeighbors(i, j, n);
                }
                intGameBoard[i,j] = n;
                prev = n;
            }
        }

        //after board is generated eliminate combos
        int comboCount = -1;
        while (comboCount != 0)
        {
            List<Combo> combosToEliminate = findCombo();
            comboCount = combosToEliminate.Count;
            foreach (Combo combo in combosToEliminate)
            {
                eliminateCombo(combo);
            }
        }
        
        //create piece GOs
        for (int i = 0; i < boardSize; i++)
        {
            for (int j = 0; j < boardSize; j++)
            {
                int piece = intGameBoard[i,j];
                GameObject temp = Instantiate(puzzlePieces[piece], 
                                              new Vector3(i * horizontalSpacing + puzzleOrigin.position.x , j * verticalSpacing + puzzleOrigin.position.y, 0), 
                                              Quaternion.identity);
                //organize pieces in heirarchy
                temp.transform.parent = pieceContainer;
                //update piece data
                temp.transform.GetComponent<Piece>().setPos(i, j);
                //update board data
                gameBoard[i,j] = temp;
            }
        }
    }

        private int checkNeighbors(int i, int j, int n)
    {
        int count = 0;

        //check left
        if (i > 0 && intGameBoard[i - 1, j] != -1 && n == (int)intGameBoard[i - 1, j])
            count++;
        
        //check up
        if (j < boardSize - 1 && intGameBoard[i, j + 1] != -1 && n == intGameBoard[i, j + 1])
            count++;
        
        //check down
        //array builds upwards so impossible for value below to not be set
        if (j > 0 && n == intGameBoard[i, j - 1])
            count++;
        
        return count;
    }

    //when the player clicks on a piece
        //1. if they don't have one selected set that piece as their selected
        //2. if they do have one selected ensure that the 2 pieces are adjacent and then swap them
    public void managePiece(Piece piece)
    {
        if (player.getSelected() == null) 
        {
            //select the new piece 
            player.selectPiece(piece);
            //scale the new piece
            StartCoroutine(smoothScale(piece.gameObject.transform, Vector3.one * pieceScale, scaleUpDuration));
            return;
        }

        //get piece
        Piece selectedPiece = player.getSelected();
        //piece always gets scaled down if the player clicked on another piece
        StartCoroutine(smoothScale(player.getSelected().gameObject.transform, Vector3.one, scaleDownDuration));
        //check if piece's position is 1 of the 4 adjacent to the selectedPiece
            //x, y + 1 || x, y - 1 || x + 1, y || x - 1, y
        if (piece.getXPos() == selectedPiece.getXPos() && Mathf.Abs(piece.getYPos() - selectedPiece.getYPos()) == 1 ||
            Mathf.Abs(piece.getXPos() - selectedPiece.getXPos()) == 1  && piece.getYPos() == selectedPiece.getYPos())
        {
            //swap the pieces
            swapPieces(selectedPiece, piece);
            //if there is a combo
            handleCombo(findCombo());
            
            // int comboCount = -1;
            // while (comboCount != 0)
            // {
            //     List<Combo> combos = findCombo();
            //     comboCount = combos.Count;
            //     if (comboCount > 0)
            //         handleCombo(combos);
            // }
        }
        else 
        {
            //deselect current piece
            player.selectPiece(null);
        }
    }

    private void swapPieces(Piece one, Piece two)
    {
        //get values before swap
        int oneX = one.getXPos();
        int oneY = one.getYPos();
        int twoX = two.getXPos();
        int twoY = two.getYPos();

        //swap positions on int game board
        int temp = intGameBoard[oneX, oneY];
        intGameBoard[oneX, oneY] = intGameBoard[twoX, twoY];
        intGameBoard[twoX, twoY] = temp;

        //swap positions on game board
        GameObject tempObj = gameBoard[oneX, oneY];
        gameBoard[oneX, oneY] = two.gameObject;
        gameBoard[twoX, twoY] = tempObj;
        
        //swap positions on pieces
        one.setPos(twoX, twoY);
        two.setPos(oneX, oneY);

        //swap positions in world
        Vector3 tempVec = one.transform.position;
        one.transform.position = two.transform.position;
        two.transform.position = tempVec;
        
        //change player's selected to null
        player.selectPiece(null);
    }

    //return a list of combos found
    private List<Combo> findCombo()
    {
        List<Combo> combos = new List<Combo>();
        //for each column check for a combo
        for (int x = 0; x < boardSize; x++)
        {
            int comboCount = 1;
            int y = 0;
            while (y <= boardSize - 2)
            {
                //check for matches
                if (intGameBoard[x, y] == intGameBoard[x, y + 1])
                {
                    comboCount++;
                }
                else
                {
                    //combo found while still in loop
                    if (comboCount >= 3)
                    {
                        Combo combo = new Combo(false, x, comboCount, y);
                        combos.Add(combo);
                    }
                    //reset since pieces didn't match
                    comboCount = 1;
                }
                y++;
            }
            //check for combo after while loop ends
            if (comboCount >= 3)
            {
                Combo combo = new Combo(false, x, comboCount, y);
                combos.Add(combo);
            }
        }

        //for each row check for a combo
        for (int y = 0; y < boardSize; y++)
        {
            int comboCount = 1;
            int x = 0;
            while (x <= boardSize - 2)
            {
                //check for matches
                if (intGameBoard[x, y] == intGameBoard[x + 1, y])
                {
                    comboCount++;
                }
                else
                {
                    //combo found while still in loop
                    if (comboCount >= 3)
                    {
                        Combo combo = new Combo(true, y, comboCount, x);
                        combos.Add(combo);
                    }
                    //reset since pieces didn't match
                    comboCount = 1;
                }
                x++;
            }
            //check for combo after while loop ends
            if (comboCount >= 3)
            {
                Combo combo = new Combo(true, y, comboCount, x);
                combos.Add(combo);
            }
        }
        return combos;
    }

    private void handleCombo(List<Combo> combos)
    {
        foreach (Combo combo in combos)
        {
            Debug.Log(combo);

            //step 1: give the player gold to reward them
            player.addGold(combo.length);

            //step 2: set all combo'd pieces to -1 so they can be processed afterwards
            for (int i = 0; i < combo.length; i++)
            {
                int indexToRemove = combo.endIndex - i;
                if (combo.isRow)
                {
                    intGameBoard[indexToRemove, combo.index] = -1;
                }
                else
                {
                    intGameBoard[combo.index, indexToRemove] = -1;
                }
            }
        }

        //step 3: identify the pieces that need to fall down 
            //loop through every spot on the board and attempt to drop pieces from above
        for (int x = 0; x < boardSize; x++)
        {
            for (int y = 0; y < boardSize; y++)
            {
                if (intGameBoard[x, y] == -1)
                {
                    int i = y+1;
                    while (intGameBoard[x, i] == -1)
                        i++;
                    //drop the value found down
                    intGameBoard[x, y] = intGameBoard[x, i];
                    //set to -1 since it needs to be replaced now
                    intGameBoard[x, i] = -1;
                }
            }
        }

        //step 4: generate new pieces to replace empty spots
            //loop through and replace every place that is still -1
        for (int x = 0; x < boardSize; x++)
        {
            for (int y = 0; y < boardSize; y++)
            {
                int n = Random.Range(0, puzzlePieces.Length);
                intGameBoard[x, y] = n;
            }
        }
    }

    //get a piece that is in the middle of combo.length to change
    private void eliminateCombo(Combo combo)
    {
        int distanceFromStart;
        //get the middle index
        if (combo.length % 2 == 1)
        {
            distanceFromStart = combo.length / 2;
        }
        //choose one of the two middle indices
        else
        {
            int n = Random.Range(0,1);
            distanceFromStart = (combo.length / 2) - n;
        }
        int indexToChange = combo.endIndex - combo.length + distanceFromStart;
        int pieceToChangeTo = Random.Range(0, puzzlePieces.Length);

        if (combo.isRow)
        {
            //ensure its a different piece
            while (pieceToChangeTo == intGameBoard[indexToChange, combo.index])
            {
                pieceToChangeTo = Random.Range(0, puzzlePieces.Length);
            }
            intGameBoard[indexToChange, combo.index] = pieceToChangeTo;
        }
        else
        {
            //ensure its a different piece
            while (pieceToChangeTo == intGameBoard[combo.index, indexToChange])
            {
                pieceToChangeTo = Random.Range(0, puzzlePieces.Length);
            }
            intGameBoard[combo.index, indexToChange] = pieceToChangeTo;
        }
    }

    IEnumerator smoothScale(Transform pieceToScale, Vector3 targetScale, float scaleDuration)
    {
        Vector3 startScale = pieceToScale.localScale;
        //get a value between 0 and 1 that represents how done the scaling process is
        for (float t = 0; t < 1; t += Time.deltaTime / scaleDuration)
        {
            pieceToScale.localScale = Vector3.Lerp(startScale, targetScale, t);
            yield return null;
        }
    }
}
