using UnityEngine;
using UnityEngine.UIElements;

public class Temp_MoveScript : MonoBehaviour
{
    public float speed;
    public float rotationSpeed;

    private void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Vector3 move = new Vector3(x, 0, z);

        transform.Translate(move * speed * Time.deltaTime, Space.World);

        if (move.magnitude > 0)
        {
            Quaternion rotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
        }
    }
}
