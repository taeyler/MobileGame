using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]

public class GameOverText : MonoBehaviour
{
    Text gameoverscore;

    void OnEnable()
    {
        gameoverscore = GetComponent<Text>();
        gameoverscore.text = "-Gameover-\n\n" + "Score: " + PlayerPrefs.GetInt("gameOverScore").ToString();
    
    }
}