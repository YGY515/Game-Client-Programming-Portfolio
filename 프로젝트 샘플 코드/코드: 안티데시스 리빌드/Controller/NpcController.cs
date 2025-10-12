using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcController : MonoBehaviour
{
    public Animator anim;
    public Transform player; 
    public float interactDistance = 2f;

    public EnemyController[] enemies;
    public GameObject bossShield;
    public GameObject NpcDialogue;


    void Start()
    {
        
        if (anim != null)
            anim.SetFloat("Looking", 0.33f);    // ���� �� ���� �ٶ󺸱�
    }

    void Update()
    {
        Interaction();
    }

    void Interaction()
    {
        if (player != null && bossShield != null)
        {
            float dist = Vector2.Distance(transform.position, player.position);
            if (dist <= interactDistance && Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("NPC�� ��ȣ�ۿ� ����");
                anim.SetFloat("Looking", 0.66f);
                NpcDialogue.SetActive(true);
                OnPhaseChanged(BossPhaseManager.Instance.currentPhase);

            }
        }
    }

    void OnPhaseChanged(int phase)
    {
        NpcDialogue.SetActive(true);
        EnableEnemyDamage();

        if (phase == 1) { }
        else if (phase == 2)
        {
            
        }
    }

    void EnableEnemyDamage()
    {
        EnemyPoolManager.Instance.EnemyCanBeDamaged = true;

        foreach (var enemy in enemies)
        {
            if (enemy != null)
            {
                enemy.canBeDamaged = true;
            }
        }
    }



}