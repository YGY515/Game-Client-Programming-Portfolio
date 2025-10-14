using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{

    public AudioSource audioSource;
    public AudioClip hitClip;
    public SpriteRenderer spriteRenderer;
    public Color hitColor = Color.red;
    public PlayerData playerData;

    public event Action<int> HealthChange;
    public event Action HealthWarning;

    private int currentHealth;
    public int MaxHealth => playerData.maxHealth;
    public int CurrentHealth => currentHealth;


    void Awake()
    {
        currentHealth = playerData.maxHealth;
    }

    private Coroutine hitCoroutine;

    private IEnumerator HitEffect()
    {
        Color originalColor = spriteRenderer.color;
        spriteRenderer.color = hitColor;

        float elapsed = 0f;
        while (elapsed < playerData.hitColorDuration)
        {
            spriteRenderer.color = Color.Lerp(hitColor, originalColor, elapsed / playerData.hitColorDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        spriteRenderer.color = originalColor;
        hitCoroutine = null;
    }

    public void PlayerTakeDamage(int damage)
    {
        if (damage <= 0) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);

        if (audioSource != null && hitClip != null)
            audioSource.PlayOneShot(hitClip);

        // ���� ����Ʈ�� �ڷ�ƾ �ߺ� ����
        if (spriteRenderer != null)
        {
            if (hitCoroutine != null)
                StopCoroutine(hitCoroutine);

            hitCoroutine = StartCoroutine(HitEffect());
        }

        HealthChange?.Invoke(currentHealth);
        //Debug.Log($"�÷��̾� ���� ������: {damage}, ���� ü��: {currentHealth}");

        if (currentHealth <= playerData.maxHealth / 3)
        {
            HealthWarning?.Invoke();
        }
    }

    public void PlayerHeal(int amount)
    {
        if (amount <= 0) return;

        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, playerData.maxHealth);

        HealthChange?.Invoke(currentHealth);
    }

}