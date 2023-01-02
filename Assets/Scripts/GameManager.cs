using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Player player;

    [Header("Board Data")]
    [Tooltip("Where the pieces will be placed in the hierarchy")]
    [SerializeField] Transform pieceContainer;
    [Tooltip("Size of the board (boardSize x boardSize)")]
    [SerializeField] int boardSize;
    [SerializeField] GameObject[] puzzlePieces;

    [Header("Board Layout")]
    public Transform puzzleOrigin;
    public float horizontalSpacing;
    public float verticalSpacing;
    
    //gameBoard is generated from bottom left up
    //(0,0) is the bottom left piece & (boardSize - 1, boardSize - 1) is the top right piece
    //first value is x coord (horizontal)
    //second value is y coord (vertical)
    private GameObject[,] gameBoard;

    // Start is called before the first frame update
    void Start()
    {
        //generate the first board
        gameBoard = new GameObject[boardSize, boardSize];
        generateBoard(boardSize);
    }

    //generate a board that is n wide & n tall where n is boardSize
    void generateBoard(int boardSize)
    {
        //internal piece data + visual for puzzle pieces
        for (int i = 0; i < boardSize; i++)
        {
            for (int j = 0; j < boardSize; j++)
            {
                
                int n = Random.Range(0, puzzlePieces.Length);
                //instantiate a random puzzle piece
                GameObject temp = Instantiate(puzzlePieces[n], 
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

    //when the player clicks on a piece
        //1. if they don't have one selected set that piece as their selected
        //2. if they do have one selected ensure that the 2 pieces are adjacent and then swap them
    public void managePiece(Piece piece)
    {
        if (player.getSelected() == null) { player.selectPiece(piece); return;}

        //check if piece's position is 1 of the 4 adjacent to the selectedPiece
            //x, y + 1 || x, y - 1 || x + 1, y || x - 1, y
        Piece selectedPiece = player.getSelected();
        
        if (piece.getXPos() == selectedPiece.getXPos() && Mathf.Abs(piece.getYPos() - selectedPiece.getYPos()) == 1 ||
            Mathf.Abs(piece.getXPos() - selectedPiece.getXPos()) == 1  && piece.getYPos() == selectedPiece.getYPos())
        {
            swapPieces(selectedPiece, piece);
        }
        else { player.selectPiece(piece); }
    }

    private void swapPieces(Piece one, Piece two)
    {
        //change their positions in script
        Piece tempPiece = one;
        one.setPos(two.getXPos(), two.getYPos());
        two.setPos(one.getXPos(), one.getYPos());

        //change their positions in world
        Vector3 tempVec = one.transform.position;
        one.transform.position = two.transform.position;
        two.transform.position = tempVec;
        
        //change player's selected to null
        player.selectPiece(null);
    }

    //got to find an efficient way to check for combos after player swaps pieces
    //AND when player completes a combo and pieces come falling down
    private bool comparePieces(Piece pieceOne, Piece pieceTwo) { return (pieceOne.pieceType == pieceTwo.pieceType) ? true : false; }
}
