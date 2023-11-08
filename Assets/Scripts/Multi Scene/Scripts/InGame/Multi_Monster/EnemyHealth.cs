using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json;
using Data;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using Unity.Mathematics;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using DamageNumbersPro;

public enum EnemyType
{
    Boss,
    Monster,
    RedSpider,
    GreenSpider
}
public class EnemyHealth : MonoBehaviour
{
    private NavMeshAgent _nav;
    public EnemyHealthBar enemyHealthBar;
    public float maxHealth;
    public float currentHealth;
    public Animator anim;
    public EnemyType enemyType = EnemyType.Monster;
    public TMP_Text deathText;
    public Image EndingImage;
    
    private static readonly int DoDie = Animator.StringToHash("doDie");
    public DamageNumber damageNumbersPrefab;
    private string currentSceneName;
    public Transform hudPos;
    public ParticleSystem dieEffect;
    public bool isDead = false;

    
    void Start()
    {
        _nav = GetComponent<NavMeshAgent>();
        
    }
    private void Awake()
    {
        currentSceneName= SceneManager.GetActiveScene().name;
        EnemyHealthBaseOnScene(currentSceneName);
    }

    private void EnemyHealthBaseOnScene(string sceneName)
    {
        if (sceneName.Equals("Game Scene"))
        {
            string json = "";
            if (enemyType == EnemyType.Monster)
            {
                json = "{\"EnemyHealth\": 6000, \"Health\": 6000}";
            }
            else if (enemyType == EnemyType.RedSpider)
            {
                json= "{\"EnemyHealth\": 5500, \"Health\": 5500}";
            }
            else if (enemyType == EnemyType.GreenSpider)
            {
                json= "{\"EnemyHealth\": 8000, \"Health\": 8000}";
            }
            else if (enemyType == EnemyType.Boss)
            {
                json = "{\"EnemyHealth\": 50000, \"Health\": 50000}";
            }

            EnemyStat enemyStat1 = JsonConvert.DeserializeObject<EnemyStat>(json);
            maxHealth = (int)enemyStat1.EnemyHealth;
            currentHealth = (int)enemyStat1.Health;
            currentHealth = maxHealth;
            enemyHealthBar = GetComponentInChildren<EnemyHealthBar>();
            DOTween.SetTweensCapacity(500, 50);
        }
    }
    public void TakeDamage(int damage, bool isNetwork = true)
    {
        if (isDead) return;
        
        if ((isNetwork && MultiScene.Instance.isMasterClient) || !isNetwork || enemyType == EnemyType.Boss)
        {
            ApplyDamage(damage, isNetwork);
        }
    }

    private void ApplyDamage(int damage, bool isNetwork)
    {
        float realDamage = Math.Min(damage, currentHealth);
        currentHealth = Math.Max(currentHealth - damage, 0);
        if (enemyHealthBar != null)
        {
            if (enemyType != EnemyType.Boss) enemyHealthBar.UpdateHealth();
            else enemyHealthBar.UpdateBossHealth();
        }

        if (damageNumbersPrefab != null)
        {
            damageNumbersPrefab.Spawn(hudPos.transform.position, realDamage);
        }
        
        if (isNetwork)
        {
            if (enemyType == EnemyType.Boss)
            {
                MultiScene.Instance.BroadCastingEnemyTakeDamage(MultiScene.Instance.enemyList.IndexOf(this.gameObject),
                    damage, true);
            }
            else
            {
                MultiScene.Instance.BroadCastingEnemyTakeDamage(MultiScene.Instance.enemyList.IndexOf(this.gameObject),
                    damage);
            }
        }
        
        if (currentHealth <= 0)
        {
            Die();
            if (enemyType == EnemyType.Boss)
            {
                
                
                
            }
        }
    }
    void Die()
    {
        isDead = true;
        anim.SetTrigger(DoDie);
        MultiScene.Instance.BroadCastingEnemyAnimation(MultiScene.Instance.enemyList.IndexOf(gameObject), DoDie, true);
    }
    void BeginDeath()
    {
        bool isEnemy = gameObject.TryGetComponent(out MultiEnemy enemy);
        bool isBoss = gameObject.TryGetComponent(out MultiBoss boss);
        _nav.isStopped = true;
        
        if (isEnemy)
        {
            enemy.isDead = true;
            dieEffect.Play();
        }
        else if (isBoss)
        {
            boss.isDead = true;
            boss.Stop();
        }
    }

    void EndDeath()
    {
        if (MultiScene.Instance.isMasterClient)
        {
            var index = MultiScene.Instance.GetRandomInt(3);
            var newItem = Instantiate(MultiScene.Instance.itemPrefabs[index], transform.position, quaternion.identity);
            newItem.transform.SetParent(MultiScene.Instance.itemListParent);
            MultiScene.Instance.itemsList.Add(newItem);
            MultiScene.Instance.BroadCastingEnemyItem(transform.position, index);
        }

        MultiScene.Instance.enemyList.Remove(gameObject);
        ReIndexing();
        Destroy(gameObject);
    }

    private void ReIndexing()
    {
        var enemies = MultiScene.Instance.enemyList;

        foreach (var enemy in enemies.Where(enemy => enemy != null))
        {
            enemy.TryGetComponent(out MultiEnemy multiEnemy);
            if(multiEnemy == null) continue;
            multiEnemy.SetIndex();
        }
    }
    
    void BossDeath()
    {
        deathText.DOText("축하합니다 당신은 " + "<color=red>" + "보스" + "</color>" + "를 잡았습니다!", 3, true, ScrambleMode.None, null);
        EndingImage.rectTransform.gameObject.SetActive(true);
    }
}
