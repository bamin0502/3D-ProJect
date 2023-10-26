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
    public Canvas canvas;
    private static readonly int DoDie = Animator.StringToHash("doDie");
    public DamageNumber damageNumbersPrefab;
    public RectTransform RectTransform;
    private string currentSceneName;
    // Start is called before the first frame update
    void Start()
    {
        _nav = GetComponent<NavMeshAgent>();
        currentSceneName= SceneManager.GetActiveScene().name;
        EnemyHealthBaseOnScene(currentSceneName);
    }
    private void ShowDamageNumbers(float damageAmount,Vector3 position)
    {
        // DamageNumber damageNumbers = Instantiate(damageNumbersPrefab, canvas.transform);
        // damageNumbers.transform.position = Camera.main.WorldToScreenPoint(position + new Vector3(0, 1f, 0));
        //
        // TextMeshProUGUI damageText = damageNumbers.GetComponentInChildren<TextMeshProUGUI>();
        // if (damageText != null)
        // {
        //     damageText.text = damageAmount.ToString("0");
        // }
        //
        // damageNumbers.transform.rotation = canvas.transform.rotation;

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
        // else if (sceneName.Equals("Single Scene"))
        // {
        //     string json = "";
        //     if (enemyType == EnemyType.Monster)
        //     {
        //         json = "{\"EnemyHealth\": 100, \"Health\": 100}";
        //     }
        //     else if (enemyType == EnemyType.RedSpider)
        //     {
        //         json= "{\"EnemyHealth\": 70, \"Health\": 70}";
        //     }
        //     else if (enemyType == EnemyType.GreenSpider)
        //     {
        //         json= "{\"EnemyHealth\": 50, \"Health\": 50}";
        //     }
        //     else if (enemyType == EnemyType.Boss)
        //     {
        //         json = "{\"EnemyHealth\": 300, \"Health\": 300}";
        //     }
        //
        //     EnemyStat enemyStat1 = JsonConvert.DeserializeObject<EnemyStat>(json);
        //     maxHealth = (int)enemyStat1.EnemyHealth;
        //     currentHealth = (int)enemyStat1.Health;
        //     currentHealth = maxHealth;
        //     enemyHealthBar = GetComponentInChildren<EnemyHealthBar>();
        //     DOTween.SetTweensCapacity(500, 50);
        // }
        
    }
    public void TakeDamage(float damage,Vector3 position)
    {
        if (enemyType == EnemyType.Monster || enemyType == EnemyType.GreenSpider || enemyType == EnemyType.RedSpider)
        {
            currentHealth -= damage;
            enemyHealthBar.UpdateHealth();
            
            ShowDamageNumbers(damage,transform.position);
        }
        else if(enemyType == EnemyType.Boss)
        {
            currentHealth -= damage;
            enemyHealthBar.UpdateBossHealth();
            ShowDamageNumbers(damage,transform.position);
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
        if (isEnemy) enemy.isDead = true;
        else if (isBoss) boss.isDead = true;
        anim.SetTrigger(DoDie);
        MultiScene.Instance.BroadCastingEnemyAnimation(MultiScene.Instance.enemyList.IndexOf(gameObject), DoDie, true);
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
