using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using UnityEngine.Events;
using System.Security.Cryptography;
using System.Text;
using System;
using Sirenix.OdinInspector;

//솔직히 이 게임에 암호화까지 필요한가 싶긴한데 그래도 해보는거임 더군다나 이 게임은 뒤에 경쟁게임이여서
//기존에는 유니티 에셋내에 json파일의 내용이 보였으나 암호화로 인해서 아마 안보일거임 정상적으로 작동하니 걱정마세요
//암호화 방법은 https://gist.github.com/Curookie/5de19e581eb54cff7d7b643408ba930c 의 224줄에 있는 내용을 참고했음
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

public class DataManager : SerializedMonoBehaviour
{
    public bool isDead { get; set; }
    [System.NonSerialized]
    public UnityEvent deadEvent = new UnityEvent();
    public static DataManager Inst;

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
        InitializeData();
    }
    //여기다가 var타입으로 만들고 SaveToJson방식으로 저장시킬거임
    private void InitializeData()
    {
        var playerStat = new PlayerStat
        {
            name = "",
            Level = 1,
            Exp = 0,
            Health = 1000,
            PlayerHealth = 1000
        };

        var enemyStat1 = new EnemyStat
        {
            EnemyHealth = 100,
            damage = 30,
            Health = 100
        };

        var enemyStat2 = new EnemyStat
        {
            EnemyHealth = 70,
            damage = 20,
            Health = 70
        };

        var enemyStat3 = new EnemyStat
        {
            EnemyHealth = 50,
            damage = 40,
            Health = 50
        };

        var potion = new Itemdata
        {
            itemName = "Potion",
            Health = 30
        };

        var weapon = new WeaponData { damage = 30 };
        //이거는 일단 비활성화(inventory는 Json을 안사용할수도 있음)
        //SaveToJsonEncrypted(potion, "Itemdata.json");

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

        // 암호화된 데이터의 길이를 저장
        int encryptedDataLength = encryptedData.Length;

        // 저장할 데이터 배열 생성 (길이 정보 + 암호화된 데이터)
        byte[] savedData = new byte[4 + encryptedDataLength];

        // 길이 정보를 바이트 배열로 변환하여 저장
        byte[] lengthBytes = BitConverter.GetBytes(encryptedDataLength);
        Buffer.BlockCopy(lengthBytes, 0, savedData, 0, 4);

        // 암호화된 데이터를 저장
        Buffer.BlockCopy(encryptedData, 0, savedData, 4, encryptedDataLength);

        string filePath = GetFilePath(fileName);
        File.WriteAllBytes(filePath, savedData);
        //Debug.Log("암호화된 데이터를 저장했습니다: " + filePath);
    }
    //자동으로 로딩시킬거임
    private T LoadFromJsonEncrypted<T>(string fileName)
    {
        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
        string filePath = GetFilePath(fileName);
        byte[] savedData = File.ReadAllBytes(filePath);

        if (savedData.Length < 4)
        {
            Debug.LogError("암호화된 데이터의 길이 정보가 올바르지 않습니다.");
            return default(T);
        }

        // 저장된 데이터 배열에서 길이 정보를 읽어옴
        int encryptedDataLength = BitConverter.ToInt32(savedData, 0);

        if (encryptedDataLength != savedData.Length - 4)
        {
            Debug.LogError("암호화된 데이터의 길이가 올바르지 않습니다.");
            return default(T);
        }

        // 길이 정보를 제외한 실제 암호화된 데이터 배열 생성
        byte[] encryptedData = new byte[encryptedDataLength];
        Buffer.BlockCopy(savedData, 4, encryptedData, 0, encryptedDataLength);

        string decryptedJsonData = Decrypt(encryptedData); // 복호화된 JSON 데이터를 가져옵니다.
        T data = JsonConvert.DeserializeObject<T>(decryptedJsonData);
        return data;
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

                    byte[] encryptedData = msEncrypt.ToArray();
                    return encryptedData;
                }
            }
        }
    }

    //해당 코드를 다시 복호화 시킬거임
    private string Decrypt(byte[] encryptedData)
    {
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = EncryptionKey;
            aesAlg.IV = IV;
            aesAlg.IV = IV;

            using (MemoryStream msDecrypt = new MemoryStream())
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV), CryptoStreamMode.Write))
                {
                    csDecrypt.Write(encryptedData, 0, encryptedData.Length);
                    csDecrypt.FlushFinalBlock(); // Flush the final block to the memory stream
                }

                byte[] decryptedBytes = msDecrypt.ToArray();
                return Encoding.UTF8.GetString(decryptedBytes).TrimEnd('\0'); // Remove padding characters
            }
        }
    }
    //랜덤 암호화키 발급
    //private byte[] GenerateRandomIV()
    //{
    //    byte[] iv = new byte[16];
    //    using (RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider())
    //    {
    //        rngCsp.GetBytes(iv);
    //    }
    //    return iv;
    //}
    //파일을 읽어오는 기능임
    private string GetFilePath(string fileName)
    {
        return Path.Combine("Assets/Resources/Data", fileName);
    }
    //랜덤 데미지 함수(고정형 데미지가 아닌 랜덤으로 데미지를 줄거임)
    public int GetWeaponRandomDamage()
    {
        WeaponData weaponData = LoadFromJsonEncrypted<WeaponData>("WeaponData.json");
        int randAttack = UnityEngine.Random.Range(weaponData.damage, weaponData.damage + 10);
        return randAttack;
    }

    //몬스터에게 데미지를 줄 기능
    public void SetEnemyAttack()
    {
        EnemyStat enemyStat = LoadFromJsonEncrypted<EnemyStat>("EnemyStat1.json");
        enemyStat.EnemyHealth -= GetWeaponRandomDamage();
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
        Debug.Log("name: " + playerStat.name + ", Level: " + playerStat.Level + ", Exp: " + playerStat.Exp + ", Health: " + playerStat.Health + ", PlayerHealth: " + playerStat.PlayerHealth);
        SaveToJsonEncrypted(playerStat, "PlayerStat.json");
        // EnemyStat 저장
        EnemyStat enemyStat1 = LoadFromJsonEncrypted<EnemyStat>("EnemyStat1.json");
        Debug.Log("EnemyHealth: " + enemyStat1.EnemyHealth + ", Health:" + enemyStat1.Health + ",damage:" + enemyStat1.damage);
        SaveToJsonEncrypted(enemyStat1, "EnemyStat1.json");
        EnemyStat enemyStat2 = LoadFromJsonEncrypted<EnemyStat>("EnemyStat2.json");
        Debug.Log("EnemyHealth: " + enemyStat2.EnemyHealth + ", Health:" + enemyStat2.Health + ",damage:" + enemyStat2.damage);
        SaveToJsonEncrypted(enemyStat2, "EnemyStat2.json");
        EnemyStat enemyStat3 = LoadFromJsonEncrypted<EnemyStat>("EnemyStat3.json");
        Debug.Log("EnemyHealth: " + enemyStat3.EnemyHealth + ", Health:" + enemyStat3.Health + ",damage:" + enemyStat3.damage);
        SaveToJsonEncrypted(enemyStat3, "EnemyStat3.json");
        // WeaponData 저장
        WeaponData weaponData = LoadFromJsonEncrypted<WeaponData>("WeaponData.json");
        Debug.Log("WeaponDamage:" + weaponData.damage);
        SaveToJsonEncrypted(weaponData, "WeaponData.json");

        // 추가적인 데이터 저장 로직을 여기에 구현하면 됩니다.

        Debug.Log("데이터 저장 성공.");
    }
    public void LevelUP()
    {
        //아직 구현 안함 뒤에 한다면 UImanager에 연동하게 할듯
    }
}