using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Level
{
    public class HexGridAnimator : MonoBehaviour
    {
        public float liftHeight = 0.6f;
        public float moveDuration = 0.35f;
        public float rotateDuration = 0.25f;

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
            float jumpHeight = 0.5f;
            float jumpDuration = scaleDuration * 0.5f;

            Sequence mySequence = DOTween.Sequence();

            Tween scaleTween = hexTransform.DOScale(Vector3.zero, scaleDuration)
                .SetEase(Ease.InQuad);

            Tween jumpTween = hexTransform.DOJump(
                    hexTransform.position + Vector3.up * jumpHeight,
                    jumpHeight,
                    1,
                    jumpDuration
                )
                .SetEase(Ease.OutSine);

            mySequence.Append(scaleTween)
                .Join(jumpTween);
        
            mySequence.OnComplete(() => Destroy(hexTransform.gameObject));
        }
    }
}