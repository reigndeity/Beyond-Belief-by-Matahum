using UnityEngine;

[CreateAssetMenu(menuName = "Mangkukulam/Mangkukulam Abilities/Potion Blitz")]
public class PotionBlitz_MangkukulamAbility : Mangkukulam_Ability
{
    [Header("Positions to go to")]
    public Transform[] positions;
    public int posCount = 8;
    public float radius = 3f;
    public bool skillOnGoing = false;

    GameObject mangkukulamObj;

    public float cooldown;
    public bool canBeUsed = true;
    public override float Cooldown() => cooldown;
    public override void Activate(GameObject user)
    {
        mangkukulamObj = user;
    }

    public override bool CanBeUsed() => canBeUsed;

    // 🔴 Draw Gizmo for radius
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(mangkukulamObj.transform.position, radius);

        // Optional: show needle spawn points as little spheres
        if (posCount > 0)
        {
            for (int i = 0; i < posCount; i++)
            {
                float angle = i * Mathf.PI * 2f / posCount;
                Vector3 pos = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
                Gizmos.DrawSphere(mangkukulamObj.transform.position + pos, 0.1f);
            }
        }
    }
}
