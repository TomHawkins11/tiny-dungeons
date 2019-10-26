using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishedAnimating : MonoBehaviour
{
    private Animator _animator;
    // Start is called before the first frame update
    void Start()
    {
        _animator = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Done"))
        {
            Destroy(gameObject);
        }
    }
}
