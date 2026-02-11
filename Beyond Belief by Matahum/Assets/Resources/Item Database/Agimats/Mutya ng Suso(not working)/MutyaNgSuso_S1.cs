// using System.Collections;
// using UnityEngine;
// [CreateAssetMenu(menuName = "Agimat/Abilities/MutyaNgSuso/MutyaNgSuso_S1")]
// public class MutyaNgSuso_S1 : R_AgimatAbility
// {
//     [HideInInspector] public float duration = 0;
//     public override string GetDescription(R_ItemRarity rarity)
//     {
//         if(duration == 0)
//             duration = GetRandomDuration(rarity);

//         return $"Turns invisible for {duration} seconds";
//     }
//     public override void Activate(GameObject user, R_ItemRarity rarity)
//     {
//         CoroutineRunner.Instance.RunCoroutine(GoInvisible(user));
//     }

//     IEnumerator GoInvisible(GameObject user)
//     {
//         SetTransparency(user, 0.5f);
//         yield return new WaitForSeconds(duration);
//         SetTransparency(user, 1f);
//     }
//     public static void SetTransparency(GameObject user, float alpha)
//     {
//         SkinnedMeshRenderer[] renderers = user.GetComponentsInChildren<SkinnedMeshRenderer>();

//         foreach (var rend in renderers)
//         {
//             foreach (var mat in rend.materials)
//             {
//                 Color c = mat.color;
//                 c.a = alpha;
//                 mat.color = c;

//                 // Ensure material uses transparent rendering
//                 mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
//                 mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
//                 mat.SetInt("_ZWrite", 0);
//                 mat.DisableKeyword("_ALPHATEST_ON");
//                 mat.EnableKeyword("_ALPHABLEND_ON");
//                 mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
//                 mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
//             }
//         }
//     }

//     #region RANDOM VALUE GENERATOR
//     private float GetRandomDuration(R_ItemRarity rarity)
//     {
//         return rarity switch
//         {
//             R_ItemRarity.Common => Random.Range(1.5f, 3f),
//             R_ItemRarity.Rare => Random.Range(2f, 4f),
//             R_ItemRarity.Epic => Random.Range(2.5f, 5f),
//             R_ItemRarity.Legendary => Random.Range(3f, 6f),
//             _ => 0.75f
//         };
//     }
//     #endregion
// }
