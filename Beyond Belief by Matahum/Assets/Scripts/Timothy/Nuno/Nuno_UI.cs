using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Nuno_UI : MonoBehaviour
{
    private Nuno_Stats nunoStats;
    private Nuno nuno;

    [Header("Health UI Properties")]
    [SerializeField] private Image healthFillDelayed;
    [SerializeField] private Image healthFillInstant;
    [SerializeField] private float healthFillDelaySpeed = 2f;
    [SerializeField] private TextMeshProUGUI healthAmountTxt;

    private void Start()
    {
        nuno = GetComponent<Nuno>();
        nunoStats = GetComponent<Nuno_Stats>();
    }

    private void Update()
    {
        UIHealthUpdate();
    }
    void UIHealthUpdate()
    {
        float current = nunoStats.n_currentHealth;
        float max = nunoStats.n_maxHealth;

        float targetFill = current / max;

        healthFillInstant.fillAmount = targetFill;

        if (healthFillDelayed.fillAmount > targetFill)
        {
            healthFillDelayed.fillAmount = Mathf.Lerp(healthFillDelayed.fillAmount, targetFill, Time.deltaTime * healthFillDelaySpeed);
        }
        else
        {
            healthFillDelayed.fillAmount = targetFill;
        }

        healthAmountTxt.text = $"{current:F0} / {max:F0}";
    }
}
