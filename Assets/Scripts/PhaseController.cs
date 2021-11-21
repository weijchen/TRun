using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseController : MonoBehaviour
{
    [SerializeField] GameObject mainGame;
    [SerializeField] GameObject menu;
    [SerializeField] GameObject introAnim;
    
    void Start()
    {
        mainGame.SetActive(false);
        introAnim.SetActive(false);
        menu.SetActive(true);
    }
}
