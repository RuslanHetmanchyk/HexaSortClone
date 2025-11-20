using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Gameplay
{
    public class HexItemAnimator : MonoBehaviour
    {
        // --- Настройки движения ---
        private float riseDuration = 0.05f;    // Длительность подъема по Y
        private float travelDuration = 0.15f;  // Длительность горизонтального перемещения

        public async UniTask PlayMoveAnimation(Vector3 targetPosition)
        {
            var tcs = new UniTaskCompletionSource();
            
            // Создаем новую Последовательность
            Sequence moveSequence = DOTween.Sequence();

            if (transform.position.y < targetPosition.y)
            {
                // 1. Добавляем первый этап: Подъем по Y
                moveSequence.Append(
                    MoveUpToNeighborHeight(targetPosition.y)
                );
            }

            // 2. Добавляем второй этап: Горизонтальное движение к цели
            moveSequence.Append(
                MoveToTargetPosition(targetPosition)
            );
            
            if (transform.position.y > targetPosition.y)
            {
                // 1. Добавляем первый этап: Подъем по Y
                moveSequence.Append(
                    MoveUpToNeighborHeight(targetPosition.y)
                );
            }

            // Опционально: действие по завершении
            moveSequence.OnComplete(() => {
                Debug.Log($"Движение гекса {gameObject.name} завершено!");
                tcs.TrySetResult();
            });

            // Запускаем последовательность
            moveSequence.Play();

            await tcs.Task;
        }

        // --- Общий метод (Управление последовательностью) ---

        /// <summary>
        /// Запускает полное перемещение: подъем на высоту соседа, затем движение к цели.
        /// </summary>
        /// <param name="targetPosition">Конечная позиция, куда нужно переместиться (X, Y, Z).</param>
        /// <param name="neighborHeight">Промежуточная высота, на которую нужно подняться (только Y).</param>
        public void StartMoveSequence(Vector3 targetPosition, float neighborHeight)
        {
            // Сначала останавливаем все текущие твины на этом объекте, чтобы избежать наложений
            transform.DOKill(true);

            // Создаем новую Последовательность
            Sequence moveSequence = DOTween.Sequence();

            // 1. Добавляем первый этап: Подъем по Y
            moveSequence.Append(
                MoveUpToNeighborHeight(neighborHeight)
            );

            // 2. Добавляем второй этап: Горизонтальное движение к цели
            moveSequence.Append(
                MoveToTargetPosition(targetPosition)
            );

            // Опционально: действие по завершении
            moveSequence.OnComplete(() => {
                Debug.Log($"Движение гекса {gameObject.name} завершено!");
            });

            // Запускаем последовательность
            moveSequence.Play();
        }

        /// <summary>
        /// Твин для перемещения объекта только по оси Y на заданную высоту.
        /// </summary>
        /// <param name="height">Высота по Y, до которой нужно подняться.</param>
        /// <returns>Созданный объект Tween.</returns>
        private Tween MoveUpToNeighborHeight(float height)
        {
            // Используем DOMoveY для движения только по Y
            return transform.DOMoveY(height, riseDuration)
                .SetEase(Ease.Linear) // Быстрый старт, замедление в конце
                .SetId("RiseTween");   // Добавляем ID для удобства отслеживания/остановки
        }

        /// <summary>
        /// Твин для перемещения объекта в конечную позицию по всем осям.
        /// </summary>
        /// <param name="target">Конечная позиция Vector3.</param>
        /// <returns>Созданный объект Tween.</returns>
        private Tween MoveToTargetPosition(Vector3 target)
        {
            return transform.DOJump(
                    target,             // Конечная позиция
                    1,         // Высота прыжка (насколько высоко он поднимется в середине)
                    1,           // Количество "прыжков" (для нас - 1 арка)
                    travelDuration      // Длительность всего перемещения
                )
                .SetEase(Ease.Linear) // Важно: используем Linear, чтобы прыжок был равномерным
                .SetId("JumpTravelTween");
        }

        public void DestroyAnimation()
        {
            float scaleDuration = 0.25f;
            float jumpHeight = 0.5f; // Насколько высоко подлетит объект по Y
            float jumpDuration = scaleDuration * 0.5f; // Длительность подскока (половина от общей длительности)

            // Создаем последовательность (Sequence) для комбинирования анимаций
            // Sequence позволяет вам объединять и упорядочивать Tween-ы
            Sequence mySequence = DOTween.Sequence();

            // 1. Анимация масштабирования до нуля
            Tween scaleTween = transform.DOScale(Vector3.zero, scaleDuration)
                .SetEase(Ease.InQuad);

            // 2. Анимация подъема по Y
            // Здесь мы используем .DOJump() для более естественного подскока,
            // но можно и .DOMoveY() для простого подъема.
            // DOJump(endValue, jumpPower, numJumps, duration)
            // Мы хотим, чтобы он просто поднялся, поэтому numJumps = 1
            Tween jumpTween = transform.DOJump(
                    transform.position + Vector3.up * jumpHeight, // Конечная позиция (текущая + подъем)
                    jumpHeight, // Мощность прыжка (сколько от начальной точки)
                    1, // Количество прыжков
                    jumpDuration // Длительность прыжка
                )
                .SetEase(Ease.OutSine); // Плавный подъем и быстрое падение

            // Добавляем Tween-ы в последовательность:
            // .Join(tween) запускает этот Tween параллельно с предыдущим
            mySequence.Append(scaleTween) // Сначала добавляем уменьшение
                .Join(jumpTween); // Запускаем подскок одновременно с уменьшением
        
            mySequence.OnComplete(() => Destroy(transform.gameObject));
        }
    }
}