using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hexagon : MonoBehaviour {

    public int ownedBy; //{0= noone, 1= player1, 2= player2}
    public SpriteRenderer spriteRenderer;
    public SpriteRenderer highlightSpriteRenderer;
    public SpriteRenderer selectedSpriteRenderer;
    public bool isSelected;
    public bool isSafe;
    public AudioSource audioSource;

    public AudioClip selectSound;

    public void HighlightHexagonOn(Color highlightColor)
    {
        highlightSpriteRenderer.color = highlightColor;
        highlightSpriteRenderer.gameObject.SetActive(true);
    }

    public void HighlightHexagonOff()
    {
        highlightSpriteRenderer.color = Color.white;
        highlightSpriteRenderer.gameObject.SetActive(false);
    }

    public void SelectHexagon()
    {
        Debug.Log(GameManager.instance.selectedeHexagons.Count);
        audioSource.pitch = 0.8f + GameManager.instance.selectedeHexagons.Count/32f;
        audioSource.PlayOneShot(selectSound);

        Color selectedColor = Color.white;

        if (GameManager.instance.turnBelongsTo == 1)
        {
            selectedColor = GameManager.instance.player1SelectionColor;
        }
        else if (GameManager.instance.turnBelongsTo == 2)
        {
            selectedColor = GameManager.instance.player2SelectionColor;
        }

        isSelected = true;
        selectedSpriteRenderer.color = selectedColor;
        selectedSpriteRenderer.gameObject.SetActive(true);

        GameManager.instance.selectedeHexagons.Add(this);
        GameManager.instance.HideAllConquereableHexagons();
        GameManager.instance.ShowConquereableHexagons();
    }

    public void UnselectHexagon()
    {
        
        isSelected = false;
        selectedSpriteRenderer.color = Color.white;
        selectedSpriteRenderer.gameObject.SetActive(false);            
        
    }    

    private void OnMouseDown()
    {
        if(GameManager.instance.turnBelongsTo == 2 && GameManager.instance.vsIA == true)
        {

        }
        else
        {
            if(GameManager.instance.rollButton.activeInHierarchy == true)
            {
                if (highlightSpriteRenderer.gameObject.activeInHierarchy == true && isSelected == false && GameManager.instance.selectedeHexagons.Count < 6)
                {
                    SelectHexagon();
                }
                else if (isSelected == true)
                {
                    UnselectHexagon();
                    GameManager.instance.selectedeHexagons.Remove(this);
                    GameManager.instance.UnselectAll();
                    GameManager.instance.HideAllConquereableHexagons();
                    GameManager.instance.ShowConquereableHexagons();
                }
            }            
        }        
    }

    public void RevertToPrefab()
    {
        ownedBy = 0;
        isSafe = false;
        spriteRenderer.color = Color.white;
        UnselectHexagon();
        HighlightHexagonOff();
    }
}
