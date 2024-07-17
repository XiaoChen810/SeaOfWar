using UnityEngine;

public class CursorManager : MonoBehaviour
{
    // �������
    public Texture2D defaultCursor;
    public Texture2D clickCursor;

    // ����ȵ�
    public Vector2 defaultHotspot = Vector2.zero;
    public Vector2 customHotspot = Vector2.zero;

    // ����ģʽ
    public static CursorManager Instance { get; private set; }

    private void Awake()
    {
        // ȷ������
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // ��ʼ��Ĭ�Ϲ��
        SetCursor(defaultCursor, defaultHotspot);
    }

    private void Update()
    {
        // ����1������ΪĬ�Ϲ��
        if (Input.GetMouseButtonDown(0))
        {
            ResetCursor();
        }

        // ����2������Ϊ�Զ�����
        if (Input.GetMouseButtonUp(0))
        {
            SetClickCursor();
        }
    }

    // ���ù��
    public void SetCursor(Texture2D cursorTexture, Vector2 hotspot)
    {
        Cursor.SetCursor(cursorTexture, hotspot, CursorMode.Auto);
    }

    // ����ΪĬ�Ϲ��
    public void ResetCursor()
    {
        SetCursor(defaultCursor, defaultHotspot);
    }

    // ���ð���ʱ�Ĺ��
    public void SetClickCursor()
    {
        SetCursor(clickCursor, customHotspot);
    }


}
