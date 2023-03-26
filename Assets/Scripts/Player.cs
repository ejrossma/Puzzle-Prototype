using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] GameManager gm;
    [SerializeField] int health;
    [SerializeField] int gold;
    [SerializeField] Piece selectedPiece;

    public void selectPiece(Piece toBeSelected) { selectedPiece = toBeSelected; }
    public Piece getSelected() { return selectedPiece; }

    public void setHealth(int newHealth) { health = newHealth; }
    public int getHealth() { return health; }
    
    public void addGold(int goldToAdd) { gold += goldToAdd; }
    public void setGold(int newGold) { gold = newGold; }
    public int getGold() { return gold; }


}
