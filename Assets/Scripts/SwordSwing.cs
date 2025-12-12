using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
public class SwordSwing : MonoBehaviour
{
    private Animator anim;
    [SerializeField] private ParticleSystem ps;
    void Start()
    {
        anim = GetComponent<Animator>();
    }
    void Update()
    {
        bool mouseClicked = Input.GetMouseButtonDown(0);

        if (mouseClicked)
        {
            anim.SetBool("isSwingingSword", true);
        }
        else
        {
            anim.SetBool("isSwingingSword", false);
        }
    }
    
    /// <summary>
    /// Used in the RH swing animation.
    /// </summary>
    public void playSwordSwing()
    {
        ps.Play(true);
    }

}
