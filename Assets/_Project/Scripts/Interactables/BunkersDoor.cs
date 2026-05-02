using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BunkerDoor : MonoBehaviour
{
    private Animator bunkerAnimator;
    private bool isPlayerInRange = false;
    private bool isOpen = false;

    void Start()
    {
        bunkerAnimator = GetComponent<Animator>();
        if (bunkerAnimator == null)
        {
            Debug.LogError("На объекте бункер не найден компонент Animator!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            if (!isOpen)
            {
                Debug.Log("Вы рядом с входом. Нажмите E для открытия двери.");
            }
            else
            {
                Debug.Log("Двери открыты. Нажмите Enter для захода в бункер.");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            Debug.Log("Вы отошли от входа.");
        }
    }

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            {
                if (!isOpen)
                {
                    Open();
                }
                else
                {
                    Close();
                }
            }
        }

        if (isPlayerInRange && isOpen && Input.GetKeyDown(KeyCode.Return))
        {
            GoInto();
        }
    }

    private void Open()
    {
        if (bunkerAnimator != null)
        {
            isOpen = true;
            bunkerAnimator.SetBool("IsOpen", true); // Запускаем анимацию в Аниматоре
            Debug.Log("Двери открыты!");
        }
    }

    private void Close()
    {
        if (bunkerAnimator != null)
        {
            isOpen = false;
            bunkerAnimator.SetBool("IsOpen", false); // Запускаем анимацию в Аниматоре
            Debug.Log("Двери закрыты! Нажмите E, чтобы открыть.");
        }
    }

    private void GoInto()
    {
        if (bunkerAnimator != null)
        {
            Debug.Log("Заходим в Бункер!");
            GameManager.Instance.LoadNextScene("Nimbus");
        }
    }
}
