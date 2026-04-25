using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public Item item;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Inventory inventory = other.GetComponent<PlayerController>().inventory;

            inventory.AddItem(item);

            Debug.Log("Подобран: " + item.itemName);

            Destroy(gameObject); // удаляем предмет с карты
        }
    }
}