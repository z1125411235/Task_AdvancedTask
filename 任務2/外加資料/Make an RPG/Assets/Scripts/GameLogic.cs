using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogic : MonoBehaviour {

	public static float ExperienceForNextLevel (int currentLevel)
    {
        if (currentLevel == 0) return 0;
        return (currentLevel * currentLevel + currentLevel + 3) * 4;
    }

    public static float CalculatePlayerBaseAttackDamage(PlayerController playerController)
    {
        float attackDamege = playerController.strength + Mathf.Floor(playerController.strength / 10) * 3 + Mathf.Floor(playerController.dexterity / 3);
        return attackDamege;
    }

    public static float CalculatePlayerTotalHealth (PlayerController playerController)
    {
        float newHealth = playerController.totalHealth + Mathf.Floor(playerController.totalHealth / 10) * 1 + Mathf.Floor(playerController.strength / 5);
        return newHealth;
    }


    public static float CalculatePlayerTotalStamina(PlayerController playerController)
    {
        float newSP = playerController.totalSP+ Mathf.Floor(playerController.totalSP / 10) * 1 + Mathf.Floor(playerController.dexterity / 10);
        return newSP;
    }
}
