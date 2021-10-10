using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneController : MonoBehaviour
{

    [SerializeField]
    GameObject instructionPanel;
    // Start is called before the first frame update
    void Start()
    {
        instructionPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame()
    {
        Debug.Log("Start");
        //SceneManager.LoadScene("PrologueScene");

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
    
}
