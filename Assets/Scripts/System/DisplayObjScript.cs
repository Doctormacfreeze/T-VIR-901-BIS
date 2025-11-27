using UnityEngine;
using TMPro;

public class DisplayObjScript : MonoBehaviour
{

    public TextMeshProUGUI currentObjectiveText;

    void OnEnable()
    {
        Refresh();
    }

   public void Refresh()
{
    if (currentObjectiveText == null)
        return;

    if (ObjectiveManager.Instance == null)
    {
        currentObjectiveText.text = "no instance";
        return;
    }

    int index = ObjectiveManager.Instance.CurrentObjectiveIndex;
    var list = ObjectiveManager.Instance.ObjectiveRuntimeList;

    if (index < 0 || index >= list.Count)
    {
        currentObjectiveText.text = "No objective";
        return;
    }

    currentObjectiveText.text = list[index].data.description;
}

}
