using UnityEngine;

public abstract class LobbySettingsManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float currentValue;
    public float maxValue;
    public float minValue;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Handles both increasing and decreasing the settings value.
    /// </summary>
    /// <param name="changeValue">-1 for decreasing, 1 for increasing</param>
    public void OnChangeValue(float changeValue)
    {
        Debug.Log("ITS RUNNINGS");
        currentValue += changeValue;
        if (currentValue < 0)
        {
            currentValue = 0;
        }
        else if (currentValue > maxValue)
        {
            currentValue = 0;
        }
    }
}
