using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helper : MonoBehaviour
{
    public int level = 1;
    public int[] sellingPrice;
    public int[] upgradePrice;
    public int[] assistAmount;

    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        StartCoroutine(animationFirer());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator animationFirer()
    {
        while (true) 
        {
            animator.SetTrigger("Fire");
            yield return new WaitForSeconds(20f);
        }
        
    }
}
