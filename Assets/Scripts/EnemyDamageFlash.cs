using System.Collections;
using UnityEngine;

public class EnemyDamageFlash : MonoBehaviour
{


    /*
        This is a temporary script that is not used in anything. This was grabbed from a tutorial, and I'm just getting things written down here.
        This code probably should not be attatched to anything.
    */



    [SerializeField] private float flashTime;
    [SerializeField] private Color flashColor = Color.red;
    [SerializeField] private Renderer rend;

    [SerializeField] private Color originalColor;

    void Start()
    {
        originalColor = rend.material.color;
    }

    private IEnumerator DoFlash()
    {
        rend.material.color = flashColor;
        yield return new WaitForSeconds(flashTime);
        rend.material.color = originalColor;
    }

    public void flash()
    {
        StopAllCoroutines();
        StartCoroutine(DoFlash());
    }
}
