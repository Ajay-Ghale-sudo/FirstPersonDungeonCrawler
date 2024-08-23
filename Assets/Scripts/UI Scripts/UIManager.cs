using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("UI Config options.")]
    public Texture2D cursorTexture;
    public Vector2 hotSpot = Vector2.zero;
    public CursorMode cursorMode = CursorMode.Auto;
    private Image crosshair;
    // Start is called before the first frame update
    void Awake()
    {
        Cursor.SetCursor(null, hotSpot, cursorMode);
        Cursor.visible = false;
        crosshair = GetComponentInChildren<Image>();
        cursorTexture = (Texture2D)crosshair.mainTexture;
        

    }

    public void EnableCursorAndTrack(){ }
    public void DisableCursorAndStopTrack(){ }

}
