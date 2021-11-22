using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSelect : MonoBehaviour
{
    public Button primaryButton;

    void Start()
    {
        primaryButton.Select();
    }

    private void OnEnable()
    {
        primaryButton.Select(); //workaround with multiple menus being toggled
    }
}
