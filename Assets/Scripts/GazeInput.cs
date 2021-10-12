using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Tobii.Gaming;

 
public class GazeInput : StandaloneInputModule
{
    private GameObject currentOverGameObject;
    private PointerEventData pointerEventData;
    private float durationTime;

    [SerializeField]
    SceneController sceneController;
 
    protected void HandlePointerExitAndEnter2(PointerEventData currentPointerData, GameObject newEnterTarget)
    {
        // if we have no target / pointerEnter has been deleted 
        // just send exit events to anything we are tracking 
        // then exit 
        if (newEnterTarget == null || currentPointerData.pointerEnter == null)
        {
            for (var i = 0; i < currentPointerData.hovered.Count; ++i)
                ExecuteEvents.Execute(currentPointerData.hovered[i], currentPointerData, ExecuteEvents.pointerExitHandler);
            currentPointerData.hovered.Clear();
            if (newEnterTarget == null)
            {
                currentPointerData.pointerEnter = newEnterTarget;
                return;
            }
        }
        // if we have not changed hover target 
        if (currentPointerData.pointerEnter == newEnterTarget && newEnterTarget)
            return;
        GameObject commonRoot = FindCommonRoot(currentPointerData.pointerEnter, newEnterTarget);
        // and we already an entered object from last time 
        if (currentPointerData.pointerEnter != null)
        {
            // send exit handler call to all elements in the chain 
            // until we reach the new target, or null! 
            Transform t = currentPointerData.pointerEnter.transform;
            while (t != null)
            {
                // if we reach the common root break out! 
                if (commonRoot != null && commonRoot.transform == t)
                    break;
                ExecuteEvents.Execute(t.gameObject, currentPointerData, ExecuteEvents.pointerExitHandler);
                currentPointerData.hovered.Remove(t.gameObject);
                t = t.parent;
            }
        }
        // now issue the enter call up to but not including the common root 
        currentPointerData.pointerEnter = newEnterTarget;
        if (newEnterTarget != null)
        {
            Transform t = newEnterTarget.transform;
            while (t != null && t.gameObject != commonRoot)
            {
                ExecuteEvents.Execute(t.gameObject, currentPointerData, ExecuteEvents.pointerEnterHandler);
                currentPointerData.hovered.Add(t.gameObject);
                t = t.parent;
            }
        }
    }
 
    public void ProcessPoint()
    {
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, results);
 
        // Debug.Log("mousePosition=" + Input.mousePosition + ", count=" + results.Count);
 
        RaycastResult raycastResult = FindFirstRaycast(results);
        HandlePointerExitAndEnter2(pointerEventData, raycastResult.gameObject);
 
        if (raycastResult.gameObject == pointerEventData.pointerEnter && raycastResult.gameObject != null)
        {
            durationTime += Time.deltaTime;
        }
        else
        {
            durationTime = 0;
        }
 
        if (durationTime > 1.5f) //看一个对象两秒触发点击事件，可以配置
        {
            Debug.Log("Send Click event, name=" + raycastResult.gameObject.name);
            ExecuteEvents.Execute(raycastResult.gameObject, pointerEventData, ExecuteEvents.pointerClickHandler);
            durationTime = 0;
        }
    }
    
    public void Update()
    {
        if (pointerEventData == null)
        {
            pointerEventData = new PointerEventData(gameObject.GetComponent<EventSystem>());
        }
        GazePoint gazePoint = TobiiAPI.GetGazePoint();
        if (gazePoint.IsValid)
        {
            pointerEventData.position = gazePoint.Screen;
            ProcessPoint();
        }
    }
 
    public override void Process()
    {
        //do nothing to avoid mouse event cover ours 
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
        Debug.Log("Help");
    }

    public void ShowCreditPanel()
    {
        Debug.Log("Credit");
    }
    
}