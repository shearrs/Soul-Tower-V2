using Shears;
using Shears.Tweens;
using System.Collections;
using UnityEngine;

namespace SoulTower.Traps.UI
{
    public class BoulderTrapAnimation : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private BoulderTrap boulderTrap;
        [SerializeField] private GameObject boulder;
        [SerializeField] private Transform door1;
        [SerializeField] private Transform door2;
        [SerializeField] private Transform boulderSpawnLoc;

        private Vector3 door1StartPos;
        private Vector3 door2StartPos;
        private Quaternion door1StartRot;
        private Quaternion door2StartRot;

        [Header("Settings")]
        [SerializeField] private float doorRotationZ;
        [SerializeField] private float boulderSpawnDelay;

        [Header("Animation Settings")]
        [SerializeField] private TweenData openTweenData;
        [SerializeField] private TweenData shutTweenData;
        [SerializeField] private TweenData shakeTweenData;

        private Tween tween;
        private Tween tween2;

        private void Start()
        {
            door1StartPos = door1.transform.position;
            door1StartRot = door1.transform.rotation;
            door2StartPos = door2.transform.position;
            door2StartRot = door2.transform.rotation;
        }

        private void OnEnable()
        {
            boulderTrap.Activated += OnBoulderActivated;
        }

        private void OnDisable()
        {
            boulderTrap.Activated -= OnBoulderActivated;
        }

        private void OnBoulderActivated(Trap _)
        {
            StartCoroutine(SpawnBoulder());
            tween.Dispose();
            tween2.Dispose();

            tween = door1.DoShakeTween(.025f, .02f, shakeTweenData);
            tween2 = door2.DoShakeTween(.025f, .02f, shakeTweenData);

            tween.Completed += OpenDoors;
        }

        private void OpenDoors()
        {
            tween.Dispose();
            tween2.Dispose();

            door1.transform.position = door1StartPos;
            door1.transform.rotation = door1StartRot;
            door2.transform.position = door2StartPos;
            door2.transform.rotation = door2StartRot;

            tween = door1.DoRotateLocalTween(Quaternion.Euler(new Vector3(0f, 0f, doorRotationZ)), true, openTweenData);
            tween2 = door2.DoRotateLocalTween(Quaternion.Euler(new Vector3(0f, 0f, doorRotationZ * -1)), true, openTweenData);

            tween.Completed += CloseDoors;
        }

        private void CloseDoors()
        {
            tween.Dispose();
            tween2.Dispose();

            tween = door1.DoRotateLocalTween(Quaternion.identity, true, shutTweenData);
            tween2 = door2.DoRotateLocalTween(Quaternion.identity, true, shutTweenData);

        }

        IEnumerator SpawnBoulder()
        {
            yield return CoroutineUtil.WaitForSeconds(boulderSpawnDelay);

            GameObject spawnedBoulder = Instantiate(boulder);
            spawnedBoulder.transform.position = boulderSpawnLoc.transform.position;
        }
    }
}
