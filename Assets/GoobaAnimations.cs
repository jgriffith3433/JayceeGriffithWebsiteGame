using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoobaAnimations : MonoBehaviour
{
    [SerializeField]
    private Animator animator;
    private SkinnedMeshRenderer skinnedMeshRenderer;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        StartCoroutine(TestAnimations());
    }

    private IEnumerator TestAnimations()
    {
        yield return new WaitForSeconds(5);
        animator.SetTrigger("Jump");
        yield return new WaitForSeconds(5);
        animator.SetTrigger("Walk");
        yield return new WaitForSeconds(5);
        animator.SetTrigger("Jump");
        animator.SetTrigger("Walk");
        yield return TestAnimations();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
