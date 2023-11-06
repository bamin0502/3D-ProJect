using System.Collections;
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
using DamageNumbersPro.Internal;

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
    public Image KilledImage;
    public TMP_Text kill;
    public TMP_Text death;
    private static readonly int DoDie = Animator.StringToHash("doDie");
    public DamageNumber damageNumbersPrefab;
    private string currentSceneName;
    public Transform hudPos;
    public ParticleSystem dieEffect;

    public bool isDead = false;
    
    
    void Start()
    {
        _nav = GetComponent<NavMeshAgent>();
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
                json = "{\"EnemyHealth\": 1000, \"Health\": 1000}";
            }
            else if (enemyType == EnemyType.RedSpider)
            {
                json= "{\"EnemyHealth\": 700, \"Health\": 700}";
            }
            else if (enemyType == EnemyType.GreenSpider)
            {
                json= "{\"EnemyHealth\": 500, \"Health\": 500}";
            }
            else if (enemyType == EnemyType.Boss)
            {
                json = "{\"EnemyHealth\": 30000, \"Health\": 30000}";
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
        
        if ((isNetwork && MultiScene.Instance.isMasterClient) || !isNetwork)
        {
            ApplyDamage(damage, isNetwork);
        }
    }

    private void ApplyDamage(int damage, bool isNetwork)
    {
        currentHealth -= damage;
        
        if (enemyHealthBar != null)
        {
            if (enemyType != EnemyType.Boss) enemyHealthBar.UpdateHealth();
            else enemyHealthBar.UpdateBossHealth();
        }

        if (damageNumbersPrefab != null)
        {
            damageNumbersPrefab.Spawn(hudPos.transform.position, damage);
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
                StartCoroutine(BossKill());
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
        else if (isBoss) boss.isDead = true;
    }

    void EndDeath()
    {
        if (MultiScene.Instance.isMasterClient)
        {
            int index = MultiScene.Instance.GetRandomInt(3);
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

        foreach (GameObject enemy in enemies)
        {
            if(enemy == null) continue;
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
    IEnumerator BossKill()
    {
        KilledImage.rectTransform.gameObject.SetActive(true);
        kill.text = "Player";
        death.text = "Boss";
        BossDeath();

        yield return new WaitForSeconds(10f);
        
        KilledImage.DOFade(0f, 1f).OnComplete(() =>
        {
            KilledImage.rectTransform.gameObject.SetActive(false);
        });
       
    }
}
