using System;
using UnityEngine;
using UnityEngine.XR;

namespace BSDarthMaul
{
    public class DarthMaulBehavior : MonoBehaviour
    {
        private PlayerController _playerController;

        private Transform _head;
        private Vector3 lastLeftPos = new Vector3(0, 0, 0);
        private Vector3 lastRightPos = new Vector3(0, 0, 0);
        private float toggleHoldTime;
        private int separation = 0;
        private bool isAutoDetect = false;

        private void Start()
        {
            try
            {
                this._playerController = FindObjectOfType<PlayerController>();
                this._head = ReflectionUtil.GetPrivateField<Transform>(_playerController, "_headTransform");
                this.separation = Plugin.Separation;
                this.isAutoDetect = Plugin.IsAutoDetect;
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

                if (Plugin.IsAutoDetect)
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

                    if (Plugin.IsDarthModeOn && angle > 90 && distance > 1.0f)
                    {
                        if (leftPointing < 0 && rightPointing > 0 && leftVelocity > 3.5f && rightVelocity > 3.5f)
                        {
                            toggleHoldTime = 0;
                            Plugin.ToggleDarthMode(false);
                        }
                    }
                    else if (!Plugin.IsDarthModeOn && angle > 150 && distance < 0.5f)
                    {
                        if (leftPointing < 0 && rightPointing > 0)
                        {
                            toggleHoldTime += Time.deltaTime;
                            if (toggleHoldTime > 0.75f * distance)
                            {
                                toggleHoldTime = 0;
                                Plugin.ToggleDarthMode(true);
                            }
                        }
                    }
                    else
                    {
                        toggleHoldTime = 0;
                    }
                }

                if (Plugin.IsDarthModeOn)
                {

                    float sep = 1f * separation / 100;
                    if (Plugin.IsOneHanded)
                    {
                        _playerController.leftSaber.transform.parent.transform.localPosition = _playerController.rightSaber.transform.parent.transform.localPosition;
                        _playerController.leftSaber.transform.parent.transform.localRotation = _playerController.rightSaber.transform.parent.transform.localRotation;
                        _playerController.leftSaber.transform.parent.transform.Rotate(0, 180, 180);
                        _playerController.leftSaber.transform.parent.transform.Translate(new Vector3(0, 0, sep * 2));
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



        public void ToggleDarthMode()
        {
            Plugin.IsDarthModeOn = !Plugin.IsDarthModeOn;
            //Vector3 leftHandPos = InputTracking.GetLocalPosition(XRNode.LeftHand);
            //Vector3 rightHandPos = InputTracking.GetLocalPosition(XRNode.RightHand);
            //Quaternion leftHandRotation = InputTracking.GetLocalRotation(XRNode.LeftHand);
            //Quaternion rightHandRotation = InputTracking.GetLocalRotation(XRNode.RightHand);
            //_playerController.leftSaber.transform.SetPositionAndRotation(leftHandPos, leftHandRotation);
            //_playerController.rightSaber.transform.SetPositionAndRotation(rightHandPos, rightHandRotation);
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
            //Vector3 rightHandPos = InputTracking.GetLocalPosition(XRNode.RightHand);
            //Quaternion rightHandRotation = InputTracking.GetLocalRotation(XRNode.RightHand);
            //_playerController.rightSaber.transform.SetPositionAndRotation(rightHandPos, rightHandRotation);
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
