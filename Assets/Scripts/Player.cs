using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Player : MonoBehaviour
{
    [SerializeField] GameManager gm;   
    private Piece selectedPiece;

    [Header("Stats")]
    [SerializeField] int health;
    [SerializeField] int gold;

    [Header("UI")]
    [SerializeField] TMP_Text playerGoldText;
    [SerializeField] TMP_Text playerHealthText;

    public void selectPiece(Piece toBeSelected) { selectedPiece = toBeSelected; }
    public Piece getSelected() { return selectedPiece; }

    public void setHealth(int newHealth) { health = newHealth; }
    public int getHealth() { return health; }
    public void loseHealth(int healthToLose)
    { 
        health -= healthToLose; 
        playerHealthText.text = $"Health: {health}";
    }
    
    public void setGold(int newGold) { gold = newGold; }
    public int getGold() { return gold; }
    public void addGold(int goldToAdd) 
    { 
        gold += (int)(goldToAdd * gm.difficulty);
        playerGoldText.text = $"Gold: {gold}";
    }

}
