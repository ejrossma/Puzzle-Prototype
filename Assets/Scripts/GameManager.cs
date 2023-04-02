using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
    public Player player;
    public BoardManager bm;

    [Header("Scaling")]
    [SerializeField] float startingDifficulty;
    public float difficulty { get; private set; }

    void Start()
    {
        difficulty = startingDifficulty;
        bm.setupIntBoard();
        bm.generateBoard(bm.boardSize);
    }

    void Update() 
    {
        if (player.getHealth() <= 0)
            GameOver();
    }

    private void GameOver()
    {
        Debug.Log("You Lost.");
    }
}
