using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodAndDrink : MonoBehaviour
{
    //the amount by which will increase your health
    public int HealthAddAmount;

    //the amount your stamina regeneration will be boosted by
    public int ReduceAttackCooldown;
    public float AttackCooldownDurationOriginal;
    public bool ReducedAttackCooldown = false;

    public float SpeedModOriginal;
    public float SpeedBoost;
    public bool SpeedBoosted = false;

    public float timer;
    public float DurationOfDrink = 5;
    
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "Food")
        {
            PickUpFood(this.gameObject);
        }
        else if(other.gameObject.name == "SpeedDrink")
        {
            PickUpSpeedBoost(this.gameObject);
        }
        else if (other.gameObject.name == "CooldownDrink")
        {
            PickUpAttackCooldownBoost(this.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if(timer > DurationOfDrink && ReducedAttackCooldown)
        {
            DeactivateAttackCooldownBoost(this.gameObject);
        }
        if (timer > DurationOfDrink && SpeedBoosted)
        {
            DeactivateSpeedBoost(this.gameObject);
        }
    }

    public void PickUpAttackCooldownBoost(GameObject player)
    {
        timer = 0; 
        if(AttackCooldownDurationOriginal == 0)
        {
            AttackCooldownDurationOriginal = player.GetComponent<AttackScript>().attackCooldownDuration;
        }
        player.GetComponent<AttackScript>().attackCooldownDuration /= 2;
        ReducedAttackCooldown = true;
    }

    public void DeactivateAttackCooldownBoost(GameObject player)
    {
        player.GetComponent<AttackScript>().attackCooldownDuration = AttackCooldownDurationOriginal;
        ReducedAttackCooldown = true;
    }

    public void PickUpSpeedBoost(GameObject player)
    {
        timer = 0;
        if(SpeedModOriginal == 0)
        {
            SpeedModOriginal = player.GetComponent<PlayerMoveScript>().speedMod;
        }
        player.GetComponent<PlayerMoveScript>().speedMod *= SpeedBoost;
        SpeedBoosted = true;
    }

    public void DeactivateSpeedBoost(GameObject player)
    {
        SpeedBoosted = false;
        player.GetComponent<PlayerMoveScript>().speedMod = SpeedModOriginal;
    }

    public void PickUpFood(GameObject player)
    {
        player.GetComponent<PlayerDamageHandler>().health += HealthAddAmount;
    }
}
