using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SingleModePanel : PanelBase
{
  
    public override void OnEsc()
    {
        Application.Quit();
    }
}
