using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BAplayerController : MonoBehaviour
{
    float OriginalSpeed;
    public float MovementSpeed;
    public float RotationSpeed;
    public float Gravity = 10;

    CharacterController CC;
    PlayerInputActions playerInputActions;
    PlayerAttackSystem pas;
    PlayerHealth hp;
    Transform ThePlayer;
    Transform PlayerModel;
    Vector3 AimingLocation;

    public Image HpBar;
    public Image StaminaBar;

    [HideInInspector]
    public bool Aim;

    void Awake()
    {
        OriginalSpeed = MovementSpeed;
        GameManager.Player = gameObject;

        ThePlayer = transform.Find("ThePlayer");
        PlayerModel = ThePlayer.Find("PlayerModel");
        CC = ThePlayer.GetComponent<CharacterController>();
        pas = ThePlayer.GetComponent<PlayerAttackSystem>();
        hp = ThePlayer.GetComponent<PlayerHealth>();
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
    }

    // Start is called before the first frame update
    void Start()
    {
        playerInputActions.Player.Fire.performed += pas.Attack;
    }

    // Update is called once per frame
    void Update()
    {
        Sprinting();
        IsAiming();
        Move();
        ShowBars();
    }

    void Move()
    {  
        Vector2 v2 = playerInputActions.Player.Move.ReadValue<Vector2>();
        Vector3 V3 = new Vector3(v2.x, 0, v2.y);
        CC.Move(V3.normalized* MovementSpeed * Time.deltaTime);
        CC.Move(Vector3.down * Gravity * Time.deltaTime);

        if (pas.Acooldown <= 0) //While Not Attacking
        {
            if (Aim) //While Aim is On - Always Look at the mouse position
            {
                RaycastHit hit;
                Ray r = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
                if (Physics.Raycast(r, out hit, pas.OnlyFloor))
                {
                    AimAt(hit.point);
                }
                    
            } // While Aim is Off - Look To The Direction You move to
            else if (V3 != Vector3.zero)
            {

                Vector3 _direction = V3.normalized;
                Quaternion _lookRotation = Quaternion.LookRotation(_direction);
                PlayerModel.rotation = Quaternion.Slerp(PlayerModel.rotation, _lookRotation, Time.deltaTime * RotationSpeed);
            }
        }
        else // While Attacking - Look Fast at Attack Direction untill the Attack is over
        {
            AimingLocation += V3.normalized * MovementSpeed * Time.deltaTime;
            AimAt(AimingLocation, 10);
        }
        
    }

    void Sprinting()
    {
        InputActionPhase phase = playerInputActions.Player.Sprint.phase;
        if (pas.Stamina > pas.Tired)
        {
            if (phase == InputActionPhase.Started)
            {
                MovementSpeed = OriginalSpeed * 1.5f;
                pas.Stamina -= (pas.StaminaRegan + pas.StaminaCost) * Time.deltaTime;
            }
            else
            {
                MovementSpeed = OriginalSpeed;
            }
        }
        else
        {
            MovementSpeed = OriginalSpeed*0.5f;
            if (phase == InputActionPhase.Started)
            { pas.Stamina -= (pas.StaminaRegan + pas.StaminaCost) * Time.deltaTime; }
        }
    }

    void IsAiming()
    {
        InputActionPhase phase = playerInputActions.Player.Aim.phase;
        if (phase == InputActionPhase.Started)
            Aim = true;     
        else
            Aim = false;
    }

    void AimAt(Vector3 Target,float speed = 1)
    {
            Target.y = CC.transform.position.y;
            Vector3 _direction = (Target - CC.transform.position).normalized;
            Quaternion _lookRotation = Quaternion.LookRotation(_direction);
            PlayerModel.rotation = Quaternion.Slerp(PlayerModel.rotation, _lookRotation, Time.deltaTime * RotationSpeed * speed);     
    }

    public void SetAimPoint(Vector3 Target)
    {
        AimingLocation = Target;
    }

    void ShowBars()
    {
        HpBar.fillAmount = hp.Hp / hp.MaxHp;
        StaminaBar.fillAmount = pas.Stamina / pas.MaxStamina;
    }
    

}
public static class GameManager
{
    public static GameObject Player;

    public static IEnumerator RestartLevel()
    {
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
    


