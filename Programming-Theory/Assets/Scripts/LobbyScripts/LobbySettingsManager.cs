using PurrNet;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class LobbySettingsManager : NetworkBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float currentValue;
    public float maxValue;
    public float minValue;
    public float changeRate;


    public TMP_Text valueText;
    public Button decreaseButton; 
    public Button increaseButton;

    protected override void OnSpawned(bool asServer)
    {
        base.OnSpawned(asServer);

        // Only the server can interact with the buttons
        if (decreaseButton != null)
        {
            decreaseButton.interactable = isServer;
        }

        if (increaseButton != null)
        {
            increaseButton.interactable = isServer;
        }

        UpdateValueDisplay();
    }

    /// <summary>
    /// Handles both increasing and decreasing the settings value.
    /// </summary>
    /// <param name="changeValue">-1 for decreasing, 1 for increasing</param>
    public void OnChangeValue(float changeDirection)
    {
        
        if (!isServer) return;
        //Debug.Log("ITS RUNNINGS");
        currentValue += changeRate * changeDirection;
        if (currentValue < minValue)
        {
            currentValue = maxValue;
        }
        else if (currentValue > maxValue)
        {
            currentValue = minValue;
        }
        BroadcastValueServerRpc(currentValue);


    }

    [ServerRpc(requireOwnership: false)]
    private void BroadcastValueServerRpc(float newValue)
    {
        UpdateValueObserversRpc(newValue);
    }

    [ObserversRpc]
    private void UpdateValueObserversRpc(float newValue)
    {
        Debug.Log($"[{GetType().Name}] UpdateValueObserversRpc received — newValue:{newValue}");
        currentValue = newValue;
        UpdateValueDisplay();
    }


    private void UpdateValueDisplay()
    {
        if (valueText != null)
        {
            valueText.text = this.ToString();
        }
    }
}
