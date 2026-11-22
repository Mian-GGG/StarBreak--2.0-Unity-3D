using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public bool levelOne, levelTwo;
    public bool groundLevel, spaceLevel, spaceEnemiesDead = false, isInWareHouse, canEnterPlanet;
    public GameObject enemiesParent;
    public TMPro.TextMeshProUGUI objectivesText;
    private Action levelUpdate;
    public CommanderShip commanderShip;
    private int enemyCount;
    InteractionManager interactionManager;
    GameObject player;

    public void Start()
    {

        

        if (levelOne && groundLevel)
        {
            levelUpdate = LevelOneGround;
        }
        else if(levelOne && spaceLevel)
        {
            levelUpdate = LevelOneSpace;
            player = GameObject.Find("Player");
            interactionManager = player.GetComponent<InteractionManager>();
        }
    }

    public void Update()
    { 
        if(commanderShip  != null)
        {
            spaceEnemiesDead = commanderShip._enemiesAlive;
            enemyCount = commanderShip.enemyCount;
        }
        
        levelUpdate();
    }

    public void LevelOneSpace()
    {
        canEnterPlanet = interactionManager.canEnterPlanet;
        if (spaceEnemiesDead)
        {

            objectivesText.text = "Kill all enemies" + "\n" + enemyCount + " Enemies left" + "\n" + "Current Wave: " + commanderShip._wavesSpawned + "/" + commanderShip._totalWaves; ; 
        }
        else if (!spaceEnemiesDead && !canEnterPlanet)
        {
            objectivesText.text = "Destroy the commander ship";
        }
        else if (canEnterPlanet)
        {
            objectivesText.text = "Land on the planet";
        }
    }

    public void LevelOneGround()
    {
        if(enemiesParent.transform.childCount > 0)
        {
            objectivesText.text = "Kill all enemies" + "\n" + enemiesParent.transform.childCount + " Enemies left";
        }
        else if(enemiesParent.transform.childCount == 0 && !isInWareHouse)
        {
            objectivesText.text = "Find a way to get inside the base";
        }
        else if (isInWareHouse)
        {
            objectivesText.text = "Equip new loadout and get ready for next level";
        }
    }



}
