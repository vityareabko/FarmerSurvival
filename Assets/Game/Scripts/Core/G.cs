
using UnityEngine;

public static class G
{
    public static Main main;
    public static RunState run;
}

public class RunState
{
    public int currentLevel;

    public RunState(int level)
    {
        currentLevel = level;
    }
}

public static class Help
{
    /// <summary>
    /// Проверяет Raycast на определенные слои и возвращает результат.
    /// </summary>
    /// <param name="origin">Начальная точка луча.</param>
    /// <param name="direction">Направление луча.</param>
    /// <param name="layerMask">Маска слоев для проверки.</param>
    /// <param name="maxDistance">Максимальная дистанция луча.</param>
    /// <returns>Возвращает RaycastHit, если луч попал в объект на указанных слоях, иначе null.</returns>
    public static RaycastHit? CheckRaycast(Vector3 origin, Vector3 direction, LayerMask layerMask, float maxDistance = Mathf.Infinity)
    {
        RaycastHit hit;
        if (Physics.Raycast(origin, direction, out hit, maxDistance, layerMask))
        {
            return hit;
        }
        return null;
    }

    /// <summary>
    /// Проверяет Raycast из центра камеры на определенные слои и возвращает результат.
    /// </summary>
    /// <param name="camera">Камера, из которой будет исходить луч.</param>
    /// <param name="layerMask">Маска слоев для проверки.</param>
    /// <param name="maxDistance">Максимальная дистанция луча.</param>
    /// <returns>Возвращает RaycastHit, если луч попал в объект на указанных слоях, иначе null.</returns>
    public static RaycastHit? CheckRaycastFromCameraCenter(Camera camera, LayerMask layerMask, float maxDistance = Mathf.Infinity)
    {
        Ray ray = camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, maxDistance, layerMask))
        {
            return hit;
        }
        return null;
    }
}

public static class L
{
    public const string EraseInteract = "EraseInteract";
}