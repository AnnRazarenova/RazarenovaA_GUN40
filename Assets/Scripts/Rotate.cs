using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    [SerializeField]
    private Vector3 _rotate;

    private IEnumerator Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.isKinematic = true;
        }

        yield return new WaitForFixedUpdate();

        while (true)
        {
            if (rb != null)
            {
                Quaternion deltaRotation = Quaternion.Euler(_rotate * Time.fixedDeltaTime);
                rb.MoveRotation(rb.rotation * deltaRotation);
            }
            else
            {
                transform.Rotate(_rotate * Time.deltaTime);
            }

            yield return new WaitForFixedUpdate();
        }
    }

}
