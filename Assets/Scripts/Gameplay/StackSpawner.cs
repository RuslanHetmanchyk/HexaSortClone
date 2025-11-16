using System.Collections.Generic;
using Core.Services.CommandRunner.Interfaces;
using Core.Services.Gameplay.Level.Interfaces;
using UnityEngine;
using Zenject;

namespace Gameplay
{
    public class StackSpawner : MonoBehaviour
    {
        [SerializeField] private HexStackView hexStackPrefab;

        public Transform spawnLine; // позиция основы
        public float spacing = 2.2f;

        private List<HexStackView> stacks = new();

        private ICommandExecutionService commandService;
        private ILevelService levelService;

        [Inject]
        private void Install(
            ICommandExecutionService commandService,
            ILevelService levelService)
        {
            this.commandService = commandService;
            this.levelService = levelService;
        }

        void Start()
        {
            levelService.OnStackSpawned += Spawn;

            levelService.GenerateRandomStacks();
        }

        private void OnDestroy()
        {
            levelService.OnStackSpawned -= Spawn;
        }

        private void Spawn()
        {
            Clear();

            for (var i = 0; i < levelService.GeneratedStacks.Count; i++)
            {
                var startPos = spawnLine.position + new Vector3(i * spacing - spacing, 0f, 0f);
                var stack = Instantiate(hexStackPrefab, startPos, Quaternion.identity);
                stack.SetStartPos(startPos);
                stack.SetCommandRunner(commandService);
                stack.Init(levelService.GeneratedStacks[i]);
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
}