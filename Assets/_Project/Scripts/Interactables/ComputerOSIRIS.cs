using UnityEngine;

public class ComputerOSIRIS : MonoBehaviour
{
    private bool isInComputerZone = false;

    void Start()
    {

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isInComputerZone = true;
            Debug.Log("Подойдите ближе к ОСИРИСУ. Нажмите E для входа.");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isInComputerZone = false;
        }
    }

    // ЛОГИКА ВЗАИМОДЕЙСТВИЯ (Ловим нажатие 'E')
    void Update()
    {
        HandleInteractions();
    }

    private void HandleInteractions()
    {
        if (isInComputerZone && Input.GetKeyDown(KeyCode.E))
        {
            ExecuteComputerAction();
        }
    }

    void ExecuteComputerAction()
    {
        Debug.Log("Вход в систему ОСИРИС... Авторизация инженера Эрика подтверждена.");
    }
}
