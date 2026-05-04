using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    private GameConfig config;
    public Slider progressBar;

    // Для плавной анимации
    private float currentVisualValue;
    private float targetValue;
    private float velocity;

    // Конфигурация
    private float currentNegative;
    private float fillSpeed;
    private float maxNegative;
    private int segments;

    // Сегментированное представление
    private int currentSegments;
    private float partialFill;

    [Header("Динамическая физика анимации")]
    [SerializeField] private float acceleration; // Ускорение в начале сегмента
    [SerializeField] private float deceleration; // Замедление в конце сегмента
    [SerializeField] private float maxSpeed; // Максимальная скорость
    [SerializeField] private float segmentSwitchBoost; // Буст при переходе на новый сегмент

    // Физические параметры
    private float currentAcceleration = 0f;
    private bool isAccelerating = true;
    private float currentSegmentStartValue = 0f;

    // Скорость регенерации
    private float baseFillSpeed;
    private float regenMultiplier = 1f;

    void Awake()
    {
        // Получаем настройки из конфигурации
        config = ConfigManager.Config;

        float initialNegative = config.initialNegative;
        maxNegative = config.maxNegative;
        float fillDuration = config.fillDuration;
        segments = Mathf.FloorToInt(maxNegative);

        acceleration = config.progressbarAcceleration;
        deceleration = config.progressbarDeceleration;
        maxSpeed = config.progressbarMaxSpeed;
        segmentSwitchBoost = config.progressbarSegmentSwitchBoost;

        // Рассчитываем скорость заполнения
        baseFillSpeed = maxNegative / fillDuration;

        // Инициализируем
        currentNegative = initialNegative;
        currentSegments = Mathf.FloorToInt(currentNegative);
        partialFill = currentNegative - currentSegments;

        targetValue = currentSegments;
        currentVisualValue = targetValue;
        currentSegmentStartValue = currentSegments;

        // Настраиваем слайдер
        progressBar.maxValue = segments;
        progressBar.wholeNumbers = false;
        progressBar.value = currentVisualValue + partialFill;
    }

    void Update()
    {
        // Автоматическое восполнение
        if (currentNegative < maxNegative)
        {
            float previousNegative = currentNegative;
            float effectiveFillSpeed = baseFillSpeed * regenMultiplier;
            currentNegative += Time.deltaTime * effectiveFillSpeed;
            currentNegative = Mathf.Clamp(currentNegative, 0, maxNegative);

            int oldSegments = Mathf.FloorToInt(previousNegative);
            currentSegments = Mathf.FloorToInt(currentNegative);
            partialFill = currentNegative - currentSegments;

            // Если перешли на новый сегмент
            if (currentSegments > oldSegments)
            {
                OnNewSegmentStarted(currentSegments);
                OnSegmentFilled();
            }

            targetValue = currentSegments + partialFill;
        }

        // Физически-правдоподобная анимация
        HandlePhysicsBasedAnimation();
    }

    void HandlePhysicsBasedAnimation()
    {
        if (!Mathf.Approximately(currentVisualValue, targetValue))
        {
            // Вычисляем прогресс внутри сегмента
            float segmentProgress = (currentVisualValue - currentSegmentStartValue);

            // Определяем режим движения
            if (segmentProgress < 0.5f)
            {
                // Первая половина сегмента: ускоряемся
                isAccelerating = true;
                currentAcceleration = acceleration;
            }
            else
            {
                // Вторая половина сегмента: замедляемся
                isAccelerating = false;

                // Рассчитываем необходимое замедление для точной остановки
                float distanceToTarget = Mathf.Abs(targetValue - currentVisualValue);
                float requiredDeceleration = (velocity * velocity) / (2f * distanceToTarget);
                currentAcceleration = -Mathf.Min(deceleration, requiredDeceleration);
            }

            // Применяем ускорение/замедление
            if (isAccelerating)
            {
                velocity += currentAcceleration * Time.deltaTime;
                velocity = Mathf.Min(velocity, maxSpeed);
            }
            else
            {
                velocity += currentAcceleration * Time.deltaTime;
                velocity = Mathf.Max(velocity, 0);
            }

            // Обновляем позицию
            currentVisualValue += velocity * Time.deltaTime;

            // Проверяем, не превысили ли целевую позицию
            if ((velocity > 0 && currentVisualValue > targetValue) ||
                (velocity < 0 && currentVisualValue < targetValue))
            {
                currentVisualValue = targetValue;
                velocity = 0f;
            }

            // Обновляем слайдер
            progressBar.value = currentVisualValue;
        }
    }

    void OnNewSegmentStarted(int newSegment)
    {
        // Буст скорости при переходе на новый сегмент
        velocity *= segmentSwitchBoost;
        velocity = Mathf.Min(velocity, maxSpeed * 1.5f);

        // Обновляем начальное значение сегмента
        currentSegmentStartValue = newSegment;

        // Сбрасываем ускорение
        currentAcceleration = acceleration;
        isAccelerating = true;
    }

    private void OnSegmentFilled()
    {
        // Вызывается когда заполняется новый целый сегмент
        Debug.Log($"Segment filled! Now at {currentSegments}/{segments}");
    }

    // Методы для управления с целыми числами
    public bool UseNegative(int amount)
    {
        if (currentSegments >= amount && amount > 0)
        {
            currentSegments -= amount;
            currentNegative = currentSegments + partialFill;

            // Мгновенное обновление
            targetValue = currentNegative;
            currentVisualValue = targetValue;
            progressBar.value = currentVisualValue;

            // Сбрасываем все анимационные параметры
            velocity = 0f;
            currentSegmentStartValue = currentSegments;
            isAccelerating = true;
            return true;
        }
        else
        {
            return false;
        }
    }

    public void AddNegative(int amount)
    {
        if (amount > 0)
        {
            int oldSegments = currentSegments;
            currentSegments = Mathf.Min(currentSegments + amount, segments);
            currentNegative = Mathf.Min(currentSegments + partialFill, maxNegative);
            targetValue = currentNegative;

            // Если добавили сегменты, применяем эффект перехода
            if (currentSegments > oldSegments)
            {
                OnNewSegmentStarted(currentSegments);
            }
        }
    }

    // Проверка наличия достаточного количества ресурса
    public bool HasEnoughNegative(int amount)
    {
        return currentSegments >= amount && amount > 0;
    }

    // Получение текущего количества ресурса
    public int GetCurrentResource()
    {
        return currentSegments;
    }

    // Свойства
    public int CurrentSegments => currentSegments;
    public int MaxSegments => segments;
    public float CurrentPartialFill => partialFill;
    public float CurrentVelocity => velocity;
    public float CurrentAcceleration => currentAcceleration;

    public void SetRegenMultiplier(float multiplier)
    {
        regenMultiplier = Mathf.Max(0f, multiplier);
    }

    public float GetRegenMultiplier()
    {
        return regenMultiplier;
    }
}