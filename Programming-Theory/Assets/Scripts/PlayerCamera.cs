using System.Collections;
using System.Collections.Generic;
using UnityEngine;

<<<<<<< Updated upstream
public enum CameraMode
{
	FIRSTPERSONCAMERA,
	THIRDPERSONBEHIND,	//camera is behind player looking forward
	THIRDPERSONINFRONT	//camera is in front of player looking backward
}


public class PlayerCamera : MonoBehaviour {
=======
>>>>>>> Stashed changes

public enum CameraMode
{
    FIRSTPERSONCAMERA,
    THIRDPERSONBEHIND,    //camera is behind player looking forward
    THIRDPERSONINFRONT    //camera is in front of player looking backward
}

public class PlayerCamera : MonoBehaviour {
    private bool canSwap = true;

    private Transform _firstPersonPosition;
    private CameraMode _previousMode = CameraMode.THIRDPERSONBEHIND;
	

    public static Transform Target { get; set; } // ENCAPSULATION
	public static float Distance { get; set; } // ENCAPSULATION
	public static float Height { get; set; } // ENCAPSULATION
<<<<<<< Updated upstream
	public static CameraMode CameraMode { get; set; }
=======
    public static CameraMode CameraMode { get; set; }
>>>>>>> Stashed changes


    [SerializeField] private float _damping = 6f;
	[SerializeField] private float _rotationDamping = 10f;
	[SerializeField] private bool _smoothRotation = true;
	[SerializeField] private bool _followBehind = true;

	private bool canFollow = false;
	private bool canSwap = true;

	private Transform _firstPersonPosition;
	private CameraMode _previousMode = CameraMode.THIRDPERSONBEHIND;

	

	public static void ChangeCamPrefs(Transform vehicle, float dist, float height) { // ABSTRACTION
		Target = vehicle;
		Distance = dist;
		Height = height;

	}

