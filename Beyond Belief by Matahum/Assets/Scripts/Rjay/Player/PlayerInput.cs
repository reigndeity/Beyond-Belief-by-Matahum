using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    [Header("Movement Input")]
    public KeyCode forwardKey = KeyCode.W;
    public KeyCode backwardKey = KeyCode.S;
    public KeyCode leftwardKey = KeyCode.A;
    public KeyCode rightwardKey = KeyCode.D;
    [Header("Toggle Movement Input")]
    public KeyCode toggleKey = KeyCode.LeftControl;
    
    [Header("Sprint Input")]
    public KeyCode sprintKey = KeyCode.LeftShift;
    [Header("Jump Input")]
    public KeyCode jumpKey = KeyCode.Space;
    public Vector2 MovementInput { get; private set; }
    public bool IsJumping { get; private set; }
    public bool IsSprinting { get; private set; }


    [Header("Skills Input")]
    public KeyCode normalSkillKey = KeyCode.E;
    public KeyCode ultimateSkillKey = KeyCode.Q;
    
    [Header("Agimat Input")]
    public KeyCode agimatOneKey = KeyCode.R;
    public KeyCode agimatTwoKey = KeyCode.C;
    
    [Header("Hud Input")]
    public KeyCode gameMenuKey = KeyCode.Escape;
    public KeyCode inventoryKey = KeyCode.Tab;
    public KeyCode questGuideKey = KeyCode.V;
    public KeyCode questLogKey = KeyCode.J;
    public KeyCode cursorShowKey = KeyCode.LeftAlt;
    [Header("Misc Input")]
    public KeyCode interactKey = KeyCode.F;
    public KeyCode mapKey = KeyCode.M;
    
    public Vector2 GetMovementInput()
    {
        float horizontal = (Input.GetKey(leftwardKey) ? -1 : 0) + (Input.GetKey(rightwardKey) ? 1 : 0);
        float vertical = (Input.GetKey(backwardKey) ? -1 : 0) + (Input.GetKey(forwardKey) ? 1 : 0);
        return new Vector2(horizontal, vertical);
    }
}