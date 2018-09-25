using UnityEngine;
using System.Collections;

public class CharController2D : MonoBehaviour {

	private const float skinWidth = 0.2f;
	private const int noOfHRays = 8;
	private const int noOfVRays = 4;
	public bool _isFacingRight; 
	private static readonly float slopeLimitTangent = Mathf.Tan (75f * Mathf.Deg2Rad);
	private Vector2 _vel;
	private Transform _trans;
	private Vector3 _lScale;
	private BoxCollider2D _boxColl;
	private float _vDbetRays; 
	private float _hDbetRays;
	private float _jmpIn;
	private Vector3 
		_rayCastTopLeft,
		_rayCastTopRight,
		_rayCastBottomRight,
		_rayCastBottomLeft;
	private ControllerParameters2D _overrideParameters;
	private Vector3
		_activeLocalPlatPoint,
		_activeGlobalPlatPoint;
	private GameObject _lastStandingOn;

	public Vector3 platVelocity { get; private set;}
	public LayerMask platMask;
	public ControllerParameters2D defParameters;
	public ControllerState2D state{ get; private set;}
	public Vector2 Velocity{ get{ return _vel; } }
	public bool CanJump{

		get{ 

			if (Parameters.jmpRestriction == ControllerParameters2D.JmpBehaviour.canJmpAnywhere)
				return _jmpIn <= 0;
			if (Parameters.jmpRestriction == ControllerParameters2D.JmpBehaviour.canJmpOnGround)
				return state.IsGrounded;
			
			return false;
		
		}
	
	}
	public bool HandleCollisions{ get; set;}
	public ControllerParameters2D Parameters{get{ return _overrideParameters ?? defParameters;}}
	public GameObject StandingOn { get; private set;}

	public void Awake(){

		_isFacingRight = transform.localScale.x > 0;
		state = new ControllerState2D ();
		_trans = transform;
		HandleCollisions = true;
		_lScale = transform.localScale;
		_boxColl = GetComponent<BoxCollider2D> ();

		var collWidth = _boxColl.size.x * Mathf.Abs (transform.localScale.x) - (2 * skinWidth);
		_hDbetRays = collWidth / (noOfVRays - 1);

		var collHeight = _boxColl.size.y * Mathf.Abs (transform.localScale.y) - (2 * skinWidth);
		_vDbetRays = collHeight / (noOfHRays - 1);
		//Debug.Log ("Height :" + _vDbetRays);

	}
	public void AddForce(Vector2 force){
		_vel += force;
	}
	public void SetForce(Vector2 force){ 
		_vel = force;
	}
	public void SetHForce(float x){
		_vel.x = x;
	}
	public void SetVForce(float y){
		_vel.y = y;
	}
	public void Jump(){
		AddForce (new Vector2(0, Parameters.jmpMagnitude));
		_jmpIn = Parameters.jmpFreq;

	}
	public void LateUpdate(){
		//Debug.Log ("Can jmp :"+Parameters.jmpRestriction);
		_jmpIn -= Time.deltaTime; 
		_vel.y += Parameters.gravity * Time.deltaTime;

		//VisionScope (Velocity * Time.deltaTime);
		Move (Velocity * Time.deltaTime);

	}
	private void Move(Vector2 deltaMov){
		//Debug.Log (deltaMov);
		var _wasGrounded = state.IsCollidingBelow;
		//VisionScope (ref deltaMov);
		state.Reset ();

		if (HandleCollisions) {
			HandlePlat ();
			CalcRayOrigin ();

			if (deltaMov.y < 0)
				HandleVSlope (ref deltaMov);

			if (Mathf.Abs (deltaMov.x) > 0.001f) {
				MovH (ref deltaMov);
				VisionScope (ref deltaMov);
			}
			MovV (ref deltaMov);
			VisionScope (ref deltaMov);
			CorrectHPlacement (ref deltaMov, true);
			CorrectHPlacement (ref deltaMov, false);

		}
		_trans.Translate (deltaMov, Space.World);
		if(Time.deltaTime > 0)
			_vel =  deltaMov / Time.deltaTime; 
		_vel.x = Mathf.Min (_vel.x, Parameters.moveVelocity.x);
		_vel.y = Mathf.Min (_vel.y, Parameters.moveVelocity.y);
		if (state.IsMovingUpSlope)
			_vel.y = 0;

		if (StandingOn != null) {
			_activeGlobalPlatPoint = transform.position;
			_activeLocalPlatPoint = StandingOn.transform.InverseTransformPoint (transform.position);
		
			if (_lastStandingOn != StandingOn) {
				if (_lastStandingOn != null)
					_lastStandingOn.SendMessage ("ControllerExit2D", this, SendMessageOptions.DontRequireReceiver);
				
				StandingOn.SendMessage ("ControllerEnter2D", this, SendMessageOptions.DontRequireReceiver);
				_lastStandingOn = StandingOn;
			} else if (StandingOn != null)
				StandingOn.SendMessage ("ControllerStay2D", this, SendMessageOptions.DontRequireReceiver);
		} 
		else if (_lastStandingOn != null) {
			_lastStandingOn.SendMessage ("ControllerExit2D", this, SendMessageOptions.DontRequireReceiver);
			_lastStandingOn = null;
		}


	}



