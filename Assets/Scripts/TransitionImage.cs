using UnityEngine;
using UnityEngine.UI;

public class TransitionImage : MonoBehaviour
{
    private Animator anim;
    private Image im;
    
    void Start()
    {
        im = GetComponent<Image>();
        anim = GetComponent<Animator>();
    }
    
    void Update()
    {
        if (!(anim.GetCurrentAnimatorStateInfo(0).length >
             anim.GetCurrentAnimatorStateInfo(0).normalizedTime))
        {
            anim.enabled = false;
            im.enabled = false;
        }
    }
}
