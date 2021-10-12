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

    void Start()
    {
        instructionPanel.SetActive(false);
        mainGame.SetActive(false);
        introAnim.SetActive(false);
        menu.SetActive(true);
    }

    public void StartGame()
    {
        introAnim.SetActive(true);
        menu.GetComponent<AudioSource>().enabled = false;
        menu.SetActive(false);

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
        StartCoroutine(AutoCloseInstructionPanel(4.5f));
    }

    IEnumerator AutoCloseInstructionPanel(float timeToClose) {
        yield return new WaitForSeconds(timeToClose);
        instructionPanel.SetActive(false);
        instructionPanel.GetComponent<Animator>().SetBool("isOut", false);
    }

    public void CloseInstructionPanel()
    {
        instructionPanel.SetActive(false);
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
