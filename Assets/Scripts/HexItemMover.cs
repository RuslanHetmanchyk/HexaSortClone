using Cysharp.Threading.Tasks;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

public class HexItemMover : MonoBehaviour
{
    public float liftHeight = 0.6f;
    public float moveDuration = 0.35f;
    public float rotateDuration = 0.25f;
    public float squashDuration = 0.15f;

    public Sequence MoveItem(
        Transform item,
        Transform fromStackTop,
        Vector3 toPosition,
        Vector3 sharedEdgeWorldPos)
    {
        Sequence seq = DOTween.Sequence();

        Vector3 startPos = item.position;
        Vector3 peakPos = startPos + Vector3.up * liftHeight;

        // 1. Взлет
        seq.Append(item.DOMove(peakPos, moveDuration).SetEase(Ease.OutQuad));

        // 2. Поворот вокруг ребра
        float lastAngle = 0f;
        seq.Append(
            DOVirtual.Float(0f, 180f, rotateDuration, (angle) =>
            {
                float delta = angle - lastAngle;
                lastAngle = angle;

                item.RotateAround(sharedEdgeWorldPos, Vector3.up, delta);
            }).SetEase(Ease.InOutSine)
        );

        // 3. Перелёт в новую стопку
        seq.Append(item.DOMove(toPosition, moveDuration).SetEase(Ease.InOutQuad));

        // 4. Пух-эффект
        seq.Join(
            item.DOScale(new Vector3(1.15f, 1.15f, 1.15f), 0.15f)
                .SetLoops(2, LoopType.Yoyo)
                .SetEase(Ease.OutQuad)
        );

        return seq;
    }
    
    public TweenerCore<Quaternion, Vector3, QuaternionOptions> FlipHex(Transform hex, Vector3 targetPosition)
    {
        Vector3 P1 = hex.position;
        Vector3 P2 = targetPosition;

        Vector3 pivot = (P1 + P2) / 2f;  
        Vector3 dir = (P2 - P1).normalized;

        Vector3 flipAxis = Vector3.Cross(dir, Vector3.forward).normalized;

        // Родитель для вращения
        GameObject pivotObj = new GameObject("Pivot");
        pivotObj.transform.position = pivot;
        hex.SetParent(pivotObj.transform);

        // Анимация flip-а
        return pivotObj.transform
            .DORotate(flipAxis * 180f, 0.5f, RotateMode.WorldAxisAdd)
            .OnComplete(() =>
            {
                hex.SetParent(null);
                Destroy(pivotObj);
                hex.position = P2; // В конце ложится в позицию второго гекса
            });
    }
    
    public void FlipHex3D(Transform hex, Vector3 targetPosition)
    {
        Vector3 P1 = hex.position;
        Vector3 P2 = targetPosition;

        // Точка ребра между гексами
        Vector3 pivot = (P1 + P2) * 0.5f;

        // Показывает направление от гекса 1 к гексу 2
        Vector3 direction = (P2 - P1).normalized;

        // Ось вращения — параллельная общей стороне гексов
        Vector3 rotationAxis = Vector3.Cross(direction, Vector3.up).normalized;

        // Создаём pivot-объект
        GameObject pivotObj = new GameObject("HexFlipPivot");
        pivotObj.transform.position = pivot;

        hex.SetParent(pivotObj.transform);

        // Анимация вращения вокруг оси (в нужной плоскости!)
        pivotObj.transform
            .DORotate(rotationAxis * 180f, 0.5f, RotateMode.WorldAxisAdd)
            .OnComplete(() =>
            {
                hex.SetParent(null);
                hex.position = P2;

                GameObject.Destroy(pivotObj);
            });
    }

