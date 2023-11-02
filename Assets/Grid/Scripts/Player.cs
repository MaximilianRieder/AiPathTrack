using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    public Rigidbody2D rigidbody;
    public GameHandler gh;
    public GameObject targetFoundBanner;
    AStar aStar;
    int[,] gridArray;

    Path path = new Path();

    private Vector2 position;
    private Vector2 velocity;
    private Vector2 acceleration;

    //used to cap to the speed limit
    private float maxspeed = 1f;
    //used to predict future location
    private float preditctionNumber = 2f;
    //used to set a seeking target with distance of lookAheadNumber from normal point
    private float lookAheadNumber = 1f;

    //initialize
    private void Start()
    {
        //get things from gamehandler class
        GameObject go = GameObject.Find("GameHandler");
        gh = go.GetComponent<GameHandler>();
        aStar = gh.aStar;
        gridArray = gh.gridArray;
        foreach (Vector2 v in gh.pathSteering)
        {
            path.addPoint(v.x, v.y);
        }
        //initialize
        position = this.transform.position;
        velocity = new Vector2(maxspeed, 0);
        acceleration = new Vector2(0, 0);
        //startign force to get going
        rigidbody.AddForce(new Vector2(5, 5));
    }

    //loop this every frame
    void Update()
    {
        follow(path);
        UpdatePosition();
    }

    //check collision with target -> spawn target object if hit
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "target")
        {
            GameObject tfb = Instantiate(targetFoundBanner, aStar.GetGrid().getPositionInGameWithOffset(gridArray.GetLength(0) / 2, gridArray.GetLength(1) / 2), Quaternion.identity);
            tfb.transform.position = new Vector3(tfb.transform.position.x, tfb.transform.position.y, -8);
            //0.128 is hacky solution same with 0.446 -> is good with initial setup
            tfb.transform.localScale = new Vector3(gridArray.GetLength(0) * aStar.GetGrid().GetCellSize() * 0.128f, gridArray.GetLength(1) * aStar.GetGrid().GetCellSize() * 0.446f, 1);
        }
    }

    //the follow behaviour
    public void follow(Path p)
    {
        //make a preditction based on velocity and chosen value
        Vector2 predict = velocity;
        predict.Normalize();
        predict = predict * preditctionNumber;
        Vector2 predictpos = position + predict;

        Vector2 target = Vector2.zero;
        //high number to compare distances
        float meassureNumberDistance = 10000000;

        //loop for all path nodes
        for (int i = 0; i < p.points.Count - 1; i++)
        {

            //segment
            Vector2 a = p.points[i];
            Vector2 b = p.points[i + 1];

            //get the normal
            Vector2 normal = getNormal(predictpos, a, b);

            //if the normal exceeds the segment between a and b put it to b
            //for each case (e.g. pathfinding from the left, right, and down)
            if (a.x < b.x)
            {
                if (normal.x < a.x || normal.x > b.x)
                {
                    normal = b;
                }
            }
            else
            {
                if (normal.x > a.x || normal.x < b.x)
                {
                    normal = b;
                }
            }
            if (a.y < b.y)
            {
                if (normal.y < a.y || normal.y > b.y)
                {
                    normal = b;
                }
            }
            else
            {
                if (normal.y > a.y || normal.y < b.y)
                {
                    normal = b;
                }
            }

            //calculate distance to normal
            float distance = Vector2.Distance(predictpos, normal);
            //check if this is a better distance -> new target
            if (distance < meassureNumberDistance)
            {
                meassureNumberDistance = distance;

                //look ahead of the normal on the path
                Vector2 direction = b - a;
                direction.Normalize();
                direction = direction * lookAheadNumber;
                target = normal;
                target = target + direction;
            }
        }

        //if distance out of path radius -> steer
        if (meassureNumberDistance > p.radius)
        {
            seek(target);
        }
    }


    //get normal from p to segment ab
    Vector2 getNormal(Vector2 p, Vector2 a, Vector2 b)
    {
        Vector2 ap = p - a;
        Vector2 ab = b - a;
        ab.Normalize();
        ab = ab * (Vector2.Dot(ap, ab));
        Vector2 normal = a + ab;
        return normal;
    }

    //apply force (with acceleration)
    void applyForce(Vector2 steer)
    {
        acceleration = acceleration + steer;
    }


    //seek the target
    void seek(Vector2 target)
    {
        //where you want to go
        Vector2 desired = target - position;
        //cap the maxspeed
        desired.Normalize();
        desired = desired * maxspeed;
        //calculate steering force
        Vector2 steer = desired - velocity;
        applyForce(steer);
    }

    //update the position with unitys rigid body 
    private void UpdatePosition()
    {
        //get the actual postion in unity
        position = transform.position;
        //add the force to the unity physics model
        rigidbody.AddForce(acceleration);
        //reset acceleration -> new force every loop
        acceleration = acceleration * 0;
        //update the actual velocity from unity
        velocity = rigidbody.velocity;
    }
}
