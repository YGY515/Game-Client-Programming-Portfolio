using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public PlayerData playerData;
    public Animator anim;
    public Rigidbody2D rb;
    public WeaponManager weaponManager;
    public WeaponData weaponData;

    [SerializeField] private BossHealth BossStatus;
    
    [SerializeField] private GameObject Baton_Icon;
    [SerializeField] private GameObject Baton_Empty;

    [SerializeField] private GameObject Gun_Icon;
    [SerializeField] private GameObject Gun_Empty;

    [SerializeField] private GameObject Dash_Icon;
    [SerializeField] private GameObject Dash_Empty;

    public AudioSource weapon_ChangeSource;
    public AudioClip Clip;

    private bool canAttack = true;
    private bool canDash = true;
    private bool isDashing = false;


    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        HandleMovementInput();
        WeaponInput();
        DashInput();
    }

    void HandleMovementInput()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        anim.SetFloat("PositionX", horizontal);
        anim.SetFloat("PositionY", vertical);

        Vector2 movement = new Vector2(horizontal, vertical).normalized * playerData.moveSpeed;
        rb.velocity = movement;


        if (movement != Vector2.zero)
        {
            anim.SetBool("Running", true);
            if (!isDashing) // 대쉬 중이 아닐 때만 속도 변경
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    anim.SetBool("Flying", true);
                    playerData.moveSpeed = 7f;
                }
                else
                {
                    anim.SetBool("Flying", false);
                    playerData.moveSpeed  = 5f;
                }
            }
 

            if (horizontal > 0)     // 오른쪽
            {
                anim.SetFloat("Looking", 0.66f);

            }
            else if (horizontal < 0) // 왼쪽
            {

                anim.SetFloat("Looking", 0.33f);

            }
            else if (vertical > 0)  // 위쪽
            {
                anim.SetFloat("Looking", 1.00f);

            }
            else if (vertical < 0) // 아래쪽
            {
                anim.SetFloat("Looking", 0.00f);

            }

        }
        else
        {
            anim.SetBool("Flying", false);
            anim.SetBool("Running", false);

        }

    }

    void WeaponInput()
    {
        if (Input.GetKeyDown(KeyCode.Z) && canAttack)
        {
            weaponManager.UseWeapon();
            StartCoroutine(WeaponCooldown());
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            weaponManager.ChangeWeapon();
            { 
                weapon_ChangeSource.PlayOneShot(Clip); 
                if (weaponData.weapon_index == 0)
                {
                    Baton_Icon.SetActive(true);
                    Gun_Icon.SetActive(false);
                }
                else if (weaponData.weapon_index == 1)
                {
                    Baton_Icon.SetActive(false);
                    Gun_Icon.SetActive(true);
                }
            }
        }
            
    }

    private IEnumerator WeaponCooldown()
    {
        if (weaponData.weapon_index == 0)
        {   
            // 근접 무기일 때
            canAttack = false;
            if (Baton_Icon != null) Baton_Icon.SetActive(false);
            if (Baton_Empty != null) Baton_Empty.SetActive(true);

            yield return new WaitForSeconds(playerData.weaponCooldown);

            if (Baton_Icon != null) Baton_Icon.SetActive(true);
            if (Baton_Empty != null) Baton_Empty.SetActive(false);
            canAttack = true;
        }

        else if (weaponData.weapon_index == 1)
        {   
            // 원거리 무기일 때
            canAttack = false;
            if (Gun_Icon != null) Gun_Icon.SetActive(false);
            if (Gun_Empty != null) Gun_Empty.SetActive(true);

            yield return new WaitForSeconds(playerData.weaponCooldown);

            if (Gun_Icon != null) Gun_Icon.SetActive(true);
            if (Gun_Empty != null) Gun_Empty.SetActive(false);
            canAttack = true;
        }
    }

    void DashInput()
    {
        if (Input.GetKeyDown(KeyCode.X) && canDash)
        {
            StartCoroutine(DashCooldown());
        }
    }

    private IEnumerator DashCooldown()
    {
        canDash = false;
        isDashing = true;
        if (Dash_Icon != null) Dash_Icon.SetActive(false);
        if (Dash_Empty != null) Dash_Empty.SetActive(true);

        float prevSpeed = playerData.moveSpeed;
        playerData.moveSpeed += playerData.dashSpeed;

        yield return new WaitForSeconds(playerData.dashDuration);

        playerData.moveSpeed = prevSpeed;
        isDashing = false;

        yield return new WaitForSeconds(playerData.weaponCooldown);

        if (Dash_Icon != null) Dash_Icon.SetActive(true);
        if (Dash_Empty != null) Dash_Empty.SetActive(false);
        canDash = true;
    }

}
