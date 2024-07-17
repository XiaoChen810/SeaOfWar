using UnityEngine;

public class CursorManager : MonoBehaviour
{
    // 光标纹理
    public Texture2D defaultCursor;
    public Texture2D clickCursor;

    // 光标热点
    public Vector2 defaultHotspot = Vector2.zero;
    public Vector2 customHotspot = Vector2.zero;

    // 单例模式
    public static CursorManager Instance { get; private set; }

    private void Awake()
    {
        // 确保单例
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // 初始化默认光标
        SetCursor(defaultCursor, defaultHotspot);
    }

    private void Update()
    {
        // 按下1键设置为默认光标
        if (Input.GetMouseButtonDown(0))
        {
            ResetCursor();
        }

        // 按下2键设置为自定义光标
        if (Input.GetMouseButtonUp(0))
        {
            SetClickCursor();
        }
    }

    // 设置光标
    public void SetCursor(Texture2D cursorTexture, Vector2 hotspot)
    {
        Cursor.SetCursor(cursorTexture, hotspot, CursorMode.Auto);
    }

    // 重置为默认光标
    public void ResetCursor()
    {
        SetCursor(defaultCursor, defaultHotspot);
    }

    // 设置按下时的光标
    public void SetClickCursor()
    {
        SetCursor(clickCursor, customHotspot);
    }


}
