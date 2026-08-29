using DesertDash.Core;
using UnityEngine;

namespace DesertDash.Input
{
    public sealed class RunnerInput : MonoBehaviour
    {
        private RunnerConfig _config;
        private Vector2 _gestureStart;
        private bool _trackingGesture;

        public int LaneDelta { get; private set; }
        public bool JumpPressed { get; private set; }
        public bool SlidePressed { get; private set; }
        public bool PausePressed { get; private set; }

        public void Initialize(RunnerConfig config)
        {
            _config = config;
        }

        private void Update()
        {
            LaneDelta = 0;
            JumpPressed = false;
            SlidePressed = false;
            PausePressed = UnityEngine.Input.GetKeyDown(KeyCode.Escape) || UnityEngine.Input.GetKeyDown(KeyCode.P);

            ReadKeyboard();
            ReadTouch();
            ReadMouse();
        }

        private void ReadKeyboard()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.LeftArrow) || UnityEngine.Input.GetKeyDown(KeyCode.A))
            {
                LaneDelta = -1;
            }
            else if (UnityEngine.Input.GetKeyDown(KeyCode.RightArrow) || UnityEngine.Input.GetKeyDown(KeyCode.D))
            {
                LaneDelta = 1;
            }

            JumpPressed = UnityEngine.Input.GetKeyDown(KeyCode.Space) || UnityEngine.Input.GetKeyDown(KeyCode.UpArrow) || UnityEngine.Input.GetKeyDown(KeyCode.W);
            SlidePressed = UnityEngine.Input.GetKeyDown(KeyCode.DownArrow) || UnityEngine.Input.GetKeyDown(KeyCode.S);
        }

        private void ReadTouch()
        {
            if (UnityEngine.Input.touchCount == 0)
            {
                return;
            }

            var touch = UnityEngine.Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                BeginGesture(touch.position);
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                CompleteGesture(touch.position);
            }
        }

        private void ReadMouse()
        {
            if (UnityEngine.Input.touchCount > 0)
            {
                return;
            }

            if (UnityEngine.Input.GetMouseButtonDown(0))
            {
                BeginGesture(UnityEngine.Input.mousePosition);
            }
            else if (UnityEngine.Input.GetMouseButtonUp(0))
            {
                CompleteGesture(UnityEngine.Input.mousePosition);
            }
        }

        private void BeginGesture(Vector2 position)
        {
            _gestureStart = position;
            _trackingGesture = true;
        }

        private void CompleteGesture(Vector2 position)
        {
            if (!_trackingGesture || _config == null)
            {
                return;
            }

            _trackingGesture = false;
            var delta = position - _gestureStart;
            if (delta.magnitude < _config.swipeThresholdPixels)
            {
                return;
            }

            if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
            {
                LaneDelta = delta.x > 0f ? 1 : -1;
            }
            else if (delta.y > 0f)
            {
                JumpPressed = true;
            }
            else
            {
                SlidePressed = true;
            }
        }
    }
}
