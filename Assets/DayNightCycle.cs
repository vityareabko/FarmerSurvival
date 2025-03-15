using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    public Light directionalLight; // Ссылка на Directional Light в сцене
    public float dayDuration = 120f; // Длительность полного цикла день/ночь в секундах
    public float maxIntensity = 1f; // Максимальная интенсивность света (день)
    public float minIntensity = 0f; // Минимальная интенсивность света (ночь)
    public float rotationSpeedX = 360f; // Скорость вращения по оси X за цикл (полный круг)
    public float rotationSpeedY = 15f; // Небольшое вращение по оси Y для эффекта

    private float timeOfDay; // Текущее время в цикле (0 - начало, 1 - конец)

    void Start()
    {
        if (directionalLight == null)
        {
            // Автоматически находим Directional Light, если не задан
            directionalLight = FindObjectOfType<Light>();
            if (directionalLight == null || directionalLight.type != LightType.Directional)
            {
                Debug.LogError("Не найден Directional Light в сцене!");
                return;
            }
        }

        // Инициализируем время дня
        timeOfDay = 0f;
    }

    void Update()
    {
        // Обновляем время дня
        timeOfDay += Time.deltaTime / dayDuration;
        if (timeOfDay >= 1f) timeOfDay = 0f; // Зацикливаем

        // Вычисляем угол вращения по оси X (симулируем движение солнца)
        float sunAngleX = timeOfDay * rotationSpeedX; // От 0 до 360 градусов
        float sunAngleY = Mathf.Sin(timeOfDay * Mathf.PI * 2) * rotationSpeedY; // Небольшое колебание по Y

        // Применяем вращение к Directional Light
        directionalLight.transform.rotation = Quaternion.Euler(sunAngleX, sunAngleY, 0f);

        // Вычисляем интенсивность света
        float intensity = CalculateIntensity(timeOfDay);
        directionalLight.intensity = Mathf.Lerp(directionalLight.intensity, intensity, Time.deltaTime * 5f); // Плавный переход
    }

    float CalculateIntensity(float time)
    {
        // Используем синус для плавного перехода день/ночь
        float t = Mathf.Sin(time * Mathf.PI * 2); // От -1 до 1
        float normalizedIntensity = (t + 1f) / 2f; // Приводим к диапазону 0..1
        return Mathf.Lerp(minIntensity, maxIntensity, normalizedIntensity); // Интерполяция между min и max
    }

    // Для отладки: позволяет вручную установить время дня через Inspector
    [Range(0f, 1f)]
    public float debugTimeOfDay;
    void OnValidate()
    {
        timeOfDay = debugTimeOfDay;
    }
}