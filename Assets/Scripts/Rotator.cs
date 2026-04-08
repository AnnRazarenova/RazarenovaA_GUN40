using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField]
    private Vector3 _rotate;


    // Start is called before the first frame update
    private IEnumerator Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.isKinematic = true;
        }

        while (true)
        {
            transform.Rotate(_rotate * Time.deltaTime);
            yield return null;
        }
    }

}
