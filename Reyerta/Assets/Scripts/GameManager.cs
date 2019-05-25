using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;
    public bool vsIA;
    public GameObject rollButton;

    public Slider sliderP1;
    public Slider sliderP2;

    [Space(10)]
    public GameObject die;
    public GameObject number1;
    public GameObject number2;
    public GameObject number3;
    public GameObject number4;
    public GameObject number5;
    public GameObject number6;

    public GameObject yes;
    public GameObject no;

    [Space(10)]

    public GameObject happyFace;
    public GameObject neutralFace;
    public GameObject anxiousFace;

    public float highY, lowY;
    public Transform manTransform;

    public Text textBocadillo;
    public GameObject bocadillo;
    public AudioClip rollSound;
    public AudioClip yesSound;
    public AudioClip noSound;
    public AudioSource audioSource;

    private int hexLooseByIA;
    private int matches = 0;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);            
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public List<Adjacencies> matrix;
    public List<Hexagon> hexagons;
    public List<Hexagon> selectedeHexagons;
    private List<Hexagon> highlightedHexagons;

    [Space(10)]
    public Color highlightColor;
    public Color player1MainColor;
    public Color player1SelectionColor;
    public Color player2MainColor;
    public Color player2SelectionColor;

    [Space(10)]
    public int turnBelongsTo;
    private int hexOwnedByThePlayer;

    private void Start()
    {
        if(matches > 0)
        {
            ShowText("Good Luck my dear and drunkard friend!", true);
        }
        else
        {
            ShowText("Good Luck my dear friend!");
        }

        selectedeHexagons = new List<Hexagon>();
        highlightedHexagons = new List<Hexagon>();

        //turnBelongsTo = 1;
        ConquerHexagon(hexagons[16], 1);
        ConquerHexagon(hexagons[2], 2);
        hexagons[16].isSafe = true;
        hexagons[2].isSafe = true;

        sliderP1.value = 1;
        sliderP2.value = 1;

        ShowConquereableHexagons();
        StartCoroutine(CheckIA());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    public List<Hexagon> neighboursOf(int index)
    {
        List<Hexagon> res = new List<Hexagon>();
        foreach (int i in matrix[index].adjacencies)
        {
            res.Add(hexagons[i]);
        }
        return res;
    }

    public void NextTurn()
    {
        selectedeHexagons = new List<Hexagon>();
        highlightedHexagons = new List<Hexagon>();

        turnBelongsTo++;
        if(turnBelongsTo > 2)
        {
            turnBelongsTo = 1;
        }

        HideAllConquereableHexagons();
        ShowConquereableHexagons();

        StartCoroutine(CheckIA());
    }

    public void ShowConquereableHexagons()
    {
        int player = turnBelongsTo;
        int hexNotOwnedByThePlayer = 0;
        //Color highlightColor = Color.white;
        //if (player == 1)
        //{
        //    highlightColor = player1HighlightColor;
        //}
        //else if (player == 2)
        //{
        //    highlightColor = player2HighlightColor;
        //}

        foreach (Hexagon hex in hexagons)
        {
            if(hex.ownedBy != player)
            {
                hexNotOwnedByThePlayer++;
                foreach (int hexagonIndex in matrix[int.Parse(hex.name)].adjacencies)
                {
                    if ((hexagons[hexagonIndex].ownedBy == player || hexagons[hexagonIndex].isSelected == true) && hex.isSafe == false)
                    {                        
                        hex.HighlightHexagonOn(highlightColor);
                        highlightedHexagons.Add(hex);
                        break;
                    }
                }
            }
            else
            {
                hex.HighlightHexagonOff();
            }
        }
        hexOwnedByThePlayer = 19 - hexNotOwnedByThePlayer;        
    }

    public void HideAllConquereableHexagons()
    {
        foreach (Hexagon hex in hexagons)
        {
            hex.HighlightHexagonOff();
        }
    }

    public void ConquerHexagon(Hexagon hexagon, int player)
    {
        if(hexagon.ownedBy != 0 && turnBelongsTo == 1)
        {            
            hexLooseByIA++;            
        }

        hexagon.ownedBy = player;
        if (player == 1)
        {
            hexagon.spriteRenderer.color = player1MainColor;
        }
        else if (player == 2)
        {
            hexagon.spriteRenderer.color = player2MainColor;
        }
        hexagon.UnselectHexagon();
        hexOwnedByThePlayer++;
    }

    public void UnselectAll()
    {
        foreach (Hexagon hex in selectedeHexagons)
        {
            hex.UnselectHexagon();
        }

        selectedeHexagons = new List<Hexagon>();
    }

    public void RollButton()
    {
        rollButton.SetActive(false);
        StartCoroutine(RollDice());
    }

    IEnumerator RollDice()
    {
        audioSource.PlayOneShot(rollSound);
        int roll = Random.Range(1, 7);

        if(roll == 1)
        {
            number1.SetActive(true);
        }
        else if (roll == 2)
        {
            number2.SetActive(true);
        }
        else if (roll == 3)
        {
            number3.SetActive(true);
        }
        else if (roll == 4)
        {
            number4.SetActive(true);
        }
        else if (roll == 5)
        {
            number5.SetActive(true);
        }
        else if (roll == 6)
        {
            number6.SetActive(true);
        }

        hexLooseByIA = 0;

        yield return new WaitForSeconds(1f);
        die.SetActive(true);
        if (roll >= selectedeHexagons.Count)
        {
            audioSource.PlayOneShot(yesSound);
            yes.SetActive(true);
            foreach (Hexagon hex in selectedeHexagons)
            {                
                ConquerHexagon(hex, turnBelongsTo);
            }
            yield return new WaitForSeconds(1.5f);

            if (turnBelongsTo == 1)
            {
                if (selectedeHexagons.Count == 6)
                {
                    ShowText("Inconceivable!");
                }
                else if(hexLooseByIA > 0)
                {
                    if(hexLooseByIA <=2 && sliderP1.value < 6)
                    {
                        ShowText("How rude!");
                    }
                    else if(hexLooseByIA > 2 && hexLooseByIA < 5)
                    {
                        ShowText("Sir, you are a filthy being.");
                    }
                    else if(hexLooseByIA >= 5)
                    {
                        ShowText("Endogamy is quite noticeable in your family.");
                    }
                }
            }
            else
            {
                if (selectedeHexagons.Count == 5)
                {
                    ShowText("Byte the dust!");
                }
                else if(selectedeHexagons.Count == 6)
                {
                    ShowText("Muahahahaha!");
                }
            }
        }
        else
        {
            audioSource.PlayOneShot(noSound);
            no.SetActive(true);
            foreach (Hexagon hex in selectedeHexagons)
            {
                hex.UnselectHexagon();
            }
            yield return new WaitForSeconds(1.5f);

            if(turnBelongsTo == 1)
            {
                if(sliderP1.value > 3)
                {
                    ShowText("Maybe next time.");
                }
                else if(sliderP1.value > 6)
                {
                    ShowText("Ha ha ha ha!");
                }
                else if(sliderP1.value > 9)
                {
                    ShowText("Screw you my dear.");
                }
            }
        }

        die.SetActive(false);
        number1.SetActive(false);
        number2.SetActive(false);
        number3.SetActive(false);
        number4.SetActive(false);
        number5.SetActive(false);
        number6.SetActive(false);
        yes.SetActive(false);
        no.SetActive(false);


        CheckVictory();
    }

    public void CheckVictory()
    {
        if(turnBelongsTo == 1)
        {
            sliderP1.value = hexOwnedByThePlayer;
        }
        else
        {
            sliderP2.value = hexOwnedByThePlayer;
        }

        manTransform.position = Vector3.Lerp(new Vector3(manTransform.position.x, highY, 0), new Vector3(manTransform.position.x, lowY, 0), sliderP1.value / 14f);

        if(sliderP1.value > 8 || sliderP1.value > sliderP2.value + 2)
        {
            happyFace.SetActive(false);
            neutralFace.SetActive(false);
            anxiousFace.SetActive(true);
        }
        else if(sliderP1.value + 4 < sliderP2.value)
        {
            happyFace.SetActive(true);
            neutralFace.SetActive(false);
            anxiousFace.SetActive(false);
        }
        else
        {
            happyFace.SetActive(false);
            neutralFace.SetActive(true);
            anxiousFace.SetActive(false);
        }

        if(hexOwnedByThePlayer > 13)
        {
            if(turnBelongsTo == 1)
            {
                ShowText("Eres un fistro.\nYOU have won!", true);
                Invoke("Rematch", 5f);
            }
            else
            {
                ShowText("I win! Looooooooser!", true);
                Invoke("Rematch", 3f);
            } 
            

        }

        else
        {
            NextTurn();
        }
    }

    void Rematch()
    {
        foreach(Hexagon hex in hexagons)
        {
            //PrefabUtility.RevertPrefabInstance(hex.gameObject);
            hex.RevertToPrefab();
        }

        turnBelongsTo++;
        if (turnBelongsTo > 2)
        {
            turnBelongsTo = 1;
        }

        matches++;

        Start();
    }
        
    IEnumerator CheckIA()
    {
        if(turnBelongsTo == 2 && vsIA == true)
        {
            //happyFace.SetActive(false);
            //neutralFace.SetActive(false);
            //anxiousFace.SetActive(true);
            //rollButton.gameObject.SetActive(false);

            List<int> probabilityDeck = new List<int>(10) { 1, 1, 1, 2, 2, 3, 3, 4, 5, 6 };
            int numToConquer = probabilityDeck[Random.Range(0, probabilityDeck.Count)];

            yield return new WaitForSeconds(1f);


            for (int i = 0; i < numToConquer; i++)
            {

                foreach (Hexagon hex2 in selectedeHexagons)
                {
                    highlightedHexagons.Remove(hex2);
                }

                bool selectionMade = false;
                foreach(Hexagon h in highlightedHexagons)
                {
                    if(h.ownedBy == 1 && selectedeHexagons.Contains(h) == false)
                    {
                        h.SelectHexagon();
                        selectionMade = true;
                        break;
                    }

                    if (Random.value > 0.9f)
                    {
                        break;
                    }

                }
                if(selectionMade == false)
                {
                    Hexagon hex = highlightedHexagons[Random.Range(0, highlightedHexagons.Count)];
                    hex.SelectHexagon();
                }                

                foreach(Hexagon hex2 in selectedeHexagons)
                {
                    highlightedHexagons.Remove(hex2);
                }

                yield return new WaitForSeconds(1.5f);
            }

            StartCoroutine(RollDice());
        }
        else
        {
            happyFace.SetActive(false);
            neutralFace.SetActive(true);
            anxiousFace.SetActive(false);
            rollButton.gameObject.SetActive(true);
        }
    }

    public void ShowText(string s, bool forced = false)
    {
        if(forced == true || bocadillo.activeInHierarchy == false)
        {
            textBocadillo.text = s;
            bocadillo.SetActive(true);
        }
    }

    //public void CheckIsolatedSelected()
    //{

    //    int player = turnBelongsTo;

    //    foreach (Hexagon hex in selectedeHexagons)
    //    {
    //        foreach (int hexagonIndex in matrix[int.Parse(hex.name)].adjacencies)
    //        {
    //            if (hexagons[hexagonIndex].ownedBy == player || hexagons[hexagonIndex].isSelected == true)
    //            {
    //                hex.HighlightHexagonOn(highlightColor);
    //                break;
    //            }
    //        }
    //    }
    //}
}
