using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    [Header("Настройки перехода")]
    [Tooltip("Точное имя сцены для загрузки (например, 'Nimbus' или 'Maze')")]
    public string sceneToLoad;

    [Tooltip("Кнопка для перехода (по умолчанию Enter)")]
    public KeyCode enterKey = KeyCode.Return;

    [Header("Настройки открытия (для люков и дверей)")]
    [Tooltip("Нужно ли сначала открыть проход отдельной кнопкой?")]
    public bool requiresOpening = false;

    [Tooltip("Кнопка для открытия (по умолчанию E)")]
    public KeyCode openKey = KeyCode.E;

    [Tooltip("Перетащи сюда Animator, если дверь должна анимироваться")]
    public Animator targetAnimator;

    [Tooltip("Имя параметра в Animator (у тебя было 'IsOpen')")]
    public string animatorBoolName = "IsOpen";

    [Header("Сообщения (для Debug/UI)")]
    public string welcomeMessage = "Нажмите Enter для перехода.";
    public string needOpenMessage = "Нажмите E для открытия люка.";
    public string exitMessage = "Вы отошли от входа.";

    private bool isPlayerInRange = false;
    private bool isOpen = false;

    private void Start()
    {
        // Если проход не требует открытия, считаем, что он изначально "открыт"
        if (!requiresOpening)
        {
            isOpen = true;
        }
        else if (targetAnimator == null)
        {
            // Пытаемся найти аниматор на этом же объекте, если забыли перетащить руками
            targetAnimator = GetComponent<Animator>();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;

            if (requiresOpening && !isOpen)
            {
                Debug.Log(needOpenMessage);
                InteractionHintUI.Show(needOpenMessage);
            }
            else
            {
                Debug.Log(welcomeMessage);
                InteractionHintUI.Show(welcomeMessage);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            Debug.Log(exitMessage);
        }
    }

    private void Update()
    {
        if (!isPlayerInRange) return;

        // 1. Логика открытия/закрытия (если включена галочка Requires Opening)
        if (requiresOpening && Input.GetKeyDown(openKey))
        {
            ToggleDoor();
        }

        // 2. Логика перехода на другую сцену
        if (isOpen && Input.GetKeyDown(enterKey))
        {
            PerformTransition();
        }
    }

    private void ToggleDoor()
    {
        isOpen = !isOpen; // Переключаем состояние (открыто/закрыто)

        // Запускаем анимацию, если аниматор назначен
        if (targetAnimator != null)
        {
            targetAnimator.SetBool(animatorBoolName, isOpen);
        }

        if (isOpen)
        {
            Debug.Log("Проход открыт! " + welcomeMessage);
            InteractionHintUI.Show("Проход открыт. " + welcomeMessage);
        }
        else
        {
            Debug.Log("Проход закрыт! " + needOpenMessage);
            InteractionHintUI.Show("Проход закрыт. " + needOpenMessage);
        }
    }

    private void PerformTransition()
    {
        Debug.Log($"Перемещаемся на сцену: {sceneToLoad}!");
        SoundManager.PlayTeleport();

        // Безопасный вызов перехода: используем твой GameManager, а если его нет на сцене — обычный SceneManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.LoadNextScene(sceneToLoad);
        }
        else
        {
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}