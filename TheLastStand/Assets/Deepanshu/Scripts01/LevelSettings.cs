using UnityEngine;
[CreateAssetMenu(fileName = "LevelSettings", menuName = "ScriptableObjects/LevelSettings", order = 1)]
public class LevelSettings : ScriptableObject
{
    public int defaultEnemyCount = 10;
    public int defaultEnemyBulletDamage = 10;
    public int defaultPlayerHealth = 100;
    public int defaultPlayerBulletDamage = 20;
    public int defaultHealthCollectableAmount = 20;
    public int defaultAmmoRefillAmount = 30;

    public int enemyCountIncreasePerLevel = 2;
    public int enemyBulletDamageIncreasePerLevel = 1;
    public int playerHealthIncreasePerLevel = 10;
    public int playerBulletDamageIncreasePerLevel = 2;
    public int healthCollectableAmountIncreasePerLevel = 5;
    public int ammoRefillAmountIncreasePerLevel = 5;

    private int currentLevel = 1;

    public void IncreaseDifficulty()
    {
        currentLevel++;
    }

    public int GetEnemyCount()
    {
        return defaultEnemyCount + enemyCountIncreasePerLevel * (currentLevel - 1);
    }

    public int GetEnemyBulletDamage()
    {
        return defaultEnemyBulletDamage + enemyBulletDamageIncreasePerLevel * (currentLevel - 1);
    }

    public int GetPlayerHealth()
    {
        return defaultPlayerHealth + playerHealthIncreasePerLevel * (currentLevel - 1);
    }

    public int GetPlayerBulletDamage()
    {
        return defaultPlayerBulletDamage + playerBulletDamageIncreasePerLevel * (currentLevel - 1);
    }

    public int GetHealthCollectableAmount()
    {
        return defaultHealthCollectableAmount + healthCollectableAmountIncreasePerLevel * (currentLevel - 1);
    }

    public int GetAmmoRefillAmount()
    {
        return defaultAmmoRefillAmount + ammoRefillAmountIncreasePerLevel * (currentLevel - 1);
    }

    public void ResetToDefault()
    {
        currentLevel = 1;
    }
}