    public void PlayFlipAnimation(Transform hexTransform, Vector3 targetPos, float height = 1.0f)
    {
        Vector3 startPos = hexTransform.position;

        // 1) Midpoint — ось вращения проходит через эту точку (ребро между гексами)
        Vector3 pivotPos = (startPos + targetPos) * 0.5f;

        // 2) Направление между гексами
        Vector3 direction = (targetPos - startPos).normalized;

        // 3D-вверх
        Vector3 up = Vector3.up;

        // 3) Ось вращения — перпендикуляр к направлению
        //Vector3 rotationAxis = Vector3.Cross(direction, up).normalized;

        // ✅ Фикс: вращаться всегда “через верх”
        //if (Vector3.Dot(rotationAxis, up) < 0)
            //rotationAxis = -rotationAxis;

        // 4) Создаём pivot-объект
        GameObject pivotObj = new GameObject("HexFlipPivot");
        pivotObj.transform.position = (startPos + targetPos) * 0.5f;

        // 2) чтобы pivot.forward указывал на центр исходного гекса
        pivotObj.transform.LookAt(startPos, Vector3.up);

        // переносим hex внутрь pivot
        hexTransform.SetParent(pivotObj.transform, true);

        Sequence seq = DOTween.Sequence();

        // 5) Поднимаем гекс вверх перед поворотом
        // seq.Append(
        //     hexTransform.DOMoveY(startPos.y + height, 0.15f)
        // );

        // 6) Сам переворот
        // seq.Append(
        //     pivotObj.transform.DORotate(rotationAxis * 180f, 0.45f, RotateMode.WorldAxisAdd)
        // );
        
        
        
        // float lastAngle = 0f;
        // seq.Append(DOVirtual.Float(0f, -180f, rotateDuration, angle =>
        // {
        //     float delta = angle - lastAngle;
        //     lastAngle = angle;
        //
        //     // вращаем pivot вокруг своей локальной right-оси в мировых координатах
        //     // RotateAround(point, axis, delta) — axis должен быть мировым вектором
        //     Vector3 axisWorld = pivotObj.transform.right; // локальная правая ось в мировых координатах
        //     pivotObj.transform.RotateAround(pivotObj.transform.position, axisWorld, delta);
        // }).SetEase(Ease.InOutSine));
        
        

        Vector3 forward = (startPos - pivotPos).normalized;
        Vector3 rotationAxis = Vector3.Cross(Vector3.up, forward).normalized;

        // 4. угол пойдёт "через верх" → просто берём отрицательный угол как у тебя
        float lastAngle = 0f;

        // 5. собственно вращение
        seq.Append(
            DOVirtual.Float(0f, -180f, 1, angle =>
            {
                float delta = angle - lastAngle;
                lastAngle = angle;

                // вращаем hex вокруг pivotPos на delta градусов
                hexTransform.RotateAround(pivotPos, rotationAxis, delta);
            }).SetEase(Ease.InOutSine)
        );

        // 6. после вращения ставим hex точно в targetPos
        seq.OnComplete(() =>
        {
            hexTransform.position = targetPos;
            // ориентацию можно оставить как есть (она придёт после поворота)
        });

        // 7) Приземление
        // seq.Append(
        //     hexTransform.DOMove(targetPos, 0.15f)
        // );

        // 8) В конце возвращаем hex обратно в нормальную иерархию
        // seq.OnComplete(() =>
        // {
        //     hexTransform.SetParent(null, true);
        //     GameObject.Destroy(pivotObj);
        // });
    }

    public async UniTask PlayMoveAnimation(Transform hexTransform, Vector3 targetPos)
    {
        var tcs = new UniTaskCompletionSource();

        hexTransform
            .DOLocalMove(targetPos, 0.25f)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                tcs.TrySetResult();
            });

        await tcs.Task;
    }

public void DestroyAnimation(Transform hexTransform)
    {
        float scaleDuration = 0.25f;
        float jumpHeight = 0.5f; // Насколько высоко подлетит объект по Y
        float jumpDuration = scaleDuration * 0.5f; // Длительность подскока (половина от общей длительности)

        // Создаем последовательность (Sequence) для комбинирования анимаций
        // Sequence позволяет вам объединять и упорядочивать Tween-ы
        Sequence mySequence = DOTween.Sequence();

        // 1. Анимация масштабирования до нуля
        Tween scaleTween = hexTransform.DOScale(Vector3.zero, scaleDuration)
            .SetEase(Ease.InQuad);

        // 2. Анимация подъема по Y
        // Здесь мы используем .DOJump() для более естественного подскока,
        // но можно и .DOMoveY() для простого подъема.
        // DOJump(endValue, jumpPower, numJumps, duration)
        // Мы хотим, чтобы он просто поднялся, поэтому numJumps = 1
        Tween jumpTween = hexTransform.DOJump(
                hexTransform.position + Vector3.up * jumpHeight, // Конечная позиция (текущая + подъем)
                jumpHeight, // Мощность прыжка (сколько от начальной точки)
                1, // Количество прыжков
                jumpDuration // Длительность прыжка
            )
            .SetEase(Ease.OutSine); // Плавный подъем и быстрое падение

        // Добавляем Tween-ы в последовательность:
        // .Join(tween) запускает этот Tween параллельно с предыдущим
        mySequence.Append(scaleTween) // Сначала добавляем уменьшение
            .Join(jumpTween); // Запускаем подскок одновременно с уменьшением
    }
}