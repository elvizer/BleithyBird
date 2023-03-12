using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StartManager : MonoBehaviour
{
    [SerializeField] private Texture2D hoverCursor;
    [SerializeField] private Texture2D idleCursor;

    private void Start()
    {
        Cursor.SetCursor(idleCursor, new Vector2(idleCursor.width / 2, idleCursor.height / 2), CursorMode.ForceSoftware);
    }

    public void PointerHover() { 
        Cursor.SetCursor(hoverCursor, new Vector2(hoverCursor.width / 2, hoverCursor.height / 2), CursorMode.ForceSoftware);
    }
    public void PointerExit() { 
        Cursor.SetCursor(idleCursor, new Vector2(idleCursor.width / 2, idleCursor.height / 2), CursorMode.ForceSoftware);
    }
}


