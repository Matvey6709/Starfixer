using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HatchWay : MonoBehaviour
{
    private Animator hatchAnimator;
    private bool isPlayerInRange = false;
    private bool isOpen = false;

    void Start()
    {
        hatchAnimator = GetComponent<Animator>();
        if (hatchAnimator == null)
        {
            Debug.LogError("На объекте люк не найден компонент Animator!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            if (!isOpen)
            {
                Debug.Log("Вы рядом с люком. Нажмите E для открытия люка.");
            }
            else
            {
                Debug.Log("Люк открыт. Нажмите Enter для выхода на планету.");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            Debug.Log("Вы отошли от люка.");
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
            GoOut();
        }
    }

    private void Open()
    {
        if (hatchAnimator != null)
        {
            isOpen = true;
            hatchAnimator.SetBool("IsOpen", true); // Запускаем анимацию в Аниматоре
            Debug.Log("Люк открыт!");
        }
    }

    private void Close()
    {
        if (hatchAnimator != null)
        {
            isOpen = false;
            hatchAnimator.SetBool("IsOpen", false); // Запускаем анимацию в Аниматоре
            Debug.Log("Люк закрыт! Нажмите E, чтобы открыть.");
        }
    }

    private void GoOut()
    {
        if (hatchAnimator != null)
        {
            Debug.Log("Перемещаемся на Nimbus!");
            SceneManager.LoadScene("Nimbus");
        }
    }
}
