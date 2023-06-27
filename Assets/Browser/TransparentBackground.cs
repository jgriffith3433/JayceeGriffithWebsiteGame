using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class TransparentBackground : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

#if UNITY_EDITOR
    public static void ClearCanvas()
    {
        GL.Clear(true, true, new Color(255, 255, 255), 1);
    }
#else
    [DllImport("__Internal")]
    public static extern void ClearCanvas();
#endif
}
