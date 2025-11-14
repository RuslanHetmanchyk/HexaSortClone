using System.Collections.Generic;
using Core.Services.CommandRunner.Interfaces;
using UnityEngine;
using Zenject;

public class StackSpawner3D : MonoBehaviour
{
    public Stack3D stackPrefab;

    public Transform spawnLine; // позиция основы
    public float spacing = 2.2f;
    
    List<Stack3D> stacks = new ();

    private ICommandExecutionService commandService;
    
    [Inject]
    private void Install(ICommandExecutionService commandService)
    {
        this.commandService = commandService;
    }

    void Start()
    {
        LevelService.Instance.OnStackSpawned += Spawn;
        
        LevelService.Instance.GenerateRandomStacks();
    }

    private void OnDestroy()
    {
        LevelService.Instance.OnStackSpawned -= Spawn;
    }

    private void Spawn()
    {
        Clear();

        for (int i = 0; i < LevelService.Instance.GeneratedStacks.Count; i++)
        {
            var startPos = spawnLine.position + new Vector3(i * spacing - spacing, 0f, 0f);
            var stack = Instantiate(stackPrefab, startPos, Quaternion.identity);
            stack.SetStartPos(startPos);
            stack.SetCommandRunner(commandService);
            stack.Init(LevelService.Instance.GeneratedStacks[i]);
            stack.SetDraggableActive(true);
            
            stacks.Add(stack);
        }
    }

    private void Clear()
    {
        foreach (var item in stacks)
        {
            if (item.IsDraggable)
            {
                Destroy(item.gameObject);
            }
        }
        
        stacks.Clear();
    }
}