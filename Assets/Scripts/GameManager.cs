using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] Transform pieceContainer;
    [SerializeField] int boardSize;
    [SerializeField] GameObject[] puzzlePieces;
    

    // Start is called before the first frame update
    void Start()
    {
        generateBoard(boardSize);
    }

    //generate a board that is n wide & n tall where n is boardSize
    void generateBoard(int boardSize)
    {
        for (int i = 1; i < boardSize + 1; i++)
        {
            for (int j = 1; j < boardSize + 1; j++)
            {
                int n = Random.Range(0, puzzlePieces.Length);
                GameObject temp = Instantiate(puzzlePieces[n], new Vector3(i - boardSize/2, j - boardSize/2, 0), Quaternion.identity);
                temp.transform.parent = pieceContainer;
            }
        }
    }
}
