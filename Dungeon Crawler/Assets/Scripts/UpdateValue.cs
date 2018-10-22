using TMPro;
using UnityEngine;

public class UpdateValue : MonoBehaviour {

    private TextMeshProUGUI tmp;

    private void Start() {
        tmp = GetComponent<TextMeshProUGUI>();
        UpdateText(0.0f);
    }
    
    public void UpdateText(float f) {
        tmp.text = f.ToString();
    }
}
