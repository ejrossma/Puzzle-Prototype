using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    private GameManager gm;

    [Header("Piece Data")]
    [SerializeField] int xPos;
    [SerializeField] int yPos;
    public enum PieceType { Circle, Square, Star, Triangle };
    public PieceType pieceType;

    public void Start()
    {
        gm = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    public void setPos(int x, int y)
    {
        xPos = x;
        yPos = y;
    }
    public int getXPos() { return xPos; }
    public int getYPos() { return yPos; }

    void OnMouseDown()
    {
        gm.managePiece(this);
    }
}
