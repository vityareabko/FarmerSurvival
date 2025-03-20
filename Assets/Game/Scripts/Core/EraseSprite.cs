using UnityEngine;

public class EraseSprite3D : MonoBehaviour
{
    public Camera mainCamera; // Камера для расчета луча
    public SpriteRenderer spriteRenderer; // Спрайт, который будем стирать
    public float eraseRadius = 0.1f; // Радиус стирания (в мировых координатах)

    private Texture2D texture;
    private Color32[] pixels;

    void Start()
    {
        // Проверяем, доступна ли текстура для чтения
        Texture2D originalTexture = spriteRenderer.sprite.texture;
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

        // Применяем новую текстуру к спрайту
        spriteRenderer.sprite = Sprite.Create(texture, spriteRenderer.sprite.rect, new Vector2(0.5f, 0.5f));

        // Добавляем MeshCollider, если его нет
        if (GetComponent<MeshCollider>() == null)
        {
            gameObject.AddComponent<MeshCollider>();
        }

        // Создаем меш для спрайта (если его нет)
        if (GetComponent<MeshFilter>() == null)
        {
            CreateSpriteMesh();
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
            // Проверяем, попали ли мы в текущий спрайт
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

        int radius = Mathf.RoundToInt(eraseRadius * texture.width / spriteRenderer.bounds.size.x);

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

    void CreateSpriteMesh()
    {
        // Создаем меш для спрайта
        Mesh mesh = new Mesh();

        // Получаем размеры спрайта
        float width = spriteRenderer.sprite.rect.width / spriteRenderer.sprite.pixelsPerUnit;
        float height = spriteRenderer.sprite.rect.height / spriteRenderer.sprite.pixelsPerUnit;

        // Вершины меша
        Vector3[] vertices = new Vector3[4]
        {
            new Vector3(-width / 2, -height / 2, 0),
            new Vector3(-width / 2, height / 2, 0),
            new Vector3(width / 2, -height / 2, 0),
            new Vector3(width / 2, height / 2, 0)
        };

        // Треугольники меша
        int[] triangles = new int[6]
        {
            0, 1, 2,
            2, 1, 3
        };

        // UV-координаты
        Vector2[] uv = new Vector2[4]
        {
            new Vector2(0, 0),
            new Vector2(0, 1),
            new Vector2(1, 0),
            new Vector2(1, 1)
        };

        // Применяем данные к мешу
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;

        // Добавляем меш на объект
        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;

        // Обновляем MeshCollider
        MeshCollider meshCollider = GetComponent<MeshCollider>();
        meshCollider.sharedMesh = mesh;
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