using UnityEngine;

public class MouseManager : MonoBehaviour
{
    [Header("Cursor Settings")]
    [SerializeField] private bool lockCursorInGame = true;

    void Start()
    {
        ApplyCursorSettings();
    }

    public void ApplyCursorSettings()
    {
        if (lockCursorInGame)
        {
            // Confined keeps the mouse inside the game window but lets it move freely
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }
        else
        {
            // Unlocks the mouse completely (great for full UI main menus)
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}