using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SCR_Health)), RequireComponent(typeof(SCR_TeamAgent))]
public class SCR_ControlTower : MonoBehaviour
{

    [SerializeField, Tooltip("Renderers used by the tower.")]
    private MeshRenderer[] towerRenderers;
    [SerializeField, Tooltip("The color of the tower when healthy.")]
    private Color healthyColor = Color.blue;
    [SerializeField, Tooltip("The color of the tower when damage.")]
    private Color damagedColor = Color.red;
    [SerializeField, Tooltip("The owned health component of the tower.")]
    private SCR_Health ownedHealth;
    [SerializeField, Tooltip("The falloff of the damaged effect when hit.")]
    private AnimationCurve damageEffectFallof;
    // The progress of the damage effect fallof
    private float damageEffectProgress = 1.0f;
    [SerializeField, Tooltip("The period of the damage effect fallof")]
    private float damageFallofPeriod = 0.5f;
    // The target new color
    public Color targetColor;
    [SerializeField, Tooltip("Game manager reference for calling the death script")]
    private SCR_GameManager gameManager;


    // Start is called before the first frame update
    void Start()
    {
        // Validate components
        if (!ownedHealth)
        {
            ownedHealth = GetComponent<SCR_Health>();
        }
        if (ownedHealth)
        {
            ownedHealth.onDeath += OnDeath;
            ownedHealth.onHit += OnHit;
        }

        // Set initial color
        foreach(MeshRenderer renderer in towerRenderers)
        {
            renderer.material.color = healthyColor;
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (damageEffectProgress < 1.0f)
        {
            damageEffectProgress = Mathf.Min((damageEffectProgress + Time.deltaTime * (1.0f / damageFallofPeriod)), 1.0f);
            Color newColor = Color.Lerp(damagedColor, targetColor, damageEffectFallof.Evaluate(damageEffectProgress));
            foreach (MeshRenderer renderer in towerRenderers)
            {
                renderer.material.color = newColor;
            }
        }
    }

    public void OnDeath()
    {
        gameManager.PlayerLose();
        Destroy(gameObject, 0.01f);
    }

    public void OnHit()
    {
        float remainingHealthScalar = ownedHealth.Health / ownedHealth.MaxHealth;
        targetColor = Color.Lerp(damagedColor, healthyColor, remainingHealthScalar);
        foreach (MeshRenderer renderer in towerRenderers)
        {
            renderer.material.color = damagedColor;
        }
        damageEffectProgress = 0.0f;
    }
}
