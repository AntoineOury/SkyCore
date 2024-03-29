using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CursorMode
{
    public static Reasons ReasonsForUnlockedCursor { get; private set; } = new Reasons((anyReasons) => SetCursorMode(anyReasons));

    public static void Initialize()
    {
        SetCursorMode(ReasonsForUnlockedCursor.AnyReasons);
    }

    private static void SetCursorMode(bool anyReasonsForFreeCursor)
    {
        Cursor.visible = anyReasonsForFreeCursor;
        Cursor.lockState = anyReasonsForFreeCursor ? CursorLockMode.None : CursorLockMode.Locked;
    }
}
