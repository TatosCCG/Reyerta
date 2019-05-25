using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpButton : MonoBehaviour {

    public GameObject panel;

	public void ShowHelp()
    {
        panel.SetActive(true);
    }
}
