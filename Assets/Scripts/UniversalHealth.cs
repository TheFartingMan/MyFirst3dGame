using System.Collections;
using UnityEngine;

public class UniversalHealth : MonoBehaviour
{
    #region Parameters
    [Header("Parameters")]
    [SerializeField] private float maxHealth;
    [SerializeField] private float startingHealth;
    [SerializeField] private float invincibilitySeconds;
    [SerializeField] private Color damageColor;
    #endregion

    #region Debug Variables
    [Header("Debug")]
    [SerializeField] private float currentHealth;
    #endregion

    #region Private variables
    private Renderer rend;
    private Color originalColor;
    private float iFrameTimer;
    #endregion

    void Start()
    {
        currentHealth = startingHealth;
        rend = GetComponent<Renderer>();
        originalColor = rend.material.color;
    }
    void Update()
    {
        iFrameTimer += Time.deltaTime;
    }

    //If trigger collider "hurtbox" is hit, then subtract health.
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Sword Hitbox"))
        {
            if (currentHealth > 0 && iFrameTimer >= invincibilitySeconds)
            {
                currentHealth--;
                flash();
                iFrameTimer = 0;
            }

            if (currentHealth <= 0)
            {
                Destroy(gameObject);
            } 
        }
        
    }

#region Color flash logic
    private IEnumerator DoFlash()
    {
        rend.material.color = damageColor;
        yield return new WaitForSeconds(invincibilitySeconds);
        rend.material.color = originalColor;
    }
    
    public void flash()
    {
        StopAllCoroutines();
        StartCoroutine(DoFlash());
    }
#endregion
}
