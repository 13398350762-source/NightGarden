using UnityEngine;

public class ChiliFlame : MonoBehaviour
{
    private ParticleSystem[] particles;
    private Vector3[] positions;
    private Vector3[] shapes;

    public void Init(float range, float width)
    {
        if (particles == null)
        {
            particles = GetComponentsInChildren<ParticleSystem>(true);
            positions = new Vector3[particles.Length];
            shapes = new Vector3[particles.Length];
            for (int i = 0; i < particles.Length; i++)
            {
                positions[i] = particles[i].transform.localPosition;
                shapes[i] = particles[i].shape.scale;
            }
        }
        transform.localScale = Vector3.one;
        for (int i = 0; i < particles.Length; i++)
        {
            ParticleSystem particle = particles[i];
            particle.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);
            particle.transform.localPosition = new Vector3(positions[i].x * range, positions[i].y * width, 0f);
            var shape = particle.shape;
            shape.scale = new Vector3(shapes[i].x * range, shapes[i].y * width, 0f);
            var main = particle.main;
            float size = Mathf.Min(width * 0.6f, range * 0.08f);
            main.startSize = new ParticleSystem.MinMaxCurve(size * 0.5f, size);
            particle.Play(false);
        }
    }

    public void Play()
    {
        if (particles == null) return;
        foreach (var particle in particles)
        {
            if (!particle.isEmitting)
                particle.Play(false);
        }
    }

    public void Stop()
    {
        if (particles == null) return;
        foreach (var particle in particles)
        {
            if (particle.isEmitting)
                particle.Stop(false, ParticleSystemStopBehavior.StopEmitting);
        }
    }

    public void Clear()
    {
        if (particles == null) return;
        foreach (var particle in particles)
            particle.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);
    }
}
