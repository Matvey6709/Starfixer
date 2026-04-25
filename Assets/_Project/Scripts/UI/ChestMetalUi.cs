using UnityEngine;
using TMPro;

public class ChestMetalUI : MonoBehaviour
{
    public GameObject iconObject;
    public TextMeshProUGUI countText;

    void Update()
    {
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
        iconObject.SetActive(true);;
    }
}