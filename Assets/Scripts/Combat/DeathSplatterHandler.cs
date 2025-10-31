using UnityEngine;

public class DeathSplatterHandler : MonoBehaviour
{
    void OnEnable()
    {
        Health.OnDeath += SpawnDeathSplatterPrefab;
        Health.OnDeath += SpawnDeathVFX;
    }

    void OnDisable()
    {
        Health.OnDeath -= SpawnDeathSplatterPrefab;
        Health.OnDeath -= SpawnDeathVFX;
    }

    private void SpawnDeathSplatterPrefab(Health sender)
    {
        GameObject newSplatterPrefab = Instantiate(sender.SplatterPrefab, sender.transform.position, transform.rotation);
        SpriteRenderer deathSplatterSR = newSplatterPrefab.GetComponent<SpriteRenderer>();
        ColorChanger colorChanger = sender.GetComponent<ColorChanger>();

        if (colorChanger)
        {
            Color currentColor = colorChanger.DefaultColor;
            deathSplatterSR.color = currentColor;
        }

        newSplatterPrefab.transform.SetParent(this.transform);
    }

    private void SpawnDeathVFX(Health sender)
    {
        GameObject deathVFX = Instantiate(sender.DeathVFX, sender.transform.position, transform.rotation);
        ParticleSystem.MainModule ps = deathVFX.GetComponent<ParticleSystem>().main;
        ColorChanger colorChanger = sender.GetComponent<ColorChanger>();

        if (colorChanger)
        {
            Color currentColor = colorChanger.DefaultColor;
            ps.startColor = currentColor;
        }

        deathVFX.transform.SetParent(this.transform);
    }
}