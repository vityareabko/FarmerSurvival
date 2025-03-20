using UnityEngine;

public class EraseMesh : MonoBehaviour
{
    public Camera mainCamera; // Камера для расчета луча
    public float eraseRadius = 0.1f; // Радиус стирания (в мировых координатах)

    private Texture2D texture;
    private Color32[] pixels;

    void Start()
    {
        // Получаем материал из MeshRenderer
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer == null || meshRenderer.material == null || meshRenderer.material.mainTexture == null)
        {
            Debug.LogError("❌ Ошибка: На объекте отсутствует MeshRenderer или материал с текстурой.");
            return;
        }

        // Получаем текстуру из материала
        Texture2D originalTexture = (Texture2D)meshRenderer.material.mainTexture;

        // Проверяем, доступна ли текстура для чтения
        if (!originalTexture.isReadable)
        {
            Debug.LogError("❌ Ошибка: Текстура не доступна для чтения! Включи Read/Write в настройках.");
            return;
        }

        // Создаём изменяемую копию текстуры
        texture = new Texture2D(originalTexture.width, originalTexture.height, TextureFormat.RGBA32, false);
        texture.SetPixels32(originalTexture.GetPixels32());
        texture.Apply();

        // Инициализируем массив пикселей
        pixels = texture.GetPixels32();

        // Применяем новую текстуру к материалу
        meshRenderer.material.mainTexture = texture;

        // Добавляем MeshCollider, если его нет
        if (GetComponent<MeshCollider>() == null)
        {
            gameObject.AddComponent<MeshCollider>();
        }
    }

    void Update()
    {
        // Создаем луч из центра камеры
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); // Центр экрана
        RaycastHit hit;

        // Проверяем, попали ли мы в объект с коллайдером
        if (Physics.Raycast(ray, out hit))
        {
            // Проверяем, попали ли мы в текущий объект
            if (hit.collider.gameObject == gameObject)
            {
                // Стираем при нажатии левой кнопки мыши
                if (Input.GetMouseButton(0))
                {
                    Erase(hit);
                }
            }
        }
    }

    void Erase(RaycastHit hit)
    {
        // Получаем UV-координаты точки попадания
        Vector2 pixelUV = hit.textureCoord;

        // Преобразуем UV-координаты в координаты текстуры
        int px = (int)(pixelUV.x * texture.width);
        int py = (int)(pixelUV.y * texture.height);

        int radius = Mathf.RoundToInt(eraseRadius * texture.width / 10f); // Радиус стирания (настроить под ваш размер плоскости)

        // Стираем пиксели вокруг точки попадания
        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                if (x * x + y * y <= radius * radius) // Проверка по кругу
                {
                    int pxX = px + x;
                    int pxY = py + y;

                    if (pxX >= 0 && pxX < texture.width && pxY >= 0 && pxY < texture.height)
                    {
                        pixels[pxY * texture.width + pxX] = new Color32(0, 0, 0, 0); // Стираем пиксель
                    }
                }
            }
        }

        // Обновляем текстуру
        texture.SetPixels32(pixels);
        texture.Apply();
    }

    private void OnDrawGizmos()
    {
        // Рисуем луч из центра камеры
        if (mainCamera != null)
        {
            Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            Gizmos.color = Color.red;
            Gizmos.DrawLine(ray.origin, ray.origin + ray.direction * 5f); // Линия длиной 5 единиц
        }
    }
}