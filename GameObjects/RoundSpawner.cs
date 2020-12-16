using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundSpawner
{
    //MAX_ENEMIES = 100;
    //MAX_HVT = 5;
    //MAX_LASER_ENEMIES = 10;
    //MAX_SKELETONS = 50;
    //MAX_EXPLODERS = 15;
    //MAX_AOE_ENEMIES = 20;


    //Our Round Spawner - 2D Array
    //keeps track of what enemies will spawn in what round 
    private int[,] roundSpawner; 

    public RoundSpawner()
    {
        // 50 rounds
        //6 different enemies
        roundSpawner = new int[50, 6] //populate each round with specific enemies
            {//0 = regEnemy, 1 = laserEnemy, 2 = HVT, 3 = Skeleton, 4 = Exploder, 5 = AOEEnemy
                {20, 0, 0, 0, 0, 0}, 
                {30, 0, 0, 0, 0, 0},
                {40, 0, 0, 0, 0, 0},
                {50, 2, 0, 0, 0, 0},
                {20, 2, 0, 25, 0, 0},  //every 5th level is skeletons
                {50, 3, 0, 0, 0, 0},
                {50, 3, 0, 0, 0, 0},
                {50, 3, 0, 0, 0, 2}, //welcome to AOE Enemies
                {50, 3, 0, 10, 0, 2}, 
                {50, 2, 1, 5, 0, 2},  //every 10th level spawns a new HVT

                {60, 0, 0, 0, 0, 0}, //after each "10th" level, give the players a bit of rest 
                {60, 3, 0, 5, 5, 2},  //welcome to exploders
                {60, 3, 0, 5, 0, 3},
                {60, 3, 0, 5, 7, 3},
                {30, 2, 0, 35, 3, 3},
                {65, 3, 0, 5, 7, 3},
                {65, 4, 0, 5, 5, 4},
                {65, 3, 0, 5, 6, 4},
                {65, 3, 0, 5, 7, 5},
                {65, 3, 2, 5, 0, 5},

                {70, 0, 0, 0, 0, 0},
                {70, 3, 0, 5, 5, 6},
                {70, 4, 0, 5, 0, 6},
                {70, 4, 0, 5, 5, 6},
                {40, 3, 0, 45, 5, 5},
                {75, 3, 0, 10, 3, 6},
                {75, 3, 0, 7, 3, 7},
                {75, 4, 0, 7, 3, 8},
                {75, 4, 0, 7, 3, 9},
                {75, 5, 3, 10, 5, 10},

                {80, 0, 0, 0, 0, 0},
                {80, 4, 0, 8, 3, 8},
                {80, 4, 0, 10, 4, 8},
                {80, 4, 0, 12, 4, 8},
                {50, 5, 0, 50, 5, 7},
                {85, 5, 0, 8, 5, 8},
                {85, 6, 0, 8, 7, 8},
                {85, 6, 0, 8, 5, 9},
                {85, 7, 0, 8, 5, 10},
                {85, 8, 4, 15, 10, 15},

                {90, 0, 0, 0, 0, 0},  
                {90, 7, 0, 8, 5, 10},
                {90, 7, 0, 10, 7, 10},
                {90, 7, 0, 10, 7, 12},
                {70, 5, 0, 50, 10, 10}, //Final 5 Unique Rounds: CHALLENGE MODE
                {95, 7, 1, 10, 7, 15},
                {95, 7, 2, 10, 8, 15},
                {95, 8, 3, 10, 9, 15},
                {100, 9, 4, 10, 10, 15},
                {100, 10, 5, 30, 15, 20}
            };

        //if players pass all 50 rounds, the rounds will continue at a capped value determined in the MSM, and enemy damage and health will begin to stack 
    }

    //what enemies are spawning next round?
    public int[] getNextRound(int round)
    {
        int[] newRound = new int[7];

        for(int c = 0; c < roundSpawner.GetLength(1); c++)
        {
            newRound[c] = roundSpawner[round,c];
        }

        return newRound;
    }

    //how many enemies spawn this round?
    public int getRoundNumMonsters(int round)
    {
        int numMonsters = 0;

        for (int c = 0; c < roundSpawner.GetLength(1); c++)
        {
            numMonsters += roundSpawner[round, c];
        }

        return numMonsters;
    }
}
