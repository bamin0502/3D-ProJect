using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using UnityEngine.Events;
using System.Security.Cryptography;
using System.Text;

//솔직히 이게임에 암호화까지 필요한가 싶긴한데 그래도 해보는거임
public class PlayerStat
{
    public string name = "";
    public float Level = 0;
    public float Exp = 0;
    public float PlayerHealth = 0;
    public float Health = 0;
}

public class EnemyStat
{
    public float EnemyHealth = 0;
    public float damage = 0;
    public float Health = 0;
}

public class Itemdata
{
    public string itemName = ""; // 아이템의 이름
    public float Health = 0; //아이템 회복량
    public float damage = 0; //아이템 데미지
    public float dot = 0; //아이템 도트데미지
    public float sight = 0; //아이템 시야설정
}

public class WeaponData
{
    public int damage = 0;
}

public class DataManager : MonoBehaviour
{
    public bool isDead { get; set; }
    [System.NonSerialized]
    public UnityEvent deadEvent = new UnityEvent();
    public static DataManager Inst;
    // 암호화에 사용할 키(임시임)
    private readonly string encryptionKey = "EncryptionKey123";

    private static readonly byte[] EncryptionKey = new byte[]
    {
    // 32바이트(256비트) 암호화 키
    0x01, 0x23, 0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF,
    0xFE, 0xDC, 0xBA, 0x98, 0x76, 0x54, 0x32, 0x10,
    0x10, 0x32, 0x54, 0x76, 0x98, 0xBA, 0xDC, 0xFE,
    0xEF, 0xCD, 0xAB, 0x89, 0x67, 0x45, 0x23, 0x01
    };
    private byte[] IV = new byte[]
    { 
    // 16바이트(128비트) 암호화 IV
    0x01, 0x23, 0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF,
    0xFE, 0xDC, 0xBA, 0x98, 0x76, 0x54, 0x32, 0x10
    };

    private void Start()
    {
        SaveData();
    }
    //여기다가 var타입으로 만들고 SaveToJson방식으로 저장시킬거임
    private void InitializeData()
    {
        var playerStat = new PlayerStat
        {
            name = "",
            Level = 1,
            Exp = 0,
            Health = 200,
            PlayerHealth = 200
        };

        var enemyStat1 = new EnemyStat
        {
            EnemyHealth = 50,
            damage = 20,
            Health = 50
        };

        var enemyStat2 = new EnemyStat
        {
            EnemyHealth = 70,
            damage = 10,
            Health = 70
        };

        var enemyStat3 = new EnemyStat
        {
            EnemyHealth = 30,
            damage = 30,
            Health = 30
        };

        var potion = new Itemdata
        {
            itemName = "Potion",
            Health = 30
        };

        var weapon = new WeaponData { damage = 30 };

        SaveToJsonEncrypted(potion, "Itemdata.json");
        SaveToJsonEncrypted(playerStat, "PlayerStat.json");
        SaveToJsonEncrypted(enemyStat1, "EnemyStat1.json");
        SaveToJsonEncrypted(enemyStat2, "EnemyStat2.json");
        SaveToJsonEncrypted(enemyStat3, "EnemyStat3.json");
        SaveToJsonEncrypted(weapon, "WeaponData.json");
    }
    //자동으로 저장시켜줄거임
    private void SaveToJsonEncrypted<T>(T data, string fileName)
    {
        string jsonData = JsonConvert.SerializeObject(data);
        byte[] encryptedData = Encrypt(jsonData);
        string filePath = GetFilePath(fileName);
        File.WriteAllBytes(filePath, encryptedData);
        Debug.Log("Saved encrypted data to: " + filePath);
    }
    //자동으로 로딩시킬거임
    private T LoadFromJsonEncrypted<T>(string fileName)
    {
        string filePath = GetFilePath(fileName);
        if (File.Exists(filePath))
        {
            byte[] encryptedData = File.ReadAllBytes(filePath);
            string jsonData = Decrypt(encryptedData);
            T data = JsonConvert.DeserializeObject<T>(jsonData);
            return data;
        }
        else
        {
            Debug.LogError("파일을 찾을 수 없음!: " + filePath);
            return default(T);
        }
    }
    //해당 코드를 암호화 시킴
    private byte[] Encrypt(string data)
    {
        byte[] rawData = Encoding.UTF8.GetBytes(data);

        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = EncryptionKey;
            aesAlg.IV = IV;

            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, aesAlg.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    csEncrypt.Write(rawData, 0, rawData.Length);
                    csEncrypt.FlushFinalBlock();
                }

