using System;
using UnityEngine;
using UnityEngine.XR;

namespace BSDarthMaul
{
    public class DarthMaulBehavior : MonoBehaviour
    {
        private PlayerController _playerController;
        private MainGameSceneSetupData _mainGameSceneSetupData;

        private Transform _head;
        private Vector3 lastLeftPos = new Vector3(0, 0, 0);
        private Vector3 lastRightPos = new Vector3(0, 0, 0);
        private float toggleHoldTime;

        private bool isDarthModeOn = false;
        private bool isOneHanded = false;
        private bool isAutoDetect = false;
        private int separation = 0;

        private void Start()
        {
            try
            {
                Console.WriteLine("Darth Maul Loaded");
                this._playerController = FindObjectOfType<PlayerController>();
                this._head = ReflectionUtil.GetPrivateField<Transform>(_playerController, "_headTransform");
                this.isDarthModeOn = Plugin.IsDarthModeOn;
                this.isOneHanded = Plugin.IsOneHanded;
                this.isAutoDetect = Plugin.IsAutoDetect;
                this.separation = Plugin.Separation;

                var _mainGameSceneSetup = FindObjectOfType<MainGameSceneSetup>();
                _mainGameSceneSetupData = ReflectionUtil.GetPrivateField<MainGameSceneSetupData>(_mainGameSceneSetup, "_mainGameSceneSetupData");

                if (Plugin.IsDarthModeOn && _mainGameSceneSetupData.gameplayMode == GameplayMode.SoloNoArrows)
                {
                    var _beatmapDataModel = ReflectionUtil.GetPrivateField<BeatmapDataModel>(_mainGameSceneSetup, "_beatmapDataModel");
                    var beatmapData = CreateTransformedBeatmapData(_mainGameSceneSetupData.difficultyLevel.beatmapData, _mainGameSceneSetupData.gameplayOptions, _mainGameSceneSetupData.gameplayMode);
                    if (beatmapData != null)
                    {
                        _beatmapDataModel.beatmapData = beatmapData;
                        ReflectionUtil.SetPrivateField(_mainGameSceneSetup, "_beatmapDataModel", _beatmapDataModel);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "\n" + ex.StackTrace);
            }
        }
        private void Update()
        {
            try
            {
                if (_playerController == null)
                {
                    return;
                }

                if (isAutoDetect && !_mainGameSceneSetupData.gameplayOptions.validForScoreUse 
                    && (Input.GetKey((KeyCode)ConInput.Vive.LeftTrigger) || Input.GetKey((KeyCode)ConInput.Vive.RightTrigger)))
                {
                    Vector3 leftHandPos = InputTracking.GetLocalPosition(XRNode.LeftHand);
                    Vector3 rightHandPos = InputTracking.GetLocalPosition(XRNode.RightHand);
                    float leftDist = Vector3.Distance(leftHandPos, lastLeftPos);
                    float rightDist = Vector3.Distance(leftHandPos, lastLeftPos);
                    float leftVelocity = leftDist / Time.deltaTime;
                    float rightVelocity = rightDist / Time.deltaTime;
                    lastLeftPos = leftHandPos;
                    lastRightPos = rightHandPos;
                    Quaternion leftHandRotation = InputTracking.GetLocalRotation(XRNode.LeftHand);
                    Quaternion rightHandRotation = InputTracking.GetLocalRotation(XRNode.RightHand);
                    Vector3 headForward = InputTracking.GetLocalRotation(XRNode.Head) * Vector3.forward;
                    Vector3 leftForward = leftHandRotation * Vector3.forward;
                    Vector3 rightForward = rightHandRotation * Vector3.forward;
                    float leftPointing = Vector3.Cross(headForward, leftForward).y;
                    float rightPointing = Vector3.Cross(headForward, rightForward).y;
                    float angle = Vector3.Angle(leftForward, rightForward);
                    float distance = Vector3.Distance(leftHandPos, rightHandPos);

                    if (isDarthModeOn && angle > 90 && distance > 1.0f)
                    {
                        if (leftPointing < 0 && rightPointing > 0 && leftVelocity > 3.5f && rightVelocity > 3.5f)
                        {
                            toggleHoldTime = 0;
                            ToggleDarthMode(false);
                        }
                    }
                    else if (!isDarthModeOn && angle > 150 && distance < 0.5f)
                    {
                        if (leftPointing < 0 && rightPointing > 0)
                        {
                            toggleHoldTime += Time.deltaTime;
                            if (toggleHoldTime > 0.75f * distance)
                            {
                                toggleHoldTime = 0;
                                ToggleDarthMode(true);
                            }
                        }
                    }
                    else
                    {
                        toggleHoldTime = 0;
                    }
                }

                if (isDarthModeOn)
                {

                    float sep = 1f * separation / 100;
                    if (isOneHanded)
                    {
                        _playerController.leftSaber.transform.parent.transform.localPosition = _playerController.rightSaber.transform.parent.transform.localPosition;
                        _playerController.leftSaber.transform.parent.transform.localRotation = _playerController.rightSaber.transform.parent.transform.localRotation;
                        _playerController.leftSaber.transform.parent.transform.Rotate(0, 180, 180);
                        _playerController.leftSaber.transform.parent.transform.Translate(0, 0, sep * 2, Space.Self);
                    }
                    else
                    {
                        Vector3 leftHandPos = _playerController.leftSaber.transform.parent.position;
                        Vector3 rightHandPos = _playerController.rightSaber.transform.parent.position;
                        Vector3 middlePos = (rightHandPos + leftHandPos) * 0.5f;
                        Vector3 forward = (rightHandPos - leftHandPos).normalized;
                        _playerController.rightSaber.transform.parent.transform.position = middlePos + forward * sep;
                        _playerController.rightSaber.transform.parent.transform.rotation = Quaternion.LookRotation(forward, _playerController.rightSaber.transform.parent.transform.up);
                        _playerController.leftSaber.transform.parent.transform.position = middlePos + -forward * sep;
                        _playerController.leftSaber.transform.parent.transform.rotation = Quaternion.LookRotation(-forward, _playerController.rightSaber.transform.parent.transform.up);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "\n" + ex.StackTrace);
            }
        }

        public static BeatmapData CreateTransformedBeatmapData(BeatmapData beatmapData, GameplayOptions gameplayOptions, GameplayMode gameplayMode)
        {
            BeatmapData beatmapData2 = beatmapData;
            if (gameplayOptions.mirror)
            {
                beatmapData2 = BeatDataMirrorTransform.CreateTransformedData(beatmapData2);
            }
            if (gameplayMode == GameplayMode.SoloNoArrows)
            {
                beatmapData2 = BeatmapDataNoArrowsTransform.CreateTransformedData(beatmapData2);
            }
            if (gameplayOptions.obstaclesOption != GameplayOptions.ObstaclesOption.All)
            {
                beatmapData2 = BeatmapDataObstaclesTransform.CreateTransformedData(beatmapData2, gameplayOptions.obstaclesOption);
            }
            if (beatmapData2 == beatmapData)
            {
                beatmapData2 = beatmapData.GetCopy();
            }
            if (gameplayOptions.staticLights)
            {
                BeatmapEventData[] beatmapEventData = new BeatmapEventData[]
                {
                new BeatmapEventData(0f, BeatmapEventType.Event0, 1),
                new BeatmapEventData(0f, BeatmapEventType.Event4, 1)
                };
                beatmapData2 = new BeatmapData(beatmapData2.beatmapLinesData, beatmapEventData);
            }
            return beatmapData2;
        }

        public void ToggleDarthMode()
        {
            Plugin.IsDarthModeOn = !Plugin.IsDarthModeOn;
            isDarthModeOn = Plugin.IsDarthModeOn;
        }

        public void ToggleDarthMode(bool enable)
        {
            if (enable != Plugin.IsDarthModeOn)
            {
                ToggleDarthMode();
            }
        }

        public void ToggleOneHanded()
        {
            Plugin.IsOneHanded = !Plugin.IsOneHanded;
            isOneHanded = Plugin.IsOneHanded;
        }

        public void ToggleOneHanded(bool enable)
        {
            if (enable != Plugin.IsOneHanded)
            {
                ToggleOneHanded();
            }
        }
    }
}
