using UnityEditor;

[InitializeOnLoad]
public class PreBuildStep 
{
    static PreBuildStep()
    {
        //WEBGL BUILDS DONT HAVE SUPPORT FOR MULTITHREADING IN UNITY 2021 :(
        //PlayerSettings.WebGL.linkerTarget = WebGLLinkerTarget.Wasm;
        PlayerSettings.WebGL.emscriptenArgs = "";
        PlayerSettings.WebGL.threadsSupport = false;
        PlayerSettings.WebGL.memorySize = 2032;
    }
}