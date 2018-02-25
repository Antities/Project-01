using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Collisions : MonoBehaviour {

    [SerializeField] float _inset;
    [SerializeField] float _speed;
    [SerializeField] private int _verticalSpacing;
    [SerializeField] private int _horizontalSpacing;
    public LayerMask _collisionMask;

    private float _rayLength = 0.15f;
    private Vector2 _input;
    private Vector2 _velocity;
    private Vector2 _verticalSpacingVector;
    private Vector2 _horizontalSpacingVector;
    private BoxCollider2D _collider;

    private ColliderEdges _colliderEdges;
    

    private void Start()
    {
        _velocity = new Vector2();
        _input = new Vector2();
        _collider = GetComponent<BoxCollider2D>();
        UpdateColliderBounds();
    }

    private void Update()
    {
        _input = Vector2.zero;
        if (Input.GetKey(KeyCode.A))
        {
            _input = _input + new Vector2(-0.5f,0);
        }
        if(Input.GetKey(KeyCode.D))
        {
            _input = _input + new Vector2(0.5f, 0);
        }
        if (Input.GetKey(KeyCode.S))
        {
            _input = _input + new Vector2(0, -0.5f);
        }
        if (Input.GetKey(KeyCode.W))
        {
            _input = _input + new Vector2(0, 0.5f);
        }

        Move(_input);
    }

    private void UpdateColliderBounds()
    {
        Bounds bounds = _collider.bounds;
        bounds.Expand(_inset * -2);

        _colliderEdges.topLeft = new Vector2(bounds.min.x + _inset, bounds.max.y - _inset);

        _colliderEdges.topRight = new Vector2(bounds.max.x - _inset, bounds.max.y -_inset);

        _colliderEdges.bottomLeft = new Vector2(bounds.min.x + _inset, bounds.min.y + _inset);

        _colliderEdges.bottomRight = new Vector2(bounds.max.x - _inset, bounds.min.y + _inset);
    }

    public void Move(Vector2 movement)
    {

        movement = movement.normalized * _speed * Time.deltaTime;

        _velocity = Vector2.zero;
        if(movement.x != 0)
        {
            CollisionsHorizontal(ref movement);
        }

        if(movement.y != 0)
        {
            CollisionsVertical(ref movement);
        }

        gameObject.transform.Translate(_velocity, Space.World);
    }

    private void CalculateSpacing()
    {
        Bounds bounds = _collider.bounds;
        bounds.Expand(_inset * -2);

        _verticalSpacingVector = new Vector2(((bounds.size.x - _inset * 2) / (_verticalSpacing - 1)), 0);
        _horizontalSpacingVector = new Vector2(0, ((bounds.size.y - _inset * 2) / (_horizontalSpacing - 1)));
        
    }

    //Jos tästä tulee if helvetti niin ainakin se on minun if helvettini :)
    private void CollisionsHorizontal(ref Vector2 movement)
    {
        UpdateColliderBounds();
        CalculateSpacing();
        
        float direction = Mathf.Sign(movement.x);
        Vector2 origin = new Vector2();

        if (direction == 1)
        {
            origin = _colliderEdges.bottomRight;
        }
        else if (direction == -1)
        {
            origin = _colliderEdges.bottomLeft;
        }

        for (int i = 0; i < _horizontalSpacing; i++)
        {
            Debug.DrawRay(origin + (_horizontalSpacingVector * i), Vector2.right * (movement.x + _inset), Color.red);
            RaycastHit2D hit = Physics2D.Raycast(origin + (_horizontalSpacingVector * i), Vector2.right, movement.x + _inset * direction, _collisionMask);
            if (hit)
            {



                if (i == 0)
                {
                    float angle = Vector2.Angle(Vector2.down, hit.normal);
                    float y = Mathf.MoveTowardsAngle(transform.eulerAngles.y, angle, (Time.deltaTime / 0.5f));
                    if (angle > 90) { 
                        Debug.Log(angle);

                        _velocity.y = y;
                    }
                }
                else if(i == _horizontalSpacing-1)
                {
                    float angle = -Vector2.Angle(Vector2.down, hit.normal);
                    Debug.Log(angle);
                    float y = Mathf.MoveTowardsAngle(transform.eulerAngles.y, angle, (Time.deltaTime / 0.5f));

                    

                    _velocity.y = y;
                }

                _velocity.x = (hit.distance * direction) - _inset * direction;
                break;
            }
            else
            {
                _velocity.x = movement.x + _inset * direction;
            }
        }

    }


    private void CollisionsVertical(ref Vector2 movement)
    {
        UpdateColliderBounds();
        CalculateSpacing();

        float direction = Mathf.Sign(movement.y);
        Vector2 origin = new Vector2();

        if (direction == 1)
        {
            origin = _colliderEdges.topLeft;
        }
        else if (direction == -1)
        {
            origin = _colliderEdges.bottomLeft;
        }

        for (int i = 0; i < _verticalSpacing; i++)
        {

            Debug.DrawRay(origin + (_verticalSpacingVector * i), Vector2.up * movement.y, Color.red);
            RaycastHit2D hit = Physics2D.Raycast(origin + (_verticalSpacingVector * i), Vector2.up, movement.y + _inset, _collisionMask);

            if (hit)
            {
                _velocity.y = (hit.distance * direction) - (_inset * direction);
                break;
            }
            else
            {
                _velocity.y = movement.y + _inset * direction;
            }
        }
    }
    
    struct ColliderEdges
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }

}
