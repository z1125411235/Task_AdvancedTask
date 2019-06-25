using UnityEngine;
using System.Collections;

public class GameLogic
{

    //Calculate experience required to level up
    public static float ExperienceForNextLevel(int currentLevel)
    {
        if (currentLevel == 0) return 0;
        return (currentLevel * currentLevel + currentLevel + 3) * 4;
    }

    /// <summary>
    /// Calculates the player base attack damage.
    /// Base damage equals **STR + 3 bonus damage per 10 STR + 1 bonus damage per 3 DEX**
    /// </summary>
    /// <returns>The player base attack damage.</returns>
    /// <param name="playerController">Player controller.</param>
    public static float CalculatePlayerBaseAttackDamage(PlayerController playerController)
    {
        float baseDamage = playerController.strength + Mathf.Floor(playerController.strength / 10) * 3 + Mathf.Floor(playerController.dexterity / 3);
        return baseDamage;
    }

}
