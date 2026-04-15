using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TeleportToShip : MonoBehaviour
{
    private bool isPlayerInRange = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            Debug.Log("Вы рядом с телепортом. Нажмите Enter для перехода на корабль.");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            Debug.Log("Вы отошли от телепорта.");
        }
    }

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.Return))
        {
            GoHome();
        }
    }

    private void GoHome()
    {
        Debug.Log("Перемещаемся на корабль!");
        SceneManager.LoadScene("SpaceShip");
    }
}
