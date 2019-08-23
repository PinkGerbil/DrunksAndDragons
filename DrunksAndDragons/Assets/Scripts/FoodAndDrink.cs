using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodAndDrink : MonoBehaviour
{
    //the amount by which will increase your health
    public int HealthAddAmount;

    //the amount your stamina regeneration will be boosted by
    public int ReduceAttackCooldown;
    private float AttackCooldownDurationOriginal;
    private bool ReducedAttackCooldown = false;

    public float SpeedModOriginal;
    public float SpeedBoost;
    private bool GinAndSonic = false;

    private float timer;
    public float DurationOfDrink = 5;

    private float GinAndSonicTimer;
    public float GinAndSonicDuration;

    /// <summary>
    /// When the player enters a trigger it will find it's name and do different things depending on the game object's name
    /// </summary>
    /// <param name="other">collider</param>
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "Food")
        {
            PickUpFood(this.gameObject);
        }
        else if(other.gameObject.name == "SpeedDrink")
        {
            PickUpSpeedBoost(this.gameObject);
            PickUpAttackCooldownBoost(this.gameObject);
        }
    }
    
    /// <summary>
    /// Update is called once per frame 
    /// </summary>
    void Update()
    {
        timer += Time.deltaTime;
        GinAndSonicTimer += Time.deltaTime;
        if(GinAndSonicTimer > GinAndSonicDuration && GinAndSonic)
        {
            DeactivateAttackCooldownBoost(this.gameObject);
            DeactivateSpeedBoost(this.gameObject);
        }
    }

    /// <summary>
    /// If the attack cooldown item is picked up it will reduce the cooldown between attacks
    /// </summary>
    /// <param name="player">GameObject being affected</param>
    public void PickUpAttackCooldownBoost(GameObject player)
    {
        timer = 0; 
        if(AttackCooldownDurationOriginal == 0)
        {
            AttackCooldownDurationOriginal = player.GetComponent<AttackScript>().attackCooldownDuration;
        }
        player.GetComponent<AttackScript>().attackCooldownDuration /= 2;
        //ReducedAttackCooldown = true;
    }

    /// <summary>
    /// Reverts the cooldown between attacks to it's original cooldown
    /// </summary>
    /// <param name="player">GameObject being affected</param>
    public void DeactivateAttackCooldownBoost(GameObject player)
    {
        player.GetComponent<AttackScript>().attackCooldownDuration = AttackCooldownDurationOriginal;
        ReducedAttackCooldown = true;
    }

    /// <summary>
    /// If the speedboost item is acquired the player will gain more speed
    /// </summary>
    /// <param name="player">GameObject being affected</param>
    public void PickUpSpeedBoost(GameObject player)
    {
        GinAndSonicTimer = 0;
        if(SpeedModOriginal == 0)
        {
            SpeedModOriginal = player.GetComponent<PlayerMoveScript>().speedMod;
        }
        player.GetComponent<PlayerMoveScript>().speedMod *= SpeedBoost;
        GinAndSonic = true;
    }

    /// <summary>
    /// Changes the speed of the affected game object to be the original modifier
    /// </summary>
    /// <param name="player">GameObject being affected</param>
    public void DeactivateSpeedBoost(GameObject player)
    {
        GinAndSonic = false;
        player.GetComponent<PlayerMoveScript>().speedMod = SpeedModOriginal;
    }

    /// <summary>
    /// Picking up food will restore an amount of health to the player
    /// </summary>
    /// <param name="player">GameObject being affected</param>
    public void PickUpFood(GameObject player)
    {
        player.GetComponent<PlayerDamageHandler>().health += HealthAddAmount;
        if(player.GetComponent<PlayerDamageHandler>().health > player.GetComponent<PlayerDamageHandler>().maxHealth)
        {
            player.GetComponent<PlayerDamageHandler>().health = player.GetComponent<PlayerDamageHandler>().maxHealth;
        }
    }
}
