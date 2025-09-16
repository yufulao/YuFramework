// ******************************************************************
//@file         InputManager.cs
//@brief        输入系统
//@author       yufulao, yufulao@qq.com
//@createTime   2024.05.18 01:26:46
// ******************************************************************
using UnityEngine;
using UnityEngine.InputSystem;

namespace Yu
{
    public class InputManager : BaseSingleTon<InputManager>, IMonoManager
    {
        public Vector2 CurrentMovement; //当前的移动输入值
        public bool MovementPressed => CurrentMovement.x != 0 || CurrentMovement.y != 0; //移动是否大于0，无法判断是否摁下摁键，因为a和d一起摁，也是false
        public bool DisableShortcut;
        
        private readonly InputActions _inputActions = new();

        public void OnInit()
        {
            _inputActions.Enable();
            //UI是映射列表, Click是一个InputAction名字
            _inputActions.UI.Click.started += OnMouseLeftClick;
            _inputActions.UI.RightClick.started += OnMouseRightClick;
            _inputActions.UI.Hold.performed += OnHoldBegin;
            _inputActions.UI.Hold.canceled += OnHoldEnd;
            _inputActions.PlayerControl.Movement.performed += outputAction => CurrentMovement = outputAction.ReadValue<Vector2>();
            _inputActions.UI.GM.started += OnGmKeyDown;
            _inputActions.UI.InspirationCatalog.started += OnInspirationKeyDown;
        }

        public void Update()
        {
        }

        public void FixedUpdate()
        {
        }

        public void LateUpdate()
        {
        }

        public void OnClear()
        {
            _inputActions.UI.Click.started -= OnMouseLeftClick;
            _inputActions.UI.RightClick.started -= OnMouseRightClick;
            _inputActions.UI.Hold.performed -= OnHoldBegin;
            _inputActions.UI.Hold.canceled -= OnHoldEnd;
            _inputActions.Disable();
        }

        /// <summary>
        /// 获取鼠标位置
        /// </summary>
        /// <returns></returns>
        public Vector3 GetMousePosition()
        {
            //因为Mouse.current.position.value是ref类型修饰的（什么玩意），lua中不能用，虽然可以强转vec3，但是lua也没有强转
            return Mouse.current == null ? Vector3.zero : Mouse.current.position.value;
        }

        /// <summary>
        /// 获取触屏位置
        /// </summary>
        /// <returns></returns>
        public Vector3 GetTouchPosition()
        {
            return Touchscreen.current == null ? Vector3.zero : Touchscreen.current.position.value;
        }

        /// <summary>
        /// GMView调试界面打开
        /// </summary>
        private void OnGmKeyDown(InputAction.CallbackContext callbackContext)
        {
            if(DisableShortcut)
            {
                return;
            }
            
            EventManager.Instance.Dispatch(EventName.OnGmKeyDown);
        }

        /// <summary>
        /// 灵感图鉴按键按下时
        /// </summary>
        private void OnInspirationKeyDown(InputAction.CallbackContext callbackContext)
        {
            if(DisableShortcut) return;
            EventManager.Instance.Dispatch(EventName.OnInspirationCatalogKeyDown);
        }

        /// <summary>
        /// 鼠标左键点击
        /// </summary>
        private static void OnMouseLeftClick(InputAction.CallbackContext callbackContext)
        {
            EventManager.Instance.Dispatch(EventName.OnMouseLeftClick);
        }

        /// <summary>
        /// 鼠标左键点击
        /// </summary>
        private static void OnMouseRightClick(InputAction.CallbackContext callbackContext)
        {
            EventManager.Instance.Dispatch(EventName.OnMouseRightClick);
        }

        /// <summary>
        /// 鼠标左键长按开始
        /// </summary>
        private static void OnHoldBegin(InputAction.CallbackContext callbackContext)
        {
            EventManager.Instance.Dispatch(EventName.OnHoldBegin);
        }

        /// <summary>
        /// 鼠标左键长按结束
        /// </summary>
        private static void OnHoldEnd(InputAction.CallbackContext callbackContext)
        {
            EventManager.Instance.Dispatch(EventName.OnHoldEnd);
        }

        //键盘事件监听
        // void Update()
        // {
        //     if (Keyboard.current.spaceKey.wasPressedThisFrame)
        //     {
        //         GameLog.Info("空格键按下");
        //     }
        //     if(Keyboard.current.aKey.wasReleasedThisFrame)
        //     {
        //         GameLog.Info("A键抬起");
        //     }
        //     if(Keyboard.current.spaceKey.isPressed)
        //     {
        //         GameLog.Info("空格按下");
        //     }
        //     if(Keyboard.current.anyKey.wasPressedThisFrame)
        //     {
        //         GameLog.Info("任意键按下");
        //     }
        // }


