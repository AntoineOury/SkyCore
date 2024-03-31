using UnityEngine;
using Player.View;
using Player.Motion;
using TMPro;
using UnityEngine.UI;

public class IslandHeartInteractBase : InteractableWithUIMode
{
    [Header("Feed-Island Heart interaction UI")]
    [SerializeField, Tooltip("If the Island Heart is leveling up, disable the feedButton")]
    private Button _feedButton;

    private IslandHeartLeveling heart;

    private void Awake()
    {
        heart = gameObject.GetComponent<IslandHeartLeveling>();
    }

    private void Update()
    {
        if (IsInteracting)
        {
            _feedButton.interactable = !heart.IsMaxLevel();
        }
    }
}
