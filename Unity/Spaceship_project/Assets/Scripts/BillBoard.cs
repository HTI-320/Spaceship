using UnityEngine;

public class Billboard : MonoBehaviour
{
    void LateUpdate()
    {
        transform.localPosition = Vector3.zero;

        if (Camera.main != null)
        {
            transform.rotation = Camera.main.transform.rotation;
        }
    }
}