using UnityEngine;
using UnityEngine.SceneManagement;

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
        if (LocalQuestManager.Instance != null)
        {
            if (LocalQuestManager.Instance.AreAllQuestsCompleted())
            {
                Debug.Log("Все системы восстановлены! Запуск протокола взлета...");
                InteractionHintUI.Show("Системы в норме. Запуск двигателей...");
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.LoadNextScene("AutroScene");
                }
                else
                {
                    SceneManager.LoadScene("AutroScene");
                }
            }
            else
            {
                Debug.Log("Попытка взлета отклонена: детали не собраны.");
                InteractionHintUI.Show("<color=red>Ошибка: Соберите все детали для ремонта!</color>");
            }
        }
        else
        {
            Debug.LogError("LocalQuestManager не найден на сцене!");
        }
    }
}