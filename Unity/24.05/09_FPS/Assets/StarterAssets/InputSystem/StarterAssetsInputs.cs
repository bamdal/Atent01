using Cinemachine;
using System;
using System.Collections;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;
#endif

namespace StarterAssets
{
	public class StarterAssetsInputs : MonoBehaviour
	{
		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool jump;
		public bool sprint;

		[Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;

		/// <summary>
		/// 플레이어를 따라다니는 카메라
		/// </summary>
        CinemachineVirtualCamera followVCAM;

        const float zoomFOV = 20.0f;
        const float normalFOV = 40.0f;

        const float zoomTime = 0.25f;   // 줌이 0.25초에 확대 축소가 이뤄짐

		public Action<bool> onZoom;

		Player player;
        private void Awake()
        {
            player = GetComponent<Player>();	
        }

        private void Start()
        {
            followVCAM = GameManager.Instance.FollowCamara;
        }
        public void OnMove(InputAction.CallbackContext context)
        {
			MoveInput(context.ReadValue<Vector2>());
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            if (cursorInputForLook)
            {
                LookInput(context.ReadValue<Vector2>());
            }
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            JumpInput(context.performed);
        }

        public void OnSprint(InputAction.CallbackContext context)
        {
			SprintInput(context.ReadValue<float>() > 0.1f);
        }

		public void OnZoom(InputAction.CallbackContext context)
		{
			StopAllCoroutines();
			StartCoroutine(ZoomInOut(!context.canceled));	// 줌인아웃 시작
            onZoom.Invoke(!context.canceled);	// 총을 보일지 안보일지 결정
        }

		public void OnFire(InputAction.CallbackContext context)
		{
			if (context.performed)
			{
                player.GunFire(true);
            }
			else if (context.canceled)
			{

				player.GunFire(false);
			}
		}

		public void OnReload(InputAction.CallbackContext context)
		{
			if (context.performed)
			{
				player.RevolverReload();
			}
		}
		int index = 0;

        public void OnWheel(InputAction.CallbackContext context)
		{
			float wheel = context.ReadValue<float>();
			int gunTypemax = Enum.GetValues(typeof(GunType)).Length;
			
			if (wheel > 0)
			{
				index++;
				if(index > gunTypemax - 1)
                    index = 0;
                index = Mathf.Clamp(index, 0, gunTypemax-1);
			}
			if (wheel < 0)
			{
                index--;
				if (index < 0)
					index = gunTypemax - 1;
                index = Mathf.Clamp(index, 0, gunTypemax-1);
            }

			player.GunChage((GunType)index);
			
		}

#if ENABLE_INPUT_SYSTEM
        public void OnMove(InputValue value)
		{
			MoveInput(value.Get<Vector2>());
		}

		public void OnLook(InputValue value)
		{
			if(cursorInputForLook)
			{
				LookInput(value.Get<Vector2>());
			}
		}

		public void OnJump(InputValue value)
		{
			JumpInput(value.isPressed);
		}

		public void OnSprint(InputValue value)
		{
			SprintInput(value.isPressed);
		}
#endif


		public void MoveInput(Vector2 newMoveDirection)
		{
			move = newMoveDirection;
		} 

		public void LookInput(Vector2 newLookDirection)
		{
			look = newLookDirection;
		}

		public void JumpInput(bool newJumpState)
		{
			jump = newJumpState;
		}

		public void SprintInput(bool newSprintState)
		{
			sprint = newSprintState;
		}

		public void ZoomInput()
		{
		
        }

        public void FireInput()
		{

		}

		public void ReloadInput()
		{

		}

		/// <summary>
		/// ZoomIn/Out을 처리하는 코루틴
		/// </summary>
		/// <param name="zoomIn">true확대, false원상복구</param>
		/// <returns></returns>
		IEnumerator ZoomInOut(bool zoomIn)
		{
			float fov = followVCAM.m_Lens.FieldOfView;
			float speed = (normalFOV - zoomFOV) / zoomTime;	// zoomTime실행시간내에 함수끝내기 위한 보정값
			if (zoomIn)
			{
                while (fov != zoomFOV)
                {
                    fov -= Time.deltaTime * speed;
                    followVCAM.m_Lens.FieldOfView = Mathf.Clamp(fov,zoomFOV,normalFOV);
                    yield return null;
                }
            }
			else
			{
                while (fov != normalFOV)
                {
                    fov += Time.deltaTime * speed;
                    followVCAM.m_Lens.FieldOfView = Mathf.Clamp(fov, zoomFOV, normalFOV);
                    yield return null;
                }
            }



        }


/*		IEnumerator ZoomTimeCoroutine(float fov)
		{
			float nowFOV = followVCAM.m_Lens.FieldOfView;
			float elapseTime = 0.0f;
			while (elapseTime < zoomTime)
			{
				elapseTime += Time.deltaTime;
                nowFOV = Mathf.Lerp(nowFOV, fov, zoomTime);
				followVCAM.m_Lens.FieldOfView = nowFOV;
				yield return null;

            }
			followVCAM.m_Lens.FieldOfView = fov;
        }*/
		private void OnApplicationFocus(bool hasFocus)
		{
			SetCursorState(cursorLocked);
		}

		private void SetCursorState(bool newState)
		{
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}
	}
	
}