using UnityEngine;

public class ComputerOSIRIS : MonoBehaviour
{
    private bool isInComputerZone = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isInComputerZone = true;
            Debug.Log("Вы рядом с OSIRIS. Нажмите E для входа.");
            InteractionHintUI.Show("Нажми E — войти в OSIRIS");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isInComputerZone = false;
        }
    }

    private void Update()
    {
        if (isInComputerZone && Input.GetKeyDown(KeyCode.E))
        {
            ExecuteComputerAction();
        }
    }

    private void ExecuteComputerAction()
    {
        Debug.Log("Вход в систему OSIRIS... функциональность будет добавлена.");
        InteractionHintUI.Show("Подключение к OSIRIS...");
    }
}
