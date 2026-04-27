using UnityEngine;
using TMPro;

public class ChestMetalUI : MonoBehaviour
{
    public GameObject iconObject;
    public TextMeshProUGUI countText;

    void Update()
    {
        if (DataManager.Instance == null)
        {
            Debug.LogError("❌ DataManager = NULL");
            return;
        }

        if (DataManager.Instance.gameData == null)
        {
            Debug.LogError("gameData = NULL");
            return;
        }

        if (DataManager.Instance.gameData.chestInventory == null)
        {
            Debug.LogError("chestInventory = NULL");
            return;
        }

        if (countText == null)
        {
            Debug.LogError(" countText НЕ привязан");
            return;
        }

        if (iconObject == null)
        {
            Debug.LogError(" iconObject НЕ привязан");
            return;
        }

        var inventory = DataManager.Instance.gameData.chestInventory;

        int metalAmount = 0;

        foreach (var item in inventory)
        {
            if (item.id == "metal")
            {
                metalAmount = item.amount;
                break;
            }
        }

        countText.text = metalAmount.ToString();
        iconObject.SetActive(true);
    }
}

    
    
    
