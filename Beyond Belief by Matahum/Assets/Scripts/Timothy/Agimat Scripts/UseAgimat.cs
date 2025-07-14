using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UseAgimat : MonoBehaviour
{
    public Agimat firstAgimat;
    public Agimat secondAgimat;

    public Image firstAgimatIcon;
    public Image secondAgimatIcon;

    public TextMeshProUGUI firstHotKey;
    public TextMeshProUGUI secondHotKey;

    public Transform spawnPoint; // Manually set spawn point in Inspector

    private PlayerInput playerInput;

    public float firstTimer;
    public float secondTimer;
    bool canFirst;
    bool canSecond;

    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    void Update()
    {
        FirstAbility();
        SecondAbility();
    }

    void FirstAbility()
    {
        if (firstAgimat != null)
        {
            firstAgimatIcon.enabled = true;
            firstAgimatIcon.sprite = firstAgimat.FirstIcon();

            if (firstAgimatIcon.fillAmount < 1f)
            {
                firstTimer += Time.deltaTime;
                firstAgimatIcon.fillAmount = firstTimer / firstAgimat.FirstCooldown();
            }
            else
            {
                firstAgimatIcon.fillAmount = 1f;
                canFirst = true;
            }

            // Check if First Ability is an active skill
            if (!firstAgimat.FirstPassive())
            {
                firstHotKey.text = playerInput.agimatOneKey.ToString();
                if (Input.GetKeyDown(playerInput.agimatOneKey) && canFirst)
                {
                    canFirst = false;
                    firstAgimatIcon.fillAmount = 0f;
                    firstTimer = 0f;

                    firstAgimat.FirstAbility(); // Execute first ability logic
                }
            }
            // If passive, continuously activate it if with cooldown
            else
            {
                firstHotKey.text = "";
                if (canFirst && firstAgimat.FirstCooldown() != 0)
                {
                    canFirst = false;
                    firstAgimatIcon.fillAmount = 0f;
                    firstTimer = 0f;
                    firstAgimat.FirstAbility();
                }
                else if (firstAgimat.FirstCooldown() == 0)
                {
                    firstAgimat.FirstAbility();
                }
            }
        }
        else
        {
            /*firstAgimatIcon.enabled = false;
            firstHotKey.text = "";*/
        }
    }

    void SecondAbility()
    {
        if (secondAgimat != null)
        {
            secondAgimatIcon.enabled = true;
            secondAgimatIcon.sprite = secondAgimat.SecondIcon();

            secondTimer += Time.deltaTime;

            if (secondAgimatIcon.fillAmount < 1f)
            {
                secondAgimatIcon.fillAmount = secondTimer / secondAgimat.SecondCooldown();
            }
            else
            {
                secondAgimatIcon.fillAmount = 1f;
                canSecond = true;
            }

            // Check if Second Ability is an active skill
            if (!secondAgimat.SecondPassive())
            {
                secondHotKey.text = playerInput.agimatTwoKey.ToString();

                if (Input.GetKeyDown(playerInput.agimatTwoKey) && canSecond)
                {
                    canSecond = false;
                    secondAgimatIcon.fillAmount = 0f;
                    secondTimer = 0f;

                    secondAgimat.SecondAbility(); // Execute second ability logic
                }
            }
            // If passive, continuously activate it
            else
            {
                secondHotKey.text = "";
                if (canSecond && secondAgimat.SecondCooldown() != 0)
                {
                    canSecond = false;
                    secondAgimatIcon.fillAmount = 0f;
                    secondTimer = 0f;
                    secondAgimat.SecondAbility();
                }
                else if (secondAgimat.SecondCooldown() == 0)
                {
                    secondAgimat.SecondAbility();
                }
            }
        }
        else
        {
            /*secondAgimatIcon.enabled = false;
            secondHotKey.text = "";*/
        }
    }
}
