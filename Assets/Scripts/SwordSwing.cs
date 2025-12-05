using UnityEngine;
using UnityEngine.InputSystem;
[RequireComponent(typeof(Animator))]
public class SwordSwing : MonoBehaviour
{
    private Animator anim;
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

}
