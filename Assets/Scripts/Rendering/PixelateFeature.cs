using UnityEngine;
using UnityEngine.UI;


public class PixelateFeature : MonoBehaviour 
{
    public enum PixelScreenMode { Resize, Scale}

    [System.Serializable]
    public struct ScreenSize
    {
        public int width;
        public int height;
    }

    [Header("Screen scaling settings")] 
    public PixelScreenMode mode;
    public ScreenSize targetScreenSize = new ScreenSize { width = 256, height = 144 };
    public uint screenScaleFactor = 1;

    private Camera renderCamera;
    private RenderTexture renderTexture;
    private int screenWidth, screenHeight;

    [Header("Display")] 
    public RawImage display;


    public void Init()
    {
        if (!renderCamera) 
            renderCamera = GetComponent<Camera>();
        screenWidth = Screen.width;
        screenHeight = Screen.height;
        targetScreenSize.width = Screen.width;
        targetScreenSize.height = Screen.height;
        
        if (screenScaleFactor < 1)
            screenScaleFactor = 1;
        if (targetScreenSize.width < 1)
            targetScreenSize.width = 1;
        if (targetScreenSize.height < 1)
            targetScreenSize.height = 1;

        int width = mode == PixelScreenMode.Resize ? (int)targetScreenSize.width : screenWidth / (int)screenScaleFactor;
        int height = mode == PixelScreenMode.Resize ? (int)targetScreenSize.height : screenHeight / (int)screenScaleFactor;

        renderTexture = new RenderTexture(width, height, 24)
        {
            filterMode = FilterMode.Point,
            antiAliasing = 1
        };

        renderCamera.targetTexture = renderTexture;
        display.texture = renderTexture;
    }
    
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        if (CheckScreenResize())
            Init();
    }

    public bool CheckScreenResize()
    {
        return Screen.width != screenWidth || Screen.height != screenHeight;
    }
}
