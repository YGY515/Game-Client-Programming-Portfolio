using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class NpcDialogue : MonoBehaviour
{
    public Animator npcAnimation;
    public GameObject descriptionPanel;
    public GameObject timer;
    public EnemySpawner enemySpawner;
    public PlayerHealth playerHealth;
    
    [SerializeField] private TMP_Text npcTextUI;

    void OnEnable()
    {
        if (descriptionPanel != null)
            descriptionPanel.SetActive(true);

        CsvReaderManager dialogueReader = CsvReaderManager.Instance;
        if (dialogueReader == null)
            return;

        int currentPhase = BossPhaseManager.Instance.currentPhase;

        // csv의 헤더 phase와 currentPhase가 일치하는 대사 추출
        List<string> linesToUse = new List<string>();
        foreach (var kvp in dialogueReader.npcDialogue)
        {
            var dialogue = kvp.Value;
            if (dialogue.phase == currentPhase.ToString())
                linesToUse.Add(dialogue.text);
        }

        // 헤더 order 기준으로 정렬
        linesToUse.Sort((a, b) =>
        {
            var orderA = int.Parse(dialogueReader.npcDialogue
                [dialogueReader.npcDialogue
                    .FirstOrDefault(x => x.Value.text == a).Key].order);
            var orderB = int.Parse(dialogueReader.npcDialogue
                [dialogueReader.npcDialogue
                    .FirstOrDefault(x => x.Value.text == b).Key].order);
            return orderA.CompareTo(orderB);
        });


        // 대화 출력 시 우측 바라보기
        npcAnimation.SetFloat("Looking", 0.66f); 
        DialogueManager.Instance.ShowDialogue(linesToUse.ToArray(), npcTextUI, () =>
        {
            if (playerHealth.CurrentHealth == 1)
            {
                // 플레이어 체력 1일 때 npc의 특정 대사 출력
                List<string> phase3Lines = new List<string>();

                foreach (var kvp in dialogueReader.npcDialogue)
                {
                    var dialogue = kvp.Value;
                    if (dialogue.phase == "3")
                        phase3Lines.Add(dialogue.text);
                }
                
                phase3Lines.Sort((a, b) =>
                {
                    var orderA = int.Parse(dialogueReader.npcDialogue
                        [dialogueReader.npcDialogue
                            .FirstOrDefault(x => x.Value.text == a).Key].order);
                    var orderB = int.Parse(dialogueReader.npcDialogue
                        [dialogueReader.npcDialogue
                            .FirstOrDefault(x => x.Value.text == b).Key].order);
                    return orderA.CompareTo(orderB);
                });

                DialogueManager.Instance.ShowDialogue(phase3Lines.ToArray(), npcTextUI, () =>
                {
                    // 플레이어 체력 1 회복
                    playerHealth.PlayerHeal(1);
                    Debug.Log($"주인공 체력 회복, 현재 체력 {playerHealth.CurrentHealth}");

                    timer.SetActive(true);
                    npcAnimation.SetFloat("Looking", 0.33f);
                    enemySpawner.isSpawning = false;
                    descriptionPanel.SetActive(false);
                });
            }
            else
            {
                timer.SetActive(true);
                npcAnimation.SetFloat("Looking", 0.33f);
                descriptionPanel.SetActive(false);
            }
        });
    }
}