	private void Awake() {
		CameraMode = CameraMode.THIRDPERSONBEHIND;
		StartCoroutine(WaitForTransition());
        CameraMode = CameraMode.THIRDPERSONBEHIND;
    }
    private IEnumerator CameraSwapCooldown()
    {
        canSwap = false;

<<<<<<< Updated upstream
	private void FixedUpdate() {
		if(canSwap)
		{
			if(Input.GetKey(KeyCode.Q))
			{
                switch (CameraMode)
				{
					case CameraMode.FIRSTPERSONCAMERA:
						break;
					case CameraMode.THIRDPERSONBEHIND:
						CameraMode = CameraMode.THIRDPERSONINFRONT;
						//_damping = 6f;
						StartCoroutine(CameraSwapCooldown());
						break;
					case CameraMode.THIRDPERSONINFRONT:
						CameraMode = CameraMode.THIRDPERSONBEHIND;
						//_damping = 0f;
						StartCoroutine(CameraSwapCooldown());
						break;
					default:
						break;
				}

				print(Target.gameObject);
			}


			if(Input.GetKey(KeyCode.LeftControl))
			{
				switch (CameraMode)
				{
					case CameraMode.FIRSTPERSONCAMERA:
						CameraMode = _previousMode;
						StartCoroutine(CameraSwapCooldown());
						break;
					case CameraMode.THIRDPERSONBEHIND:
                        if (_firstPersonPosition == null)
                        {
                            _firstPersonPosition = Target.gameObject.GetComponent<CarController>()._firstPersonCameraPosition;
                        }
						_previousMode = CameraMode;
						CameraMode = CameraMode.FIRSTPERSONCAMERA;
						StartCoroutine(CameraSwapCooldown());
                        break;
					case CameraMode.THIRDPERSONINFRONT:
                        if (_firstPersonPosition == null)
                        {
                            _firstPersonPosition = Target.gameObject.GetComponent<CarController>()._firstPersonCameraPosition;
                        }
						_previousMode = CameraMode;
						CameraMode = CameraMode.FIRSTPERSONCAMERA;
						StartCoroutine(CameraSwapCooldown());
                        break;
					default:
						break;
				}
			}
		}

		if (canFollow) {

			Vector3 wantedPosition;
			switch (CameraMode)
			{
				case CameraMode.FIRSTPERSONCAMERA:
                    transform.position = Vector3.Lerp(transform.position, _firstPersonPosition.localToWorldMatrix.GetPosition(), 1);// Time.deltaTime * _damping);

                    if (_smoothRotation)
                    {
                        //Quaternion wantedRotation = Quaternion.LookRotation(Target.position - transform.position, Target.up);
                        transform.rotation = Quaternion.Slerp(transform.rotation, _firstPersonPosition.localToWorldMatrix.rotation, Time.deltaTime * _rotationDamping);
                    }
                    else transform.LookAt(Target, Target.up);
                    break;
				case CameraMode.THIRDPERSONBEHIND:
					if (_followBehind)
						wantedPosition = Target.TransformPoint(0, Height, -Distance);
					else
						wantedPosition = Target.TransformPoint(0, Height, Distance);

					transform.position = Vector3.Lerp(transform.position, wantedPosition, Time.deltaTime * _damping);

					if (_smoothRotation) {
						Quaternion wantedRotation = Quaternion.LookRotation(Target.position - transform.position, Target.up);
						transform.rotation = Quaternion.Slerp(transform.rotation, wantedRotation, Time.deltaTime * _rotationDamping);
					} else transform.LookAt(Target, Target.up);
					break;
				case CameraMode.THIRDPERSONINFRONT:
                    if (_followBehind)
                        wantedPosition = Target.TransformPoint(0, Height, Distance);
                    else
                        wantedPosition = Target.TransformPoint(0, Height, -Distance);

					transform.position = Vector3.Lerp(transform.position, wantedPosition, 1);// Time.deltaTime * _damping);

                    if (_smoothRotation)
                    {
                        Quaternion wantedRotation = Quaternion.LookRotation(Target.position - transform.position, Target.up);
                        transform.rotation = Quaternion.Slerp(transform.rotation, wantedRotation, Time.deltaTime * _rotationDamping);
                    }
                    else transform.LookAt(Target, Target.up);
                    break;
				default:
					break;
			}

		}
	}
=======
        yield return new WaitForSeconds(0.5f);

        canSwap = true;
    }
    private void FixedUpdate()
    {
        if (canSwap)
        {
            if (Input.GetKey(KeyCode.Q))
            {
                switch (CameraMode)
                {
                    case CameraMode.FIRSTPERSONCAMERA:
                        break;
                    case CameraMode.THIRDPERSONBEHIND:
                        CameraMode = CameraMode.THIRDPERSONINFRONT;
                        //_damping = 6f;
                        StartCoroutine(CameraSwapCooldown());
                        break;
                    case CameraMode.THIRDPERSONINFRONT:
                        CameraMode = CameraMode.THIRDPERSONBEHIND;
                        //_damping = 0f;
                        StartCoroutine(CameraSwapCooldown());
                        break;
                    default:
                        break;
                }

                print(Target.gameObject);
            }
>>>>>>> Stashed changes


            if (Input.GetKey(KeyCode.LeftControl))
            {
                switch (CameraMode)
                {
                    case CameraMode.FIRSTPERSONCAMERA:
                        CameraMode = _previousMode;
                        StartCoroutine(CameraSwapCooldown());
                        break;
                    case CameraMode.THIRDPERSONBEHIND:
                        if (_firstPersonPosition == null)
                        {
                            _firstPersonPosition = Target.gameObject.GetComponent<CarController>()._firstPersonCameraPosition;
                        }
                        _previousMode = CameraMode;
                        CameraMode = CameraMode.FIRSTPERSONCAMERA;
                        StartCoroutine(CameraSwapCooldown());
                        break;
                    case CameraMode.THIRDPERSONINFRONT:
                        if (_firstPersonPosition == null)
                        {
                            _firstPersonPosition = Target.gameObject.GetComponent<CarController>()._firstPersonCameraPosition;
                        }
                        _previousMode = CameraMode;
                        CameraMode = CameraMode.FIRSTPERSONCAMERA;
                        StartCoroutine(CameraSwapCooldown());
                        break;
                    default:
                        break;
                }
            }
        }

        if (canFollow)
        {

            Vector3 wantedPosition;
            switch (CameraMode)
            {
                case CameraMode.FIRSTPERSONCAMERA:
                    transform.position = Vector3.Lerp(transform.position, _firstPersonPosition.localToWorldMatrix.GetPosition(), 1);// Time.deltaTime * _damping);

                    if (_smoothRotation)
                    {
                        //Quaternion wantedRotation = Quaternion.LookRotation(Target.position - transform.position, Target.up);
                        transform.rotation = Quaternion.Slerp(transform.rotation, _firstPersonPosition.localToWorldMatrix.rotation, Time.deltaTime * _rotationDamping);
                    }
                    else transform.LookAt(Target, Target.up);
                    break;
                case CameraMode.THIRDPERSONBEHIND:
                    if (_followBehind)
                        wantedPosition = Target.TransformPoint(0, Height, -Distance);
                    else
                        wantedPosition = Target.TransformPoint(0, Height, Distance);

                    transform.position = Vector3.Lerp(transform.position, wantedPosition, Time.deltaTime * _damping);

                    if (_smoothRotation)
                    {
                        Quaternion wantedRotation = Quaternion.LookRotation(Target.position - transform.position, Target.up);
                        transform.rotation = Quaternion.Slerp(transform.rotation, wantedRotation, Time.deltaTime * _rotationDamping);
                    }
                    else transform.LookAt(Target, Target.up);
                    break;
                case CameraMode.THIRDPERSONINFRONT:
                    if (_followBehind)
                        wantedPosition = Target.TransformPoint(0, Height, Distance);
                    else
                        wantedPosition = Target.TransformPoint(0, Height, -Distance);

                    transform.position = Vector3.Lerp(transform.position, wantedPosition, 1);// Time.deltaTime * _damping);

                    if (_smoothRotation)
                    {
                        Quaternion wantedRotation = Quaternion.LookRotation(Target.position - transform.position, Target.up);
                        transform.rotation = Quaternion.Slerp(transform.rotation, wantedRotation, Time.deltaTime * _rotationDamping);
                    }
                    else transform.LookAt(Target, Target.up);
                    break;
                default:
                    break;
            }

        }
    }

    private IEnumerator WaitForTransition() { // ABSTRACTION
		canFollow = true;

		float defaultDamp = _damping;
		_damping /= 8f;

		yield return new WaitForSeconds(7f);
		_damping = defaultDamp;
	}

	private IEnumerator CameraSwapCooldown()
	{
		canSwap = false;

		yield return new WaitForSeconds(0.5f);

		canSwap = true;
	}
}
