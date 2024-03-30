using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private BasicCombat basicCombat;

    // Start is called before the first frame update
    void Start()
    {
        basicCombat = GetComponent<BasicCombat>();
    }

    // Update is called once per frame
    void Update()
    {
        Attack();
    }

    private void Attack()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            basicCombat.Attack();
        }
    }
}
