#region using 선언문들
using System.Collections;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using UnityEngine.Events;
using System.Security.Cryptography;
using System.Text;
using System;
using TMPro;
#endregion

#region 설명
/* 솔직히 이 게임에 암호화까지 필요한가 싶긴한데 그래도 해보는거임
더군다나 이 게임은 뒤에 경쟁게임이여서
기존에는 유니티 에셋내에 json파일의 내용이 보였으나
암호화로 인해서 아마 안보일거임 정상적으로 작동하니 걱정마세요
암호화 방법은 https://gist.github.com/Curookie/5de19e581eb54cff7d7b643408ba930c
의 224줄에 있는 내용을 참고했음 */
#endregion
namespace Data 
{
    #region Json타입 지정, 여기서는 플레이어, 몬스터 , 무기 , 아이템등을 만들었음
    [Serializable]
    public class PlayerStat
    {
        public string name = "";
        public float Level = 0;
        public float Exp = 0;
        public float PlayerHealth = 0;
        public float Health = 0;
    }
    [Serializable]
    public class EnemyStat
    {
        public float EnemyHealth = 0;
        public float damage = 0;
        public float Health = 0;
        public float Heal = 0;
    }
    [Serializable]
    public class Itemdata
    {
        public string itemName = ""; // 아이템의 이름
        public float Health = 0; //아이템 회복량
        public int damage = 0; //아이템 데미지
        public float dot = 0; //아이템 지속시간 설정
        public float sight = 0; //아이템 시야설정
    }
    //[Serializable]
    //public class Itemdata2
    //{
    //    public string itemName = ""; // 아이템의 이름
    //    public float Health = 0; //아이템 회복량
    //    public int damage = 0; //아이템 데미지
    //    public float dot = 0; //아이템 지속시간 설정
    //    public float sight = 0; //아이템 시야설정
    //}
    [Serializable]
    public class WeaponData
    {
        public int damage = 0;
        public int skillDamage = 0;
    }
    #endregion

    public class DataManager : MonoBehaviour
    {
        #region 변수지정
        public bool isDead { get; set; }
        [System.NonSerialized]
        public UnityEvent deadEvent = new UnityEvent();

        public static DataManager Inst;
        [SerializeField]
        private Itemdata[] itemEffects;//아이템 관련 값 지정
        [SerializeField]
        private WeaponData[] weaponEffects;//무기 공격력 값 
        [SerializeField]
        private PlayerStat[] playerStats;//플레이어 스탯 
        [SerializeField]
        private EnemyStat[] enemyStats;//몬스터 스탯
        [SerializeField]
        private SlotToolTip theSlotToolTip;
        [SerializeField]
        private TMP_Text UseItemResultText;
        public string itemName;  // 아이템의 이름(Key값으로 사용할 것)
        private Weapon weapon;
        public MeleeWeapon meleeWeapon;
        #endregion

        #region 32바이트 암호화키
        private static readonly byte[] EncryptionKey = new byte[]
        {
        // 32바이트(256비트) 암호화 키
        0x01, 0x23, 0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF,
        0xFE, 0xDC, 0xBA, 0x98, 0x76, 0x54, 0x32, 0x10,
        0x10, 0x32, 0x54, 0x76, 0x98, 0xBA, 0xDC, 0xFE,
        0xEF, 0xCD, 0xAB, 0x89, 0x67, 0x45, 0x23, 0x01
        };
        #endregion

        #region 16바이트 암호화키
        private byte[] IV = new byte[]
        { 
        // 16바이트(128비트) 암호화 IV
        0x01, 0x23, 0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF,
        0xFE, 0xDC, 0xBA, 0x98, 0x76, 0x54, 0x32, 0x10
        };
        #endregion

        private void Start()
        {
            SaveData();
            InitializeData();

        }
        #region 여기다가 var타입으로 만들고 SaveToJson방식으로 저장시킬거임
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
                Health = 100,
                Heal=20
            };

            var enemyStat2 = new EnemyStat
            {
                EnemyHealth = 300,
                damage = 50,
                Health = 300
            };

