using UnityEngine;

public class Nut : MonoBehaviour
{
    [SerializeField] ParticleSystem _insertionEffect;

    public Color Color { get; private set; }

    public void SetColor(Color color)
    {
        Color = color;
        if (TryGetComponent<Renderer>(out var renderer))
        {
            renderer.material.color = Color;
        }
        var main = _insertionEffect.main;
        main.startColor = Color;
    }
    private void HandleNutInserted(Nut nut, int index, bool isMatch)
    {
        if ( nut == this && isMatch)
        {
            _insertionEffect.Play();
        }
    }
    public bool IsSameColorAs(Nut other)
    {
        return Color == other.Color;
    }

    private void OnEnable()
    {
        Bolt.OnNutInserted += HandleNutInserted;
    }

    private void OnDisable()
    {
        Bolt.OnNutInserted -= HandleNutInserted;
    }
}
