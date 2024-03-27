using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;


/// <summary>
/// This class contains click handlers for the main menu buttons that don't just change which settings page is open.
/// </summary>
public class SettingsMenuButtonsHandler : MonoBehaviour
{
    private EscMenuToggle _escMenuToggle;
    private PauseManagement _pauseManagement;


    private void Awake()
    {
        _escMenuToggle = FindObjectOfType<EscMenuToggle>();
        _pauseManagement = FindObjectOfType<PauseManagement>();
    }

    public void OnSaveClicked()
    {
        throw new NotImplementedException();
    }

    public void OnLoadClicked()
    {
        throw new NotImplementedException();
    }

    public void OnCloseButtonClicked()
    {
        _escMenuToggle.SetActive(false);
        if (_pauseManagement != null)
        {
            _pauseManagement.TogglePause();
        }
    }

    public void OnReturnToMainMenuClicked()
    {
        StartCoroutine(DisplayConfirmationDialogAndWait());
    }


    private IEnumerator DisplayConfirmationDialogAndWait()
    {
        ConfirmationDialog.DisplayConfirmationDialog("Are you sure you want to return to the main menu?");

        while (ConfirmationDialog.IsOpen)
        {
            yield return null;
        }

        if (!ConfirmationDialog.UserChoice)
        {
            // The use clicked No, so do nothing here.
            yield break;
        }
        else
        {
            // The user clicked Yes, so return to the main menu.
            SceneManager.LoadScene("StartMenu");
        }

    }

}
