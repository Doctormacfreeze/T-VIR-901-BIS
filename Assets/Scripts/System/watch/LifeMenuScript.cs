using UnityEngine;
using TMPro;

public class LifeMenuScript : MonoBehaviour
{
    public TextMeshProUGUI lifeText;
    private DisplayObjScript displayObj;

    void Awake(){
        displayObj.Refresh();
    }
    
    void OnEnable(){
        lifeText.text = "ma vie ma vie ma vie";
    }
}