            var enemyStat3 = new EnemyStat
            {
                EnemyHealth = 50,
                damage = 40,
                Health = 50
            };

            var Item = new Itemdata
            {
                itemName = "Potion",
                Health = 100,
                damage = 30
            };
            //var adrophine = new Itemdata2 
            //{ 
            //    itemName= "adrophine",
            //    damage=30,
            //    dot=30
            //};

            var weapon = new WeaponData
            {
                damage = 30
            };
            //SaveToJsonEncrypted(adrophine, "Itemdata2.json");
            SaveToJsonEncrypted(Item, "Itemdata.json");
            SaveToJsonEncrypted(playerStat, "PlayerStat.json");
            SaveToJsonEncrypted(enemyStat1, "EnemyStat1.json");
            SaveToJsonEncrypted(enemyStat2, "EnemyStat2.json");
            SaveToJsonEncrypted(enemyStat3, "EnemyStat3.json");
            SaveToJsonEncrypted(weapon, "WeaponData.json");
        }
        #endregion

        #region 암호화저장방식
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
        #endregion

        #region 암호화로드방식
        public T LoadFromJsonEncrypted<T>(string fileName)
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
        #endregion

        #region Json암호화방식
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
        #endregion

        #region Json복호화방식
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
        #endregion

        #region 파일읽어오는 경로 지정 빌드시에도 파일을 읽어올수 있도록 경로를 StreamingAssets으로 지정함
        string GetFilePath(string fileName)
        {
            return Path.Combine(Application.dataPath + "/StreamingAssets", fileName);
        }
        #endregion
        #region 아이템 사용관련 구현
        public virtual bool UseItem(Item _item)
        {
            PlayerStat playerStat = LoadFromJsonEncrypted<PlayerStat>("PlayerStat.json");
            Itemdata itemdata = LoadFromJsonEncrypted<Itemdata>("Itemdata.json");
            WeaponData weaponData = LoadFromJsonEncrypted<WeaponData>("WeaponData.json");
            EnemyStat enemyStat = LoadFromJsonEncrypted<EnemyStat>("EnemyStat1.json");
            MultiPlayerHealth currentPlayerHealth = MultiScene.Instance.currentPlayerHealth;
            
            if (_item.itemType == Item.ItemType.Throw)
            {
                var thrownWeaponController = MultiScene.Instance.currentThrownWeaponController;
                
                if (!thrownWeaponController.isGrenadeMode)
                {
                    thrownWeaponController.isGrenadeMode = true;
                    thrownWeaponController.throwMode = 0;
                    thrownWeaponController.throwRangeIndicator.SetActive(true);
                    thrownWeaponController.damageRangeIndicator.SetActive(true);
                    return true;
                }
            }
            if (_item.itemType == Item.ItemType.Buff)
            {
                currentPlayerHealth.MaxHealth += 500;
                currentPlayerHealth.UpdateHealth();
                MultiScene.Instance.BroadCastingItemUse((int)Item.ItemType.Buff);
                StartCoroutine(DisplayItemMessage("최대 체력이 100 증가하였습니다!"));
                return true;
                #region 일단 무기공격력에다가 연결 실패해서 일단 주석처리하고 나중에 할 예정 임시로 최대체력을 증가시키기로
                //// 아이템으로 인한 공격력 증가
                //weaponData.damage += itemdata.damage;
                //Debug.Log("현재 무기 공격력: " + weaponData.damage + " 아이템으로 인한 공격력 증가량: " + itemdata.damage);

                //StartCoroutine(RemoveBuffAfterDuration((int)itemdata.dot));
                //StartCoroutine(DisplayItemMessage("공격력이 30 증가하였습니다!"));

                //StartCooldown(_item); // 아이템 쿨다운 시작

                //return true;
                #endregion
            }
            if (_item.itemType == Item.ItemType.Used)
            {
                if (currentPlayerHealth.CurrentHealth < currentPlayerHealth.MaxHealth)
                {
                    currentPlayerHealth.CurrentHealth += 500;
                    if (currentPlayerHealth.CurrentHealth >= currentPlayerHealth.MaxHealth)
                    {
                        currentPlayerHealth.CurrentHealth = currentPlayerHealth.MaxHealth;
                    }

                    currentPlayerHealth.UpdateHealth();
                    MultiScene.Instance.BroadCastingItemUse((int)Item.ItemType.Used);
                    StartCoroutine(DisplayItemMessage("체력을 회복했습니다!"));
                    return true;
                }

                return false;
            }            
            return false;
        }
        //각각 원하는 메세지를 코루틴으로 띄운다. 이러면 물약아이템을 사용하고 메세지가 사라지기 전에 사용해도 바뀐다!
        private IEnumerator DisplayItemMessage(string message)
        {
            UseItemResultText.text = message;
            yield return new WaitForSeconds(2);
            UseItemResultText.text = "";
            yield return null;
        }

        //private IEnumerator RemoveBuffAfterDuration(float dot)
        //{
            
        //    yield return new WaitForSeconds(dot);
        //    UseItemResultText.text = "아이템의 효과가 사라졌습니다.";
        //    yield return new WaitForSeconds(2);
        //    UseItemResultText.text = "";
        //    //weaponData.damage -= itemdata.damage;
        //    Debug.Log("버프 지속 시간이 지나서 무기 공격력이 복구되었습니다.");
        //}
        //private IEnumerator ThrowItemEffecter()
        //{

        //    yield return new WaitForSeconds(1);
        //}
        #endregion

        #region var 타입에 따른 데이터저장
        private void SaveData()
        {
            // PlayerStat 저장
            PlayerStat playerStat = LoadFromJsonEncrypted<PlayerStat>("PlayerStat.json");
            Debug.Log("name: " + playerStat.name + ", Level: " + playerStat.Level + ", Exp: " + playerStat.Exp + ", Health: " + playerStat.Health + ", PlayerHealth: " + playerStat.PlayerHealth);
            SaveToJsonEncrypted(playerStat, "PlayerStat.json");

            // EnemyStat 저장
            EnemyStat enemyStat1 = LoadFromJsonEncrypted<EnemyStat>("EnemyStat1.json");
            Debug.Log("EnemyHealth: " + enemyStat1.EnemyHealth + ", Health:" + enemyStat1.Health + ",damage:" + enemyStat1.damage+"Heal:"+enemyStat1.Heal);
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

            //Itemdata 저장
            Itemdata potion = LoadFromJsonEncrypted<Itemdata>("Itemdata.json");
            Debug.Log("회복량 확인:" + potion.Health);
            SaveToJsonEncrypted(potion, "Itemdata.json");
            Itemdata adrophine = LoadFromJsonEncrypted<Itemdata>("Itemdata.json");
            Debug.Log("Damage Increase:" + adrophine.damage);
            SaveToJsonEncrypted(adrophine, "Itemdata.json");
            // 추가적인 데이터 저장 로직을 여기에 구현하면 됩니다.

            Debug.Log("데이터 저장 성공.");
        }
        #endregion

        public void LevelUP()
        {
            //아직 구현 안함 뒤에 한다면 UImanager에 연동하게 할듯
        }

        #region 할지 안할지 모르겠는 스크립트들
        /*
         * 미구현 코드들 아이템이 있는 슬롯에 
         * 마우스를 가져가면 툴팁이 뜨는건데 
         * 할지 말지는 고민중     
        public void ShowToolTip(Item _item, Vector3 _pos)
        {
            theSlotToolTip.ShowToolTip(_item, _pos);
        }
        public void HideToolTip()
        {
            theSlotToolTip.HideToolTip();
        }
        */
        #endregion
        //public void StartCooldown(Item _item)
        //{
        //    _item.cooldownTime = 10; // 쿨타임 시간 설정 (예시로 10초로 설정)
        //    StartCoroutine(CooldownCoroutine(_item));
        //}
        //public IEnumerator CooldownCoroutine(Item _item)
        //{
        //    while (_item.cooldownTime > 0)
        //    {
        //        yield return new WaitForSeconds(1f); // 1초 대기
        //        _item.cooldownTime--;
        //    }
        //}
    }
}

