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

    public Image HpBar;
    public Image StaminaBar;

    void Awake()
    {
        OriginalSpeed = MovementSpeed;
        GameManager.Player = gameObject;

        ThePlayer = transform.Find("ThePlayer");
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
        Move();
        ShowBars();
    }

    void Move()
    {
        Sprinting();
        Vector2 v2 = playerInputActions.Player.Move.ReadValue<Vector2>();
        Vector3 V3 = new Vector3(v2.x, 0, v2.y);
        CC.Move(V3.normalized* MovementSpeed * Time.deltaTime);
        CC.Move(Vector3.down * Gravity * Time.deltaTime);

        if (V3 != Vector3.zero && pas.Acooldown<=0)
        {
            Vector3 _direction = (CC.transform.position + V3 - CC.transform.position).normalized;
            Quaternion _lookRotation = Quaternion.LookRotation(_direction);
            ThePlayer.Find("PlayerModel").rotation = Quaternion.Slerp(ThePlayer.Find("PlayerModel").rotation, _lookRotation, Time.deltaTime * RotationSpeed);
        }
    }

    public void Sprinting()
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
    


