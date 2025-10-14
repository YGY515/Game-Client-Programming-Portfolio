using System.Collections;
using UnityEngine;

public enum BossState
{
    RandomMove,
    Chase,
    Attack,
    Rest,
    Dead
}

public class BossController : MonoBehaviour
{
    public Animator anim;
    public GameObject shieldObject;
    public Transform player;
    public Rigidbody2D rb;
    public PlayerHealth playerHealth;
    public BossHealth bossHealth;
    public BossData bossData;

    private Vector2 currentDirection;
    private Vector2 randomDirection;

    private bool isMoving = true;
    private bool isDashing = false;

    private Coroutine stateCoroutine;
    private BossState currentState;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        PickRandomDirection();
        ChangeState(BossState.RandomMove);
    }

    void Update()
    {
        if (currentState == BossState.Dead)
            return;

        if (bossHealth.CurrentHealth <= 0 && currentState != BossState.Dead)
        {
            ChangeState(BossState.Dead);
            return;
        }

        switch (currentState)
        {
            case BossState.RandomMove:
                RandomMoveWithPause();

                if (!shieldObject.activeSelf)    // 방어막이 꺼지면 추격
                    ChangeState(BossState.Chase);
                break;

            case BossState.Chase:
                HandleChaseWithDash();

                if (shieldObject.activeSelf)    // 방어막이 켜지면 무작위 이동
                    ChangeState(BossState.RandomMove);

                else if (IsPlayerInAttackRange())
                    ChangeState(BossState.Attack);
                break;

                // Attack, Rest, Dead는 코루틴에서 처리하기
                // Attack: AttackStateCoroutine, DashRoutine
                // Rest: RestStateCoroutine
                // Dead: OnBossDied
        }
    }

    void FixedUpdate()
    {
        if (currentState == BossState.Chase)
        {
            rb.MovePosition(rb.position + currentDirection * bossData.moveSpeed * Time.fixedDeltaTime);
        }
    }

    void ChangeState(BossState newState)
    {
        if (stateCoroutine != null)
        {
            StopCoroutine(stateCoroutine);
            stateCoroutine = null;
        }

        currentState = newState;

        switch (newState)
        {
            case BossState.Attack:
                stateCoroutine = StartCoroutine(AttackStateCoroutine());
                break;
            case BossState.Rest:
                stateCoroutine = StartCoroutine(RestStateCoroutine());
                break;
            case BossState.Dead:
                OnBossDied();
                break;
        }
    }


    void RandomMoveWithPause()
    {
        bossData.moveTimer += Time.deltaTime;
        if (isMoving)
        {
            MoveAndAnimate(randomDirection);
            if (bossData.moveTimer >= bossData.randomMoveInterval)
            {
                bossData.moveTimer = 0f;
                isMoving = false;
            }
        }
        else
        {
            if (bossData.moveTimer >= bossData.restInterval)
            {
                bossData.moveTimer = 0f;
                isMoving = true;
                PickRandomDirection();
            }
        }
    }

    void PickRandomDirection()
    {
        Vector2[] directions = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
        randomDirection = directions[Random.Range(0, directions.Length)];
    }

    void HandleChaseWithDash()
    {
        if (!isDashing && bossData.cooldownTimer <= 0f)
        {
            isDashing = true;
            bossData.dashTimer = bossData.dashDuration;
        }

        if (isDashing)
        {
            bossData.moveSpeed = bossData.dashSpeed;
            bossData.dashTimer -= Time.deltaTime;
            if (bossData.dashTimer <= 0f)
            {
                isDashing = false;
                bossData.cooldownTimer = bossData.dashCooldown;
                bossData.moveSpeed = 5.0f;
            }
        }
        else
        {
            bossData.moveSpeed = 5.0f;
            if (bossData.cooldownTimer > 0f)
                bossData.cooldownTimer -= Time.deltaTime;
        }

        ChasePlayer();
    }

    void ChasePlayer()
    {
        if (player == null) return;
        Vector2 toPlayer = player.position - transform.position;
        float distance = toPlayer.magnitude;

        if (distance < 0.1f)
        {
            anim.SetFloat("Looking", anim.GetFloat("Looking"));
            currentDirection = Vector2.zero;
            rb.velocity = Vector2.zero;
            return;
        }

        Vector2 direction = toPlayer.normalized;
        currentDirection = direction;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        float looking = 0.00f;

        if (angle >= -45f && angle < 45f)
            looking = 0.66f;
        else if (angle >= 45f && angle < 135f)
            looking = 1.00f;
        else if (angle >= -135f && angle < -45f)
            looking = 0.00f;
        else
            looking = 0.33f;

        anim.SetFloat("Looking", looking);
    }

    void MoveAndAnimate(Vector2 dir)
    {
        if (dir == Vector2.right)
            anim.SetFloat("Looking", 0.66f);
        else if (dir == Vector2.left)
            anim.SetFloat("Looking", 0.33f);
        else if (dir == Vector2.up)
            anim.SetFloat("Looking", 1.00f);
        else if (dir == Vector2.down)
            anim.SetFloat("Looking", 0.00f);

        transform.position += (Vector3)(dir.normalized * bossData.moveSpeed * Time.deltaTime);
    }

    bool IsPlayerInAttackRange()
    {
        if (player == null) return false;
        Vector2 diff = player.position - transform.position;
        return Mathf.Abs(diff.x) <= bossData.attackDistanceX && Mathf.Abs(diff.y) <= bossData.attackDistanceY;
    }

    IEnumerator AttackStateCoroutine()
    {
        // 공격(대쉬) 후 Rest 상태로 전이
        Vector2 diff = player.position - transform.position;
        Vector2 dashDir = diff.normalized;

        yield return StartCoroutine(DashRoutine(dashDir));
        ChangeState(BossState.Rest);
    }

    IEnumerator RestStateCoroutine()
    {
        // 공격 쿨타임 후 다시 Chase로 전이
        yield return new WaitForSeconds(bossData.attackCooldown);
        ChangeState(BossState.Chase);
    }

    IEnumerator DashRoutine(Vector2 dashDir)
    {
        float dashTime = 0.2f;
        float elapsed = 0f;
        Vector2 startPos = rb.position;
        Vector2 endPos = startPos + dashDir * bossData.dashDistance;

        bool didDamage = false;

        while (elapsed < dashTime)
        {
            elapsed += Time.deltaTime;
            Vector2 newPos = Vector2.Lerp(startPos, endPos, elapsed / dashTime);
            rb.MovePosition(newPos);

            if (!didDamage && Vector2.Distance(newPos, player.position) <= 0.5f)
            {
                playerHealth.PlayerTakeDamage(bossData.damageAmount);
                didDamage = true;
            }

            yield return null;
        }

        rb.MovePosition(endPos);
    }

    void OnBossDied()
    {
        StartCoroutine(DieWithArcEffect());
    }

    IEnumerator DieWithArcEffect()
    {
        rb.velocity = Vector2.zero;
        rb.gravityScale = 0f;
        anim.enabled = false;

        float duration = 1.0f;
        float elapsed = 0f;
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + new Vector3(4f, 0f, 0f);
        float height = 2.5f;

        float startAngle = transform.eulerAngles.z;
        float endAngle = startAngle - 90f;

        SpriteRenderer sr = bossHealth.Boss_spriteRenderer;
        Color startColor = sr.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            Vector3 parabola = Vector3.Lerp(startPos, endPos, t);
            parabola.y += height * 4 * t * (1 - t);
            transform.position = parabola;

            float angle = Mathf.Lerp(startAngle, endAngle, t);
            transform.rotation = Quaternion.Euler(0, 0, angle);

            if (sr != null)
            {
                Color c = startColor;
                c.a = Mathf.Lerp(1f, 0f, t);
                sr.color = c;
            }

            yield return null;
        }

        if (sr != null)
        {
            Color c = startColor;
            c.a = 0f;
            sr.color = c;
        }
        transform.rotation = Quaternion.Euler(0, 0, endAngle);
        transform.position = endPos;

        Destroy(gameObject);
    }
}