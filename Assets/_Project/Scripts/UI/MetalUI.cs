using UnityEngine;
using TMPro;

public class MetalUI : MonoBehaviour
{
    public GameObject iconObject;
    public TextMeshProUGUI countText;

    void Update()
    {
        var inventory = DataManager.Instance.gameData.inventory;

        int metalAmount = 0;

        // ищем металл в списке
        foreach (var item in inventory)
        {
            if (item.id == "metal")
            {
                metalAmount = item.amount;
                break;
            }
        }

        // обновляем UI
        countText.text = metalAmount.ToString();

        // скрываем если 0
        iconObject.SetActive(metalAmount > 0);
    }
}