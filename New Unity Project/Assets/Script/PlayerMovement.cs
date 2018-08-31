using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    [SerializeField] public float maxSpeed = 7;
    protected Vector2 targetVelocity;
    protected Rigidbody2D rigidBody2D;
    protected ContactFilter2D contactFilter;
    protected RaycastHit2D[] hitBuffer = new RaycastHit2D[16];
    protected List<RaycastHit2D> hitBufferList = new List<RaycastHit2D>(16);

    protected const float minMoveDistance = 0.001f;
    protected const float shellRadius = 0.01f;
    private bool fireIsPressed = false;

    private void Awake() {
        contactFilter.useTriggers = false;
        contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
        contactFilter.useLayerMask = true;
        rigidBody2D = GetComponent<Rigidbody2D>();
    }
    void FixedUpdate() {
        Vector2 velocityX = new Vector2(); ;
        Vector2 velocityY = new Vector2(); ;
        velocityX.x= targetVelocity.x;
        velocityY.y = targetVelocity.y;
        Vector2 deltaPositionX = velocityX * Time.deltaTime;
        Movement(deltaPositionX, true);
        Vector2 deltaPositionY = velocityY * Time.deltaTime;
        Movement(deltaPositionY, true);
    }
    void Update() {
        ComputeVelocity();
        ManageInterraction();
    }
    protected  void ComputeVelocity() {
        Vector2 move = Vector2.zero;
        move.x = Input.GetAxis("Horizontal");
        move.y = Input.GetAxis("Vertical");
        targetVelocity = move.normalized * maxSpeed;
    }
    void Movement(Vector2 move, bool yMovement) {
        float distance = move.magnitude;

        if (distance > minMoveDistance) {
            int count = rigidBody2D.Cast(move, contactFilter, hitBuffer, distance + shellRadius);
            hitBufferList.Clear();
            for (int i = 0; i < count; i++) {
                hitBufferList.Add(hitBuffer[i]);
            }
 
            for (int i = 0; i < hitBufferList.Count; i++) {
                Vector2 currentNormal = hitBufferList[i].normal;
                float modifiedDistance = hitBufferList[i].distance - shellRadius;
                distance = modifiedDistance < distance ? modifiedDistance : distance;
            }
        }
        rigidBody2D.position = rigidBody2D.position + move.normalized * distance;
    }

    private void ManageInterraction()
    {
        if(Input.GetAxis("Jump")!=0)
        {
            if(!fireIsPressed)
            {
                print("FIRE!");
                fireIsPressed = true;
                ContactFilter2D contactFilter2DInterraction = new ContactFilter2D();
                contactFilter2DInterraction.useTriggers = false;
                contactFilter2DInterraction.SetLayerMask(Physics2D.GetLayerCollisionMask(LayerMask.NameToLayer("Interaction")));
                contactFilter2DInterraction.useLayerMask = true;
                RaycastHit2D[] interractionHit = new RaycastHit2D[16];
                List<RaycastHit2D> hitBufferListInterraction = new List<RaycastHit2D>(16);
                hitBufferListInterraction.Clear();
                int countInterraction = Physics2D.Raycast(gameObject.transform.position, Vector2.up, contactFilter2DInterraction, interractionHit);
                for (int i = 0; i < countInterraction; i++)
                {
                    hitBufferListInterraction.Add(interractionHit[i]);
                }
                if(hitBufferListInterraction.Count > 0)
                {
                    hitBufferListInterraction[0].transform.gameObject.GetComponent<Interraction>().Interact();
                }
            }
            
        }
        else
        {
            fireIsPressed = false;
        }
    }
}
