using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct PartyMember
{
    public string Name;
    public GameObject prefab;

}

public class PartyManager : MonoBehaviour
{
    public PartyMember[] party;   // from here 
    
    List<Agent> members = new List<Agent>(); // to here

    private void Start()
    {
        var world = FindObjectOfType<HexWorld>();
        SpawnParty(world);
    }

    private void SpawnParty(HexWorld world)
    {
        for (int i = 0; i < party.Length; i++)
        {
            Agent agent = Instantiate(party[i].prefab).GetComponent<Agent>();
            agent.transform.position = world.RandomPosition();

            members.Add(agent);    
        }


    }
}
