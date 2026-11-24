using UnityEngine;
using TMPro;

public class KeyBinder : MonoBehaviour
{
    [Header("Key Bindings")]
    public KeyCode moveLeft = KeyCode.LeftArrow;
    public KeyCode moveRight = KeyCode.RightArrow;
    public KeyCode moveUp = KeyCode.UpArrow;
    public KeyCode moveDown = KeyCode.DownArrow;
    public KeyCode attackA = KeyCode.A;
    public KeyCode fireF = KeyCode.F;
    public KeyCode respawnR = KeyCode.R;

    [Header("Button Text Labels (TextMeshPro)")]
    public TMP_Text moveLeftText;
    public TMP_Text moveRightText;
    public TMP_Text moveUpText;
    public TMP_Text moveDownText;
    public TMP_Text attackText;
    public TMP_Text fireText;
    public TMP_Text respawnText;

    private string waitingForKeyAction = "";
    private TMP_Text activeButtonText;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        LoadKeys();
    }

    private void Start()
    {
        UpdateAllButtonTexts();
    }

    // Call this from your UI buttons: pass the action name (e.g., "MoveLeft")
    public void StartRebind(string actionName)
    {
        waitingForKeyAction = actionName;
        activeButtonText = GetTextComponentForAction(actionName);

        if (activeButtonText != null)
        {
            activeButtonText.text = "Press a key...";
        }
    }

    private void OnGUI()
    {
        if (!string.IsNullOrEmpty(waitingForKeyAction))
        {
            Event e = Event.current;
            if (e.isKey && e.type == EventType.KeyDown && e.keyCode != KeyCode.None)
            {
                AssignKey(waitingForKeyAction, e.keyCode);
                waitingForKeyAction = "";
                activeButtonText = null;
                UpdateAllButtonTexts();
            }
        }
    }

    private void AssignKey(string action, KeyCode newKey)
    {
        switch (action)
        {
            case "MoveLeft": moveLeft = newKey; break;
            case "MoveRight": moveRight = newKey; break;
            case "MoveUp": moveUp = newKey; break;
            case "MoveDown": moveDown = newKey; break;
            case "Attack": attackA = newKey; break;
            case "Fire": fireF = newKey; break;
            case "Respawn": respawnR = newKey; break;
        }
        SaveKeys();
        UpdateAllButtonTexts();
    }

    private TMP_Text GetTextComponentForAction(string action)
    {
        switch (action)
        {
            case "MoveLeft": return moveLeftText;
            case "MoveRight": return moveRightText;
            case "MoveUp": return moveUpText;
            case "MoveDown": return moveDownText;
            case "Attack": return attackText;
            case "Fire": return fireText;
            case "Respawn": return respawnText;
            default: return null;
        }
    }

    public void UpdateAllButtonTexts()
    {
        if (moveLeftText != null) moveLeftText.text = moveLeft.ToString();
        if (moveRightText != null) moveRightText.text = moveRight.ToString();
        if (moveUpText != null) moveUpText.text = moveUp.ToString();
        if (moveDownText != null) moveDownText.text = moveDown.ToString();
        if (attackText != null) attackText.text = attackA.ToString();
        if (fireText != null) fireText.text = fireF.ToString();
        if (respawnText != null) respawnText.text = respawnR.ToString();
    }

    public void SaveKeys()
    {
        PlayerPrefs.SetInt("Key_MoveLeft", (int)moveLeft);
        PlayerPrefs.SetInt("Key_MoveRight", (int)moveRight);
        PlayerPrefs.SetInt("Key_MoveUp", (int)moveUp);
        PlayerPrefs.SetInt("Key_MoveDown", (int)moveDown);
        PlayerPrefs.SetInt("Key_Attack", (int)attackA);
        PlayerPrefs.SetInt("Key_Fire", (int)fireF);
        PlayerPrefs.SetInt("Key_Respawn", (int)respawnR);
        PlayerPrefs.Save();
    }

    public void LoadKeys()
    {
        moveLeft = (KeyCode)PlayerPrefs.GetInt("Key_MoveLeft", (int)moveLeft);
        moveRight = (KeyCode)PlayerPrefs.GetInt("Key_MoveRight", (int)moveRight);
        moveUp = (KeyCode)PlayerPrefs.GetInt("Key_MoveUp", (int)moveUp);
        moveDown = (KeyCode)PlayerPrefs.GetInt("Key_MoveDown", (int)moveDown);
        attackA = (KeyCode)PlayerPrefs.GetInt("Key_Attack", (int)attackA);
        fireF = (KeyCode)PlayerPrefs.GetInt("Key_Fire", (int)fireF);
        respawnR = (KeyCode)PlayerPrefs.GetInt("Key_Respawn", (int)respawnR);
    }
}