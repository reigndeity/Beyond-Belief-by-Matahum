using UnityEngine;

public class VineCollider : MonoBehaviour
{
    [HideInInspector] public MutyaNgSampalok_VineDamage mainVine;
    [HideInInspector] public Collider collider;

    private void Awake()
    {
        collider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Hit {other}");
        if(other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            mainVine.HandleTrigger(other);
    }
}
