using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Autohide : MonoBehaviour {

    public float time;

	void OnEnable()
    {
        Invoke("Hide", time);
    }

    void Hide()
    {
        gameObject.SetActive(false);
    }
}
