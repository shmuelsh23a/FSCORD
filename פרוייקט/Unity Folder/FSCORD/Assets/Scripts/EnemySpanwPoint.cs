using UnityEngine;
using System.Collections;

public class EnemySpanwPoint : MonoBehaviour {
	float waveNumber;
	float numberOfTanksPerWave;
    float numberOfTanksSpawnedThiswave;
	public GameObject[] enemyTanks;
    public float timeGapBetweenTankSpawn;
    float lastspawnTime;
    bool spawnTime;
    bool spawnNow;
	// Use this for initialization
	void Start () 
	{
		numberOfTanksPerWave = Game.instance.enemyWaveSize;
		waveNumber = 1;
        lastspawnTime = Time.timeSinceLevelLoad;

	
	}
	
	// Update is called once per frame
	void Update () 
	{

        spawnNow = Game.instance.spawnNow;
        if (((Game.instance.enemyTanksInLevel < 1) && (waveNumber <= Game.instance.totalNumberOfWaves)) || (spawnNow == true))
        {
            if (spawnTime == false)
            {
                numberOfTanksSpawnedThiswave = 0;
                spawnTime = true;
                Game.instance.spawnNow = false;
            }
        }
        /*else
        {
            spawnTime = false;
        }*/

        if ( spawnTime == true)
        { 
            
            if ((numberOfTanksSpawnedThiswave <= numberOfTanksPerWave) && (Time.timeSinceLevelLoad > lastspawnTime + timeGapBetweenTankSpawn))
            //for (int i =0; i <= numberOfTanksPerWave; i++) 
			{
				int tankRandomizer = Random.Range(1, 101);
				if (tankRandomizer < Game.instance.t55Chance)
				{
					GameObject t55  = Instantiate(enemyTanks[0], this.transform.position, Quaternion.identity) as GameObject;
				}

				if ((tankRandomizer > Game.instance.t55Chance) && (tankRandomizer<= Game.instance.t62Chance))
				{
					GameObject t62  = Instantiate(enemyTanks[1], this.transform.position, Quaternion.identity) as GameObject;
				}

				if (tankRandomizer<= Game.instance.t72Chance)
				{
					GameObject t72  = Instantiate(enemyTanks[2], this.transform.position, Quaternion.identity) as GameObject;
				}
                lastspawnTime = Time.timeSinceLevelLoad;
                numberOfTanksSpawnedThiswave++;

			}
			
		}

        if (numberOfTanksSpawnedThiswave >= numberOfTanksPerWave)
        {
            if (spawnTime == true)
            {
                spawnTime = false;
                waveNumber++;
            }
        }
	
	}
}

