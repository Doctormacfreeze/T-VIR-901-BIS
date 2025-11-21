using UnityEngine;
using TMPro;

public class LifeMenuScript : MonoBehaviour
{
    public TextMeshProUGUI lifeText;
    
    void OnEnable(){
        lifeText.text = "ma vie ma vie ma vie";
    }
}
