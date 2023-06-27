using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransparentBackgroundCamera : MonoBehaviour
{
    void OnPreRender()
    {
        TransparentBackground.ClearCanvas();
    }
}
