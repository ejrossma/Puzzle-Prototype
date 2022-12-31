using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    [Header("Piece Data")]
    [SerializeField] int xPos;
    [SerializeField] int yPos;
    public enum PieceType { Circle, Square, Star, Triangle };
    public PieceType pieceType;

    public void setPos(int x, int y)
    {
        xPos = x;
        yPos = y;
    }
}
