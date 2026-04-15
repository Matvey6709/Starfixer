using UnityEngine;

public class OxygenKapsula : MonoBehaviour
{
    private Animator hatchAnimator;
    private bool isPlayerInRange = false; 
    private bool isFull = true; 

    void Start()
    {
        hatchAnimator = GetComponent<Animator>();
        if (hatchAnimator == null)
        {
            Debug.LogError("На объекте капсула не найден компонент Animator!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            if (isFull)
            {
                Debug.Log("Вы рядом с капсулой. Нажмите E для пополнения запаса кислорода.");
            }
            else
            {
                Debug.Log("Запас кислорода полон, возвращайтесь после вылазки.");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            Debug.Log("Вы отошли от капсулы.");
        }
    }

    void Update()
    {
        if (isPlayerInRange && isFull && Input.GetKeyDown(KeyCode.E))
        {
            Open();
        }
    }

    private void Open()
    {
        if (hatchAnimator != null)
        {
            isFull = false;
            hatchAnimator.SetBool("IsFull", false); // Запускаем анимацию в Аниматоре
            Debug.Log("Запас кислорода пополнен!");
        }
    }
}