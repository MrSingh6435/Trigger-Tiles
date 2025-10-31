using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DiscoBallManager : MonoBehaviour
{    
    public static Action OnDiscoBallHitEvent;

    [SerializeField] private Light2D _globalLigh;
    [SerializeField] private float _discoBallPartyTime = 2f;
    [SerializeField] private float _discoGlobalLightIntensity = .2f;

    private float _defaultGlobalLighIntensity;
    private Coroutine _discoCoroutine;
    private ColorSpotlight[] _allSpotLightes;
        
    private void Awake()
    {
    _defaultGlobalLighIntensity = _globalLigh.intensity;
    }

    void Start()
    {
        _allSpotLightes = FindObjectsByType<ColorSpotlight>(FindObjectsSortMode.None);
    }

    void OnEnable()
    {
        OnDiscoBallHitEvent += DimTheLightes;
    }

    void OnDisable()
    {
        OnDiscoBallHitEvent -= DimTheLightes;
    }

    public void DiscoBallParty()
    {
        if (_discoCoroutine != null) { return; }
        OnDiscoBallHitEvent?.Invoke();
    }

    private void DimTheLightes()
    {
        foreach (ColorSpotlight colorSpotlight in _allSpotLightes)
        {
            StartCoroutine(colorSpotlight.SpotLightDiscoParty(_discoBallPartyTime));
        }

        _discoCoroutine = StartCoroutine(GlobalLightResetRoutine());
    }

    public IEnumerator GlobalLightResetRoutine()
    {
        _globalLigh.intensity = _discoGlobalLightIntensity;
        yield return new WaitForSeconds(_discoBallPartyTime);
        _globalLigh.intensity = _defaultGlobalLighIntensity;
        _discoCoroutine = null;
    }
}
