using UnityEngine;

public class DamageNumber : MonoBehaviour
{
    public static bool Enabled = true;
    public static Font NumberFont;

    private const float duration = 0.6f;
    private TextMesh textMesh;
    private float elapsed;

    public static void Show(int damage, Vector3 position)
    {
        if (!Enabled)
            return;

        GameObject obj = new GameObject("DamageNumber", typeof(TextMesh), typeof(DamageNumber));
        obj.transform.position = position;

        DamageNumber number = obj.GetComponent<DamageNumber>();
        number.textMesh = obj.GetComponent<TextMesh>();
        Font font = NumberFont != null ? NumberFont : Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        number.textMesh.font = font;
        number.textMesh.text = damage.ToString();
        number.textMesh.anchor = TextAnchor.MiddleCenter;
        number.textMesh.alignment = TextAlignment.Center;
        number.textMesh.fontSize = 64;
        number.textMesh.fontStyle = FontStyle.Bold;
        number.textMesh.characterSize = 0.08f;
        number.textMesh.color = Color.white;

        MeshRenderer renderer = obj.GetComponent<MeshRenderer>();
        renderer.sharedMaterial = font.material;
        renderer.sortingLayerName = "Weapon";
        renderer.sortingOrder = 100;
    }

    private void Update()
    {
        elapsed += Time.unscaledDeltaTime;
        if (!Enabled || elapsed >= duration)
        {
            Destroy(gameObject);
            return;
        }

        transform.position += Vector3.up * (1.5f * Time.unscaledDeltaTime);
        textMesh.color = new Color(1f, 1f, 1f, 1f - elapsed / duration);
    }
}