                return msEncrypt.ToArray();
            }
        }
    }

    //해당 코드를 복호화 시킬거임
    private string Decrypt(byte[] encryptedData)
    {
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = EncryptionKey;
            aesAlg.IV = IV;
            using (MemoryStream msDecrypt = new MemoryStream(encryptedData))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, aesAlg.CreateDecryptor(), CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        return srDecrypt.ReadToEnd();
                    }
                }
            }
        }
    }
    private byte[] GenerateRandomIV()
    {
        byte[] iv = new byte[16];
        using (RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider())
        {
            rngCsp.GetBytes(iv);
        }
        return iv;
    }

    private string GetFilePath(string fileName)
    {
        return Path.Combine(Application.persistentDataPath, fileName);
    }
    //랜덤 데미지 함수(고정형 데미지가 아닌 랜덤으로 데미지를 줄거임)
    public int GetRandomDamage()
    {
        WeaponData weaponData = LoadFromJsonEncrypted<WeaponData>("WeaponData.json");
        int randAttack = Random.Range(weaponData.damage, weaponData.damage + 10);
        return randAttack;
    }
    //몬스터에게 데미지를 줄 기능
    public void SetEnemyAttack()
    {
        EnemyStat enemyStat = LoadFromJsonEncrypted<EnemyStat>("EnemyStat1.json");
        enemyStat.EnemyHealth -= GetRandomDamage();
        UpdateAfterReceiveEnemyAttack(enemyStat);
    }
    //플레이어가 데미지를 받을 기능
    public void SetPlayerAttack()
    {
        EnemyStat enemyStat = LoadFromJsonEncrypted<EnemyStat>("EnemyStat1.json");
        PlayerStat playerStat = LoadFromJsonEncrypted<PlayerStat>("PlayerStat.json");
        playerStat.PlayerHealth -= enemyStat.damage;
        UpdateAfterReceivePlayerAttack(playerStat);
    }
    //몬스터 데미지 가상함수
    protected virtual void UpdateAfterReceiveEnemyAttack(EnemyStat enemyStat)
    {
        if (enemyStat.EnemyHealth <= 0)
        {
            enemyStat.EnemyHealth = 0;
            isDead = true;
            deadEvent.Invoke();
        }

    }
    //플레이어 데미지 가상함수
    protected virtual void UpdateAfterReceivePlayerAttack(PlayerStat playerStat)
    {
        if (playerStat.PlayerHealth <= 0)
        {
            playerStat.PlayerHealth = 0;
            isDead = true;
            deadEvent.Invoke();
        }
    }
    private void SaveData()
    {
        // PlayerStat 저장
        PlayerStat playerStat = LoadFromJsonEncrypted<PlayerStat>("PlayerStat.json");
        SaveToJsonEncrypted(playerStat, "PlayerStat.json");
        // EnemyStat 저장
        EnemyStat enemyStat1 = LoadFromJsonEncrypted<EnemyStat>("EnemyStat1.json");
        SaveToJsonEncrypted(enemyStat1, "EnemyStat1.json");
        EnemyStat enemyStat2 = LoadFromJsonEncrypted<EnemyStat>("EnemyStat2.json");
        SaveToJsonEncrypted(enemyStat2, "EnemyStat2.json");
        EnemyStat enemyStat3 = LoadFromJsonEncrypted<EnemyStat>("EnemyStat3.json");
        SaveToJsonEncrypted(enemyStat3, "EnemyStat3.json");
        // WeaponData 저장
        WeaponData weaponData = LoadFromJsonEncrypted<WeaponData>("WeaponData.json");
        SaveToJsonEncrypted(weaponData, "WeaponData.json");

        //뒤에 만들것도 위랑 같은 방식으로 만들면 됩니다.

        // 추가적인 데이터 저장 로직을 여기에 구현하면 됩니다.

        Debug.Log("데이터 저장 성공.");
    }
    public void LevelUP()
    {
        //아직 구현 안함 뒤에 한다면 UImanager에 연동하게 할듯
    }
}