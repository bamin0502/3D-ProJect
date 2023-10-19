using System.Collections;
using UnityEngine;
using Newtonsoft.Json;
using Data;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using Unity.Mathematics;
using UnityEngine.AI;

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
    public int maxHealth;
    public int currentHealth;
    public Animator anim;
    public EnemyType enemyType = EnemyType.Monster;
    public TMP_Text deathText;
    public Image EndingImage;
    public Image KilledImage;
    public TMP_Text kill;
    public TMP_Text death;

    private static readonly int DoDie = Animator.StringToHash("doDie");

    // Start is called before the first frame update
    void Start()
    {
        _nav = GetComponent<NavMeshAgent>();
        
        string json = "";

        if (enemyType == EnemyType.Monster)
        {
            json = "{\"EnemyHealth\": 100, \"Health\": 100}";
        }
        else if (enemyType == EnemyType.RedSpider)
        {
            json= "{\"EnemyHealth\": 70, \"Health\": 70}";
        }
        else if (enemyType == EnemyType.GreenSpider)
        {
            json= "{\"EnemyHealth\": 50, \"Health\": 50}";
        }
        else if (enemyType == EnemyType.Boss)
        {
            json = "{\"EnemyHealth\": 300, \"Health\": 300}";
        }

        EnemyStat enemyStat1 = JsonConvert.DeserializeObject<EnemyStat>(json);
        maxHealth = (int)enemyStat1.EnemyHealth;
        currentHealth = (int)enemyStat1.Health;
        currentHealth = maxHealth;
        enemyHealthBar = GetComponentInChildren<EnemyHealthBar>();
        DOTween.SetTweensCapacity(500, 50);
        
    }

    public void TakeDamage(int damage)
    {
        if (currentHealth <= 0)
        {
            return;     
        }
        if (enemyType == EnemyType.Monster || enemyType == EnemyType.GreenSpider || enemyType == EnemyType.RedSpider)
        {
            currentHealth -= damage;
            enemyHealthBar.UpdateHealth();
        }
        else if(enemyType == EnemyType.Boss)
        {
            currentHealth -= damage;
            enemyHealthBar.UpdateBossHealth();
        }
        if (currentHealth <= 0 )
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
        Debug.Log("Enemy died!");
        bool isEnemy = gameObject.TryGetComponent(out MultiEnemy enemy);
        bool isBoss = gameObject.TryGetComponent(out MultiBoss boss);
        _nav.speed = 0;
        if (isEnemy)
        {
            enemy.isDead = true;
            EndDeath();
        }
        else if (isBoss) {
            boss.isDead = true;
            
        }
        anim.SetTrigger(DoDie);
    }
    
    void EndDeath()
    {
        int index = MultiScene.Instance.GetRandomInt(3);
        var newItem = Instantiate(MultiScene.Instance.itemPrefabs[index], transform.position, quaternion.identity);
        newItem.transform.SetParent(MultiScene.Instance.itemListParent);
        MultiScene.Instance.itemsList.Add(newItem);
        MultiScene.Instance.BroadCastingEnemyItem(transform.position, index);
        Destroy(gameObject);
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
