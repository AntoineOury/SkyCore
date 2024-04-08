using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Player.View;

public class PauseManagement : MonoBehaviour
{
    private PlayerActions _pauseAction;

    private EscMenuToggle _escMenuToggle;

    private static PauseManagement _instance;
    public static PauseManagement Instance
    {
        get
        {
            if (_instance == null)
            {
                // Need to do this if another script's Awake needs it before this
                // one's Awake runs
                _instance = FindObjectOfType<PauseManagement>();
            }
            return _instance;
        }
    }

    public static bool IsPaused => Time.timeScale == 0;

    private void Awake()
    {
        _instance = this;

        _pauseAction = new PlayerActions();
        _pauseAction.UI.Pause.performed += callbackContext => TogglePause();

        _escMenuToggle = FindObjectOfType<EscMenuToggle>(true);

        _escMenuToggle.SetActive(false);
    }

    private void OnEnable()
    {
        _pauseAction.Enable();
    }

    private void OnDisable()
    {
        _pauseAction.Disable();
    }

    private void SetPause(bool pause)
    {
        bool wasPaused = IsPaused;
        if (pause != wasPaused)
        {
            CursorMode.ReasonsForUnlockedCursor.ChangeReason("paused", pause);
            InputIgnoring.ChangeReasonToIgnoreInputsForMovementAndInteractionThings("paused", pause);
            Inventory.Instance.IgnoreInput.ChangeReason("paused", pause);
        }

        Time.timeScale = pause ? 0 : 1;

        _escMenuToggle.SetActive(pause);

    }

    public void TogglePause()
    {
        SetPause(!IsPaused);
    }
}
