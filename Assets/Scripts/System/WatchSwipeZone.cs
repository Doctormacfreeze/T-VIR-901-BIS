using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class WatchSwipeZone : MonoBehaviour
{
    
    public GameObject[] displays;
    private int currentIndex = 0;
    private Vector3 startTouchPos;
    private bool isTouching = false;
    private XRBaseInteractor currentInteractor;


    public void OnPokeEnter(XRBaseInteractor interactor)
    {
        currentInteractor = interactor;
        startTouchPos = interactor.transform.position;
        isTouching = true;
    }

    public void Update()
    {
        if(!isTouching || currentInteractor == null) return;

        Vector3 delta = currentInteractor.transform.position - startTouchPos;
        if(Mathf.Abs(delta.x) > 0.03f)
        {
            if(delta.x > 0){
                currentIndex = Mathf.Max(0, currentIndex - 1);
            }else{
                currentIndex = Mathf.Min(displays.Length  - 1, currentIndex + 1);
            }

            UpdateDisplay();
            isTouching = false;
        }
    }

    public void OnPokeExit(XRBaseInteractor interactor)
    {
        currentInteractor = null;
        isTouching = false;
    }

    private void UpdateDisplay(){
        for(int i = 0; i < displays.Length; i++)
            displays[i].SetActive(i == currentIndex);
    }


}