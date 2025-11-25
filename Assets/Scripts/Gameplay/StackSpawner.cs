using System.Collections.Generic;
using Controller;
using Core.Services.Gameplay.Level.Interfaces;
using UnityEngine;
using Zenject;

namespace Gameplay
{
    public class StackSpawner : MonoBehaviour
    {
        public Transform spawnLine; // позиция основы
        public float spacing = 2.2f;

        private readonly List<HexStackView> stacks = new();

        private ILevelService levelService;
        private HexStackViewPool hexStackViewPool;

        [Inject]
        private void Install(
            ILevelService levelService,
            HexStackViewPool hexStackViewPool)
        {
            this.levelService = levelService;
            this.hexStackViewPool = hexStackViewPool;
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
            Despawn();

            for (var i = 0; i < levelService.GeneratedStacks.Count; i++)
            {
                var startPos = spawnLine.position + new Vector3(i * spacing - spacing, 0f, 0f);

                var hexStackView = hexStackViewPool.Spawn(levelService.GeneratedStacks[i]);
                hexStackView.transform.SetParent(transform);
                hexStackView.transform.SetPositionAndRotation(startPos, Quaternion.identity);

                hexStackView.SetStartPos(startPos);
                hexStackView.SetDraggableActive(true);

                stacks.Add(hexStackView);
            }
        }

        private void Despawn()
        {
            foreach (var hexStackView in stacks)
            {
                if (hexStackView.IsDraggable)
                {
                    hexStackView.ForceDespawnAllTopHexItems();
                    hexStackViewPool.Despawn(hexStackView);
                }
            }

            stacks.Clear();
        }
    }
}