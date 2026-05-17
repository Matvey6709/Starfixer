using UnityEngine;

public class BossFightManager : MonoBehaviour
{
    public static BossFightManager Instance { get; private set; }

    [Header("Ссылки на объекты")]
    public BossAI bossScript;            // Ссылка на твой скрипт босса
    public EnergyGenerator[] generators; // Массив из 4-х генераторов
    public ActivationLever finalLever;   // Ссылка на наш новый рычаг!

    private bool bossIsDead = false;
    private bool phaseTwoActivated = false;
    private bool leverActivated = false; // Флаг, чтобы не активировать рычаг каждый кадр

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void CheckGeneratorsProgress()
    {
        if (bossIsDead || generators == null || generators.Length == 0) return;

        int chargedCount = 0;
        int totalProgressPercentage = 0;

        foreach (var generator in generators)
        {
            if (generator != null)
            {
                int generatorPercent = generator.GetCurrentPhase() * 25;
                totalProgressPercentage += generatorPercent;

                if (generator.IsFullyCharged)
                {
                    chargedCount++;
                }
            }
        }

        Debug.Log($"Общий прогресс: {totalProgressPercentage}% из 400%. Заряжено полностью: {chargedCount}");

        // Активация 2 фазы (ускорение босса)
        if (!phaseTwoActivated && totalProgressPercentage > 300)
        {
            phaseTwoActivated = true;
            if (bossScript != null)
            {
                bossScript.ActivatePhaseTwo();
                Debug.Log("Внимание! Прогресс > 300%! 2-я фаза!");
            }
        }

        // ИЗМЕНЕНИЕ ТУТ: Если все 4 генератора готовы, активируем рычаг вместо убийства босса
        if (!leverActivated && chargedCount >= 4)
        {
            leverActivated = true;
            if (finalLever != null)
            {
                finalLever.EnableLever(); // Включаем рычаг!
            }
            else
            {
                Debug.LogError("Забыли перетащить скрипт Рычага в BossFightManager!");
            }
        }
    }

    // Этот метод теперь PUBLIC. Его вызывает скрипт рычага, когда игрок нажимает E
    public void TriggerBossDeath()
    {
        if (bossIsDead) return;

        bossIsDead = true;
        Debug.Log("Рычаг нажат! Босс повержен!");

        if (generators != null && generators.Length > 0)
        {
            foreach (var generator in generators)
            {
                if (generator != null)
                {
                    generator.ResetAndDisable(); 
                }
            }
        }
        if (bossScript != null)
        {
            bossScript.Die();
        }
    }
}