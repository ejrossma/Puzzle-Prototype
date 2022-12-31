using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Tooltip("Where the pieces will be placed in the hierarchy")]
    [SerializeField] Transform pieceContainer;
    [Tooltip("Size of the board (boardSize x boardSize)")]
    [SerializeField] int boardSize;
    [SerializeField] GameObject[] puzzlePieces;
    
    //gameBoard is generated from bottom left up
        //(0,0) is the bottom left piece
        //(boardSize - 1, boardSize - 1) is the top right piece
    //first value is x coord (horizontal)
    //second value is y coord (vertical)
    private GameObject[,] gameBoard;
    

    // Start is called before the first frame update
    void Start()
    {
        //generate the first board
        gameBoard = new GameObject[boardSize, boardSize];
        generateBoard(boardSize);
        Debug.Log(comparePieces(gameBoard[0,0].transform.GetComponent<Piece>(), gameBoard[1,0].transform.GetComponent<Piece>()));
    }

    //generate a board that is n wide & n tall where n is boardSize
        //next step is generating a board with no combos to start
    void generateBoard(int boardSize)
    {
        //internal piece data + visual for puzzle pieces
        for (int i = 1; i < boardSize + 1; i++)
        {
            for (int j = 1; j < boardSize + 1; j++)
            {
                
                int n = Random.Range(0, puzzlePieces.Length);
                //instantiate a random puzzle piece
                GameObject temp = Instantiate(puzzlePieces[n], new Vector3(i - boardSize/2, j - boardSize/2, 0), Quaternion.identity);
                //organize pieces in heirarchy
                temp.transform.parent = pieceContainer;
                //update piece data
                temp.transform.GetComponent<Piece>().setPos(i - 1, j - 1);
                //update board data
                gameBoard[i-1,j-1] = temp;
            }
        }
    }

    //check to see if data is working
        //next step make this check for combos
            //got to find an efficient way to check for combos after player swaps pieces
            //AND when player completes a combo and pieces come falling down
    private bool comparePieces(Piece pieceOne, Piece pieceTwo)
    {
        bool temp = (pieceOne.pieceType == pieceTwo.pieceType) ? true : false;
        return temp;
    }
}
