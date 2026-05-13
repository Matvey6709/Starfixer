using UnityEngine;
using UnityEngine.SceneManagement;

public class FakeMazeEnter : MonoBehaviour
{
    private bool isPlayerInRange = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("В триггер вошел объект: " + other.name);
        if (other.CompareTag("Player"))
        {
            var em = GameObject.FindWithTag("EnterMaze");
            if (em != null)
            {
                other.transform.position = em.transform.position ;
                Debug.Log("Ловушка сработала!");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            Debug.Log("Зайди там микросхема");
        }
    }
}
