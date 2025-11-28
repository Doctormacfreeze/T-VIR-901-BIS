using UnityEngine;
using TMPro;

public class DisplayObjScript : MonoBehaviour
{

    public TextMeshProUGUI currentObjectiveText;

    void OnEnable()
    {
        Refresh();
    }

   public void Refresh() { 
       if (ObjectiveManager.Instance == null) 
       { 
           currentObjectiveText.text = "no instance"; 
           return; 
       
       } 
       
       int index = ObjectiveManager.Instance.CurrentObjectiveIndex; 
       
       if (index < 0 || index >= ObjectiveManager.Instance.ObjectiveRuntimeList.Count) 
       { 
           ObjectiveManager.Instance.CurrentObjectiveIndex ++; 
           currentObjectiveText.text = index.ToString(); 
           return; 
       }
       
       currentObjectiveText.text = ObjectiveManager.Instance.ObjectiveRuntimeList[index].data.description; 
   }

}
