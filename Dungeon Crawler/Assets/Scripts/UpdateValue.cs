using TMPro;
using UnityEngine;

public class UpdateValue : MonoBehaviour {

    private TextMeshProUGUI tmp;

    private void Start() {
        tmp = GetComponent<TextMeshProUGUI>();
        //Default value, might be better to make a reference to the slider/button and set the default.
        UpdateText(0.0f);
    }
    
    public void UpdateText(float f) {
        tmp.text = f.ToString();
    }
}
