using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseController : MonoBehaviour
{
    [SerializeField]
    GameObject mainGame;
    [SerializeField]
    GameObject menu;
    [SerializeField]
    GameObject introAnim;
    // Start is called before the first frame update
    void Start()
    {
        mainGame.SetActive(false);
        introAnim.SetActive(false);
        menu.SetActive(true);
    }

    

    // Update is called once per frame
    void Update()
    {
        
    }
}
