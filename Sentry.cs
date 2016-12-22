using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sentry : MonoBehaviour {

    public GameObject player; //Our player
    public Transform target; //HAS TO BE THE SAME THINGY!!!
    public float rotationSpeed = 40f; //Rotation speed when sentry is idling.
    public float possibleRotationSpeed = 400f; //Determines how quickly the sentry can move, when it has got a player in vision.
    public float sentryRange = 10; //Determines tracking range of the sentry.
    public float fireEveryMS = 100f; //Determines the fire cooldown.
    public float bulletSpeed = 20f; //Determines the speed of the fired bullets.
    public float bulletScale = 0.1f; //0.1 is recommended.
    public float maxXAngle; //Specify how huge the turn angle is on the x-axis!
    public float damagePerSecond; 

    Vector3 initialRotation;
    float time;
    bool hasFocusOnPlayer;


	// Use this for initialization
	void Start () {

        initialRotation = this.transform.rotation.eulerAngles;
        hasFocusOnPlayer = false;
	}
	
	// Update is called once per frame
	void Update () {
        time += Time.deltaTime;

        Vector3 targetVector = this.transform.position - target.transform.position; //Gets the vector between the player and the sentry gun.
        Vector2 thisObject = new Vector2(this.transform.eulerAngles.x, this.transform.eulerAngles.y); //Convert to 2-dimensional vector.
        Vector2 toPlayer = new Vector2(targetVector.x, targetVector.z); //Convert target vectore to two-dimensional vector.

        if (absValue(targetVector) < sentryRange) //Checks whether player is in range.
        {
            if (angleBetweenVectors(new Vector2(Mathf.Sin(thisObject.y * Mathf.PI / 180), Mathf.Cos(thisObject.y * Mathf.PI / 180)), toPlayer) <  possibleRotationSpeed * Time.deltaTime) //Checks whether sentry can get to player direction within one frame.
            {
                hasFocusOnPlayer = true;
                this.transform.rotation = Quaternion.Euler(Quaternion.LookRotation(targetVector).eulerAngles.x, Quaternion.LookRotation(targetVector).eulerAngles.y, 0); //Gets the required x and y axis.

                if (Quaternion.LookRotation(targetVector).eulerAngles.x > maxXAngle && Quaternion.LookRotation(targetVector).eulerAngles.x <= 180) //If x rotation would be to huge
                {
                    hasFocusOnPlayer = false;
                    this.transform.rotation = Quaternion.Euler(maxXAngle, Quaternion.LookRotation(targetVector).eulerAngles.y, 0); //Sets the x rotation to the highest possible value.
                }
                else if (Quaternion.LookRotation(targetVector).eulerAngles.x < 360 - maxXAngle && Quaternion.LookRotation(targetVector).eulerAngles.x >= 180) //If x rotation would be to huge in negative direction
                {
                    hasFocusOnPlayer = false;
                    this.transform.rotation = Quaternion.Euler(-maxXAngle, Quaternion.LookRotation(targetVector).eulerAngles.y, 0); //Sets the x rotation to the highest possible value in negative direction.
                }

                if (1000 * time >= fireEveryMS) //executed if time threshold is over.
                {
                    generateBullet();
                    time = 0; //resets time
                }
            }
            else if (angleBetweenVectors(new Vector2(Mathf.Sin(thisObject.y * Mathf.PI / 180), Mathf.Cos(thisObject.y * Mathf.PI / 180)), toPlayer) < 180) //If not, move as quickly as possible
            {
                hasFocusOnPlayer = false;
                this.transform.localEulerAngles -= new Vector3(0, possibleRotationSpeed * Time.deltaTime, 0);
            }
            else if (angleBetweenVectors(new Vector2(Mathf.Sin(thisObject.y * Mathf.PI / 180), Mathf.Cos(thisObject.y * Mathf.PI / 180)), toPlayer) > 180) //Same, but other direction. TODO: DOES NOT WORK YET
            {
                hasFocusOnPlayer = false;
                this.transform.localEulerAngles += new Vector3(0, possibleRotationSpeed * Time.deltaTime, 0);
            }
        } else
        {
            hasFocusOnPlayer = false;
            this.transform.localEulerAngles = new Vector3(0, this.transform.localEulerAngles.y, 0); //Resets the x rotation of the sentry.
            this.transform.localEulerAngles += new Vector3(0, rotationSpeed * Time.deltaTime, 0); //Sentry just rotates around y-axis.
        }
    }

    private void FixedUpdate()
    {
        RaycastHit hit;

        Debug.DrawRay(transform.position, -this.transform.forward * 1000);

        if (Physics.Raycast(this.transform.position, -this.transform.forward, out hit))
        {
            if (hit.transform.tag == "Player" && hasFocusOnPlayer)
            {
                PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
                playerHealth.dealDamage(damagePerSecond * Time.deltaTime);
            }
        }
    }

    float absValue(Vector3 vector)
    {
        return Mathf.Pow(Mathf.Pow(vector.x, 2) + Mathf.Pow(vector.y, 2) + Mathf.Pow(vector.z, 2), 1 / 3f); //Returns the absolute value of the vector.
    }

    float absValue(Vector2 vector)
    {
        return Mathf.Pow(Mathf.Pow(vector.x, 2) + Mathf.Pow(vector.y, 2), 1 / 2f); //Returns the absolute value of the vector.
    }

    float angleBetweenVectors(Vector2 vector1, Vector2 vector2)
    {
        return Mathf.Acos(dotProduct(vector1, vector2) / (absValue(vector1) * absValue(vector2))) * 180 / Mathf.PI; //Returns the angle between two vectors.
    }

    float dotProduct(Vector2 vector1, Vector2 vector2)
    {
        return vector1.x * vector2.x + vector1.y * vector2.y; //Returns the dot product of two vectors.
    }

    void generateBullet()
    {
        float creationTime = time;
        GameObject bullet = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        bullet.tag = "bullet";
        bullet.AddComponent<Rigidbody>();
        bullet.transform.position = this.transform.position;
        bullet.transform.localScale = new Vector3(bulletScale, bulletScale, bulletScale);
        MoveObject.Move(bullet, new Vector3(target.transform.position.x - this.transform.position.x, target.transform.position.y - this.transform.position.y, target.transform.position.z - this.transform.position.z), bulletSpeed);
    }
}
