using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Baton : MonoBehaviour, IWeapon
{
    public int Damage => 0; 
    public Animator playerAnimator;
    public Transform batonTransform;
    public Collider2D Collider2D;

    public AudioSource audioSource;
    public AudioClip swingClip;
    public BossHealth bossHealth;
    public EnemyController enemy;
    public GameObject sheildObject;
    public GameObject playerDialogue;
    public GameObject boss;


    void OnTriggerEnter2D(Collider2D other)
    {
        if ((other.CompareTag("Boss")) && (!sheildObject.activeSelf))
        {

            bool isCritical = Random.value < 0.3f; 
            int Damage = isCritical ? 25 : Random.Range(15, 24); // ġ��Ÿ�� 30 ������, �ƴϸ� �Ϲ�

            Debug.Log("������ �´���");
            bossHealth.BossTakeDamage(Damage);

            DamageTextManager.Instance.ShowDamage(
                boss.transform.position, Damage, isCritical

            );
        }

        if (other.CompareTag("Enemy"))
        {
            EnemyController target = other.GetComponent<EnemyController>();
            if (target.canBeDamaged == true)
            {
                target.TakeDamage(1);
            }
            else
            {
                playerDialogue.SetActive(true);
            }
        }

    }


    public void Attack()
    {

        if (playerAnimator == null)
        {
            Debug.LogError("playerAnimator�� �Ҵ���� ����");
            return;
        }


        float looking = playerAnimator.GetFloat("Looking");

        int dir = 0;
        if (Mathf.Approximately(looking, 0.00f))
            dir = 0; // �Ʒ�
        else if (Mathf.Approximately(looking, 1.00f))
            dir = 1; // ��
        else if (Mathf.Approximately(looking, 0.33f))
            dir = 2; // ����
        else if (Mathf.Approximately(looking, 0.66f))
            dir = 3; // ������
        else
            dir = 0; // �⺻��(�Ʒ�)


        Vector3[] positions = {
        new Vector3(0, -0.5f, 0),       // �Ʒ�
        new Vector3(0, 0.4f, 0),        // ��
        new Vector3(-0.2f, -0.3f, 0),   // ����
        new Vector3(0.3f, -0.2f, 0)     // ������
    };

        float[] startZ = { -100f, 100f, -180f, -20f }; // �Ʒ�, ��, ����, ������

        batonTransform.gameObject.SetActive(true);
        batonTransform.localPosition = positions[dir];
        batonTransform.localEulerAngles = Vector3.zero;

        Quaternion startRotation = Quaternion.Euler(0, 0, startZ[dir]);
        Quaternion endRotation = Quaternion.Euler(0, 0, startZ[dir] + 100f);
        
        playerAnimator.gameObject.GetComponent<MonoBehaviour>().StartCoroutine(RotateBaton(batonTransform, startRotation, endRotation, 0.1f));
    }

    private IEnumerator RotateBaton(Transform target, Quaternion from, Quaternion to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            target.localRotation = Quaternion.Slerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        if(swingClip != null && audioSource != null)
            audioSource.PlayOneShot(swingClip);

        target.localRotation = to;
        target.gameObject.SetActive(false);
    }
}
