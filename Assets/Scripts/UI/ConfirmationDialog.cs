using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class ConfirmationDialog : MonoBehaviour
{
    public static ConfirmationDialog Instance {get; private set;}

    public static bool IsOpen {get; private set;}

    /// <summary>
    /// Returns true if the user clicked yes, or false otherwise.
    /// </summary>
    public static bool UserChoice { get; private set; }



    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI _messageText;
    [SerializeField] private Button _yesButton;
    [SerializeField] private Button _noButton;


    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("A ConfirmationDialog already exists! Self destructing.");
            Destroy(gameObject);
            return;
        }


        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (!IsOpen)
        {
            // Disable the dialog.
            gameObject.gameObject.SetActive(false);
        }
    }

    public static void DisplayConfirmationDialog(string message, string yesButtonText = "Yes", string noButtonText = "No")
    {
        if (Instance == null)
        {
            Debug.LogError("Cannot display the ConfirmationDialog, as no instance of it is present in the scene!");
            return;
        }


        Instance.Show(message, yesButtonText, noButtonText);
    }

    public void Show(string message, string yesButtonText = "Yes", string noButtonText = "No")
    {
        _yesButton.GetComponentInChildren<TextMeshProUGUI>().text = yesButtonText;
        _noButton.GetComponentInChildren<TextMeshProUGUI>().text = noButtonText;
        _messageText.text = message;

        gameObject.gameObject.SetActive(true);
        IsOpen = true;
    }

    public void OnYesButtonClicked()
    {
        UserChoice = true;

        Close();
    }

    public void OnNoButtonClicked()
    {
        UserChoice = false;

        Close();
    }


    private void Close()
    {
        // Close the dialog.
        gameObject.gameObject.SetActive(false);
        IsOpen = false;
    }
}
