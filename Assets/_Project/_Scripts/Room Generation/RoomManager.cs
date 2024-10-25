using UnityEngine;
using Utility;

public class RoomManager : Singleton<RoomManager>
{
    private GameObject[] roomPrefabs = new GameObject[4];
    private GameObject shopPrefab;
    private GameObject bossPrefab;

    private RoomType[] rooms = new RoomType[10];
    private IEraGenerationStrategy eraGenerationStrategy;

    //We dont need this in room manager, just for testing generating a era
    override protected void Awake()
    {
        RandomGen.Initialize();

        base.Awake();
    }

    public void SetEraGenerationStrategy(IEraGenerationStrategy strategy)
    {
        eraGenerationStrategy = strategy;
    }

    //I'll change this up later, just to test out generating a classical era
    private void GenerateEra()
    {
        SetEraGenerationStrategy(new ClassicalGenerationStrategy());

        rooms = eraGenerationStrategy.GenerateEra(rooms);
    }

    private void GenerateRoom(int index)
    {
        //Actually instantiate the room
    }
}
