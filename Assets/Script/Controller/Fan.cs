using UnityEngine;

public class Fan : MonoBehaviour
{
    public Vector3 rotationSpeed = new Vector3(0, 0, 500f);

    void Update()
    {
        RotateFan();
    }

    void RotateFan()
    {
        if(!DrawLineWithMouse.Instance.hasDrawn)
        {
            rotationSpeed.z = 500f;
        }
        else
        {
            rotationSpeed.z = 1000f;
        }

        transform.Rotate(rotationSpeed * Time.deltaTime);
    }
}
