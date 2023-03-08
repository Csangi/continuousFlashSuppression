using UnityEngine;

public class ScreenShot : MonoBehaviour
{
    private static ScreenShot instance;

    private Camera myCamera;
    private bool takeScreenshotonNextFrame;

    private void Awake()
    {
        instance = this;
        myCamera = gameObject.GetComponent<Camera>();
    }

    private void OnPostRender()
    {
        if (takeScreenshotonNextFrame)
        {
            takeScreenshotonNextFrame = false;
            RenderTexture renderTexture = myCamera.targetTexture;

            Texture2D renderResult = new Texture2D(renderTexture.width, renderTexture.height);

        }
    }

    private void TakeScreenShot(int width, int height)
    {
        myCamera.targetTexture = RenderTexture.GetTemporary(width, height, 16);
        takeScreenshotonNextFrame = true;
    }
}