	private void HandlePlat(){
		
		if (StandingOn != null) {
			var newGlobalPlatPoint = StandingOn.transform.TransformPoint (_activeLocalPlatPoint);
			var movDistance = newGlobalPlatPoint - _activeGlobalPlatPoint;

			if (movDistance != Vector3.zero)
				transform.Translate (movDistance, Space.World);
			platVelocity = (newGlobalPlatPoint - _activeGlobalPlatPoint) / Time.deltaTime;
		} else
			platVelocity = Vector3.zero;

		StandingOn = null;
	} 

	private void CorrectHPlacement(ref Vector2 deltaMov, bool isRight){
		var halfWidth = (_boxColl.size.x * _lScale.x) / 2f;
		var rayOrigin = isRight ? _rayCastBottomRight : _rayCastBottomLeft;

		if (isRight)
			rayOrigin.x -= (halfWidth - skinWidth);
		else
			rayOrigin.x += (halfWidth - skinWidth);
		var rayDirection = isRight ? Vector2.right : -Vector2.right;
		var offset = 0f;

		for (var i = 1; i < noOfHRays; i++) {
			var rayVector = new Vector2 (deltaMov.x+rayOrigin.x, deltaMov.y + rayOrigin.y + (i * _vDbetRays));
			Debug.DrawRay (rayVector, rayDirection * halfWidth, isRight ? Color.cyan : Color.magenta);

			var rayCastHit = Physics2D.Raycast (rayVector, rayDirection, halfWidth, platMask);
			if (!rayCastHit)
				continue;

			offset = isRight ? ((rayCastHit.point.x - _trans.position.x) - halfWidth) : (halfWidth - ( _trans.position.x-rayCastHit.point.x ));
		}

		deltaMov.x += offset;
	
	}

	private void CalcRayOrigin(){
		
		var size = new Vector2 (_boxColl.size.x * Mathf.Abs (_lScale.x), _boxColl.size.y * Mathf.Abs (_lScale.y)) / 2;
		var center = new Vector2 (_boxColl.offset.x * _lScale.x, _boxColl.offset.y * _lScale.y);

		_rayCastTopLeft = _trans.position + new Vector3 (center.x - size.x+skinWidth, center.y+size.y);
		_rayCastTopRight = _trans.position + new Vector3 (center.x + size.x-skinWidth, center.y+size.y);
		_rayCastBottomRight = _trans.position + new Vector3 (center.x + size.x - skinWidth, center.y - size.y + skinWidth );
		_rayCastBottomLeft = _trans.position + new Vector3 (center.x - size.x + skinWidth, center.y - size.y + skinWidth );


	}

	private void VisionScope(ref Vector2 deltaMov){
		var distance = 20;
		var rayOrigin = _isFacingRight ? _rayCastTopRight : _rayCastTopLeft;
		var rayDistance = Mathf.Abs (deltaMov.x) + distance + skinWidth;
		var rayDirection = _isFacingRight ? (_trans.forward + _trans.right).normalized : (_trans.forward - _trans.right).normalized;
		for (var i = 1; i <= 15; i++) {
			
			var rayVector = new Vector2 (rayOrigin.x, rayOrigin.y);
			Debug.DrawRay (rayVector,  rayDirection * rayDistance, Color.yellow);
			rayDirection.y +=  0.1f;
			rayDistance -= 0.7f;
		}


	}

	private void MovH(ref Vector2 deltaMov){
		var isGoingRight = deltaMov.x > 0;
		var rayDistance = Mathf.Abs (deltaMov.x) + skinWidth;
		var rayDirection = isGoingRight ? Vector2.right : -Vector2.right;
		var rayOrigins = isGoingRight ? _rayCastBottomRight : _rayCastBottomLeft;
		//Debug.Log ("ray ori x :" + rayOrigins.x + "ray ori y :" + rayOrigins.y);

		for(var i=0; i<noOfHRays; i++){
			
			var rayVector = new Vector2 (rayOrigins.x, rayOrigins.y + (i * _vDbetRays));
			Debug.DrawRay (rayVector, rayDirection * rayDistance, Color.red);
			//rayDirection.y = i * 1;
			var rayCastHit = Physics2D.Raycast (rayVector, rayDirection, rayDistance, platMask);
			if (!rayCastHit)
				continue;
			if (i == 0 && HandleHSlope (ref deltaMov, Vector2.Angle (rayCastHit.normal, Vector2.up), isGoingRight))
				break;	

			deltaMov.x = rayCastHit.point.x - rayVector.x;
			rayDistance = Mathf.Abs (deltaMov.x);

			if (isGoingRight) {
				deltaMov.x -= skinWidth;
				state.IsCollidingRight = true;
			} 
			else {
				deltaMov.x += skinWidth;
				state.IsCollidingLeft = true;
			}
			if (rayDistance < skinWidth + 0.0001f)
				break;


		}
	}

	private void MovV(ref Vector2 deltaMov){
		var isGoingUp = deltaMov.y > 0;
		Debug.Log ("is going up :" + isGoingUp);
		var rayDistance = Mathf.Abs (deltaMov.y) + skinWidth;
		var rayDirection = isGoingUp ? Vector2.up : -Vector2.up;
		var rayOrigins = isGoingUp ? _rayCastTopLeft : _rayCastBottomLeft;

		rayOrigins.x += deltaMov.x; 
		var standOnDistance = float.MaxValue;
		for(var i=0; i<noOfVRays;i++){
			var rayVector = new Vector2 (rayOrigins.x + (i * _hDbetRays), rayOrigins.y);
			Debug.DrawRay (rayVector, rayDirection * rayDistance, Color.red);
		
			var rayCastHit = Physics2D.Raycast (rayVector, rayDirection, rayDistance, platMask);
			if (!rayCastHit)
				continue;
			if(!isGoingUp){
				var vDistanceToHit = _trans.position.y - rayCastHit.point.y;
				if (vDistanceToHit < standOnDistance) {
					standOnDistance = vDistanceToHit;
					StandingOn = rayCastHit.collider.gameObject;
				}
			}

			deltaMov.y = rayCastHit.point.y - rayVector.y;
			rayDistance = Mathf.Abs (deltaMov.y);

			if (isGoingUp) {
				Debug.Log("asd");
				deltaMov.y -= skinWidth;
				state.IsCollidingAbove = true;
				//Parameters.jmpRestriction = ControllerParameters2D.JmpBehaviour.cantJmp;
				//Debug.Log("JMP :"+Parameters.jmpRestriction);
			} else {
				deltaMov.y += skinWidth;
				state.IsCollidingBelow = true;
				state.IsGrounded = true;
				Debug.Log("JMP :"+Parameters.jmpRestriction);
			}

			if (!isGoingUp && deltaMov.y > 0.0001f)
				state.IsMovingUpSlope = true;
			if (rayDistance < skinWidth + 0.0001f)
				break;

		}
	}
	private void HandleVSlope(ref Vector2 deltaMov){
		var center = (_rayCastBottomLeft.x + _rayCastBottomRight.x) / 2;
		//Debug.Log ("center :" + center);
		var direction = -Vector2.up;

		var slopeDistance = slopeLimitTangent * (_rayCastBottomRight.x - center);
		var slopeRayVector = new Vector2(center, _rayCastBottomLeft.y);

		Debug.DrawRay (slopeRayVector, direction * slopeDistance, Color.yellow); 
		var rayCastHit = Physics2D.Raycast (slopeRayVector, direction, slopeDistance, platMask);
		if (!rayCastHit)
			return;
		// ReSharper disable CompareOfFloatsByEqualityOperator
		var isMovingDownSlope = Mathf.Sign (rayCastHit.normal.x) == Mathf.Sign (deltaMov.x);
		if (!isMovingDownSlope)
			return;
		var angle = Vector2.Angle (rayCastHit.normal, Vector2.up);
		if (Mathf.Abs (angle) < 0.0001f)
			return;

		state.IsMovingDownSlope = true;
		state.SlopeAngle = angle;
		deltaMov.y = rayCastHit.point.y - slopeRayVector.y;

	}
	private bool HandleHSlope(ref Vector2 deltaMov, float angle, bool isGoingR){
		if (Mathf.RoundToInt (angle) == 90)
			return false;
		if (angle > Parameters.slopeLimit) {
			deltaMov.x = 0;
			return true;
		}
		if (deltaMov.y > 0.7f)
			return true;

		deltaMov.x += isGoingR ? -skinWidth : skinWidth;
		deltaMov.y = Mathf.Abs (Mathf.Tan (angle * Mathf.Deg2Rad) * deltaMov.x);
		state.IsMovingUpSlope = true;
		state.IsCollidingBelow = true;
		return true;
	}
	public void OnTriggerEnter2D(Collider2D coll){
		var parameters = coll.gameObject.GetComponent<ControllerPhysicsVolume2D> ();
		if (parameters == null)
			return;
		_overrideParameters = parameters.Parameters;
	}
	public void OnTriggerExit2D(Collider2D coll){
		var parameters = coll.gameObject.GetComponent<ControllerPhysicsVolume2D> ();
		if (parameters == null)
			return;
		_overrideParameters = null;
}
}