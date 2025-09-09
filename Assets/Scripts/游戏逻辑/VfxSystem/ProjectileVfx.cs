using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PixPlays.ElementalVFX
{
    public class ProjectileVfx : BaseVfx
    {
        [SerializeField] ParticleSystem _CastEffect;
        [SerializeField] ParticleSystem _HitEffect;
        [SerializeField] ParticleSystem _ProjectileEffect;
        [SerializeField] float _FlySpeed = 1;
        [SerializeField] AnimationCurve _FlyCurve;
        [SerializeField] Vector2 _FlyCurveDirection;
        [SerializeField] bool _RandomizeFlyCurveDirection;
        [SerializeField] float _FlyCurveStrength;
        [SerializeField] float _ProjectileFlyDelay;
        [SerializeField] float _ProjectileDeactivateDelay;
        [SerializeField] float _maxFlightDistance = 50f;
        [SerializeField] CollisionDetector _collisionDetector; // 引用碰撞检测器

        private bool _isCollided = false;
        private VfxData vfxData;

        protected override void Start()
        {
            base.Start();
            if (!IsLocal)
            {
                Play(new VfxData());
            }
        }

        public override void Play(VfxData data)
        {
            base.Play(data);
            this.vfxData = data;
            _isCollided = false;
            StartCoroutine(Coroutine_Projectile());
        }

        IEnumerator Coroutine_Projectile()
        {
            // 初始化碰撞检测器
            if (_collisionDetector != null)
            {
                _collisionDetector.Init(_ProjectileEffect.gameObject);

            }

            _CastEffect.gameObject.SetActive(true);
            transform.position = _data.Source;
            _CastEffect.transform.localPosition = Vector3.zero;
            _CastEffect.transform.forward = _data.Target;
            _CastEffect.Play();

            yield return new WaitForSeconds(_ProjectileFlyDelay);
            _ProjectileEffect.gameObject.SetActive(true);
            _ProjectileEffect.transform.position = _CastEffect.transform.position;
            _ProjectileEffect.Play();

            Vector3 flightDirection = _data.Target.normalized;
            _FlyCurveDirection = _FlyCurveDirection.normalized;

            if (_RandomizeFlyCurveDirection)
            {
                _FlyCurveDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
            }

            Vector3 currentPos = _ProjectileEffect.transform.position;
            Vector3 startPos = currentPos;
            float distanceTraveled = 0;

            // 飞行循环中加入碰撞检测
            while (distanceTraveled < _maxFlightDistance && !_isCollided)
            {
                float curveProgress = distanceTraveled / _maxFlightDistance;
                Vector3 curveOffset = (Vector3)_FlyCurveDirection * _FlyCurve.Evaluate(curveProgress) * _FlyCurveStrength;

                currentPos += flightDirection * _FlySpeed * Time.deltaTime;
                currentPos += curveOffset;

                transform.position = currentPos;
                _ProjectileEffect.transform.forward = flightDirection;
                _ProjectileEffect.transform.position = currentPos;

                // 执行碰撞检测
                CheckCollision();

                distanceTraveled = Vector3.Distance(startPos, currentPos);
                yield return null;
            }

            // 播放命中效果
            PlayHitEffect(currentPos, flightDirection);

            _ProjectileEffect.Stop();
            yield return new WaitForSeconds(_ProjectileDeactivateDelay);
            _ProjectileEffect.gameObject.SetActive(false);
        }

        private void CheckCollision()
        {
            if (_collisionDetector == null || _isCollided) return;

            _collisionDetector.Init(ownerCBCtrl.gameObject);
            // 执行碰撞检测
            var detections = _collisionDetector.PerformDetection();

            // 如果检测到碰撞
            if (detections.Count > 0)
            {
                _isCollided = true;
                // 可以在这里获取碰撞点和法线
                var hitInfo = detections[0];
                PlayHitEffect(hitInfo.collisionPoint, hitInfo.collisionNormal);



                ownerCBCtrl.Attack(detections, vfxData.damageInfo.damageFormulaType, vfxData.damageInfo.attackTag);

            }
        }

        private void PlayHitEffect(Vector3 position, Vector3 direction)
        {
            _HitEffect.transform.position = position;
            _HitEffect.transform.forward = direction;
            _HitEffect.gameObject.SetActive(true);
            _HitEffect.Play();
        }

        public override void Stop()
        {
            base.Stop();
            if (gameObject != null)
            {
                _HitEffect.Stop();
                _ProjectileEffect.Stop();
                _CastEffect.Stop();
            }
        }
    }
}