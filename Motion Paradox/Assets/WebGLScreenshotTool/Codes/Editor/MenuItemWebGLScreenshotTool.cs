#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

namespace WebGLScreenshotTool
{
    public class MenuItemWebGLScreenshotTool : MonoBehaviour
    {
        [MenuItem("GameObject/WebGLScreenshot/WebGLScreenshotTool", false, 10)]
        private static void CreateWebGLScreenshotManager()
        {
            GameObject gameObject = new GameObject();
            gameObject.name = "WebGLScreenshotTool";
            gameObject.AddComponent<WebGLScreenshotTool>();
        }
    }
}

#endif