using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Log : MonoBehaviour
{
    private int lifeSpan;
    // Start is called before the first frame update
    void Start()
    {
        lifeSpan = 0;
        StartCoroutine(logTimer());
    }

    // Update is called once per frame
    void Update()
    {

    }

    private IEnumerator logTimer()
    {
        yield return new WaitForSeconds(1f);

        lifeSpan += 1;

        if (lifeSpan >= 5)
        {
            Destroy(gameObject);
        }

        StartCoroutine(logTimer());
    }
}