        //鼠标
        // void Update {
        //     if(Mouse.current.rightButton.wasPressedThisFrame)
        // {
        //     GameLog.Info("鼠标右键按下");
        // }
        //
        // if(Mouse.current.middleButton.wasPressedThisFrame)
        // {
        //     GameLog.Info("鼠标中建按下");
        // }
        //
        // if(Mouse.current.forwardButton.wasPressedThisFrame)
        // {
        //     GameLog.Info("鼠标前键按下");
        // }
        //
        // if(Mouse.current.backButton.wasPressedThisFrame)
        // {
        //     GameLog.Info("鼠标后键按下");
        // }
        //
        // //获取鼠标屏幕坐标(左下角为（0,0)
        // GameLog.Info(Mouse.current.position.ReadValue());
        //
        // //两帧之间的偏移
        // GameLog.Info(Mouse.current.delta.ReadValue());
        //
        // //获取鼠标滚轮坐标
        // GameLog.Info(Mouse.current.scroll.ReadValue());


        //触摸屏
        // void Update { Touchscreen ts = Touchscreen. current;
        //     if (ts == null)
        // {
        //     return;
        // }
        //
        // else
        // {
        //     TouchControl tc = ts.touches[0];
        //     if (tc.press.wasPressedThisFrame)
        //     {
        //         GameLog.Info("按下");
        //     }
        //
        //     if (tc.press.wasReleasedThisFrame)
        //     {
        //         GameLog.Info("抬起");
        //     }
        //
        //     if (tc.press.isPressed)
        //     {
        //         GameLog.Info("按住");
        //     }
        //
        //     if (tc.tap.isPressed)
        //     {
        //     }
        //
        //     //点击次数 
        //     GameLog.Info(tc.tapCount);
        //     //点击位置
        //     GameLog.Info(tc.position.ReadValue());
        //     //第一次接触位置
        //     GameLog.Info(tc.startPosition.ReadValue());
        //     //得到的范围
        //     GameLog.Info(tc.radius.ReadValue());
        //     //偏移位置
        //     GameLog.Info(tc.delta.ReadValue());
        //     //返回TouchPhase: None,Began,Moved,Ended,Canceled,Stationary
        //     GameLog.Info(tc.phase.ReadValue());
        //
        //     //判断状态
        //     UnityEngine.InputSystem.TouchPhase tp = tc.phase.ReadValue();
        //     switch (tp)
        //     {
        //         //无
        //         case UnityEngine.InputSystem.TouchPhase.None:
        //             break;
        //         //开始接触
        //         case UnityEngine.InputSystem.TouchPhase.Began:
        //             break;
        //         //移动
        //         case UnityEngine.InputSystem.TouchPhase.Moved:
        //             break;
        //         //结束
        //         case UnityEngine.InputSystem.TouchPhase.Ended:
        //             break;
        //         //取消
        //         case UnityEngine.InputSystem.TouchPhase.Canceled:
        //             break;
        //         //静止
        //         case UnityEngine.InputSystem.TouchPhase.Stationary:
        //             break;
        //     }
        // }


        //手柄
        // Gamepad handle = Gamepad.current;
        //
        //     if(handle==null)
        // {
        //     return;
        // }
        //
        // Vector2 leftDir= handle.leftStick.ReadValue();//左手柄坐标
        // Vector2 rightDir= handle.rightStick.ReadValue();//右手柄坐标
        //
        //     //左摇杆按下抬起
        //     if(Gamepad.current.leftStickButton.wasPressedThisFrame)
        // {
        // }
        // if (Gamepad.current.leftStickButton.wasReleasedThisFrame)
        // {
        // }
        // if (Gamepad.current.leftStickButton.isPressed)
        // {
        // }
        // //右摇杆按下抬起
        // if (Gamepad.current.rightStickButton.wasPressedThisFrame)
        // {
        // }
        // if (Gamepad.current.rightStickButton.wasReleasedThisFrame)
        // {
        // }
        // if (Gamepad.current.rightStickButton.isPressed)
        // {
        // }
        //
        // if(Gamepad.current.dpad.left.wasPressedThisFrame)
        // {
        // }
        // if (Gamepad.current.dpad.left.wasReleasedThisFrame)
        // {
        // }
        // if (Gamepad.current.dpad.left.isPressed)
        // {
        // }
        //
        // //右侧三角方块/XYAB按键
        // //Gamepad.current.buttonEast;
        // //Gamepad.current.buttonWest;
        // //Gamepad.current.buttonSouth;
        // //Gamepad.current.buttonEast;
        // if (Gamepad.current.buttonNorth.wasPressedThisFrame)
        // {
        // }
        // if (Gamepad.current.buttonNorth.wasReleasedThisFrame)
        // {
        // }
        // if (Gamepad.current.buttonNorth.isPressed)
        // {
        // }
        // //手柄中央键
        // if(Gamepad.current.startButton.wasPressedThisFrame)
        // {
        // }
        // if(Gamepad.current.selectButton.wasPressedThisFrame)
        // {
        // }
        // //肩键
        // if(Gamepad.current.leftShoulder.wasPressedThisFrame)
        // {
        // }
        // if (Gamepad.current.rightShoulder.wasPressedThisFrame)
        // {
        // }
        // if(Gamepad.current.leftTrigger.wasPressedThisFrame)
        // {
        // }
        // if(Gamepad.current.rightTrigger.wasPressedThisFrame)
        // {
        // }
    }
}