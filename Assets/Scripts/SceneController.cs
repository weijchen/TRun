using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneController : MonoBehaviour
{

    [SerializeField] GameObject instructionPanel;
    [SerializeField] GameObject mainGame;
    [SerializeField] GameObject menu;
    [SerializeField] GameObject introAnim;

    private GameManager _gameManager;
    
    void Start()
    {
        instructionPanel.SetActive(false);
        mainGame.SetActive(false);
        introAnim.SetActive(false);
        menu.SetActive(true);
        _gameManager = FindObjectOfType<GameManager>();
    }

    public void StartGame()
    {
        introAnim.SetActive(true);
        menu.SetActive(false);
        //SceneManager.LoadScene("PrologueScene");
        Invoke("StartMainGame", 13.0f);
    }

    public void ExitGame()
    {
        Application.Quit(0);
    }

    public void ShowInstructionPanel()
    {
        instructionPanel.SetActive(true);
        instructionPanel.GetComponent<Animator>().SetBool("isOut", true);
        //Debug.Log("Help");
    }

    public void CloseInstructionPanel()
    {
        //instructionPanel.SetActive(false);
        instructionPanel.GetComponent<Animator>().SetBool("isOut", false);
    }

    public void ShowCreditPanel()
    {
        Debug.Log("Credit");
    }

    void StartMainGame()
    {
        introAnim.SetActive(false);
        mainGame.SetActive(true);
    }
    
}
