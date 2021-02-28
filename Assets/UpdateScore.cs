using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateScore : MonoBehaviour
{
    TMPro.TextMeshProUGUI txt;

    private void Start()
    {
        txt = GetComponent<TMPro.TextMeshProUGUI>();
    }

    private void Update()
    {
        if (txt.text != "Test #" + MecanicsManager.instance.mapManager.difficulty)
            txt.text = "Test #" + MecanicsManager.instance.mapManager.difficulty;
    }
}
