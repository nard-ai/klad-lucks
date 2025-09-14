using UnityEngine;

public class BasicCatUnit : MonoBehaviour
{
    [Header("Basic Cat Stats")]
    public string unitName = "Basic Cat";
    public int cost = 75;
    public float cooldown = 2f;
    public float health = 10f;
    public float attackDamage = 10f;
    public float moveSpeed = 3f;
    
    void Start()
    {
        // Set up unit with these stats
        UnitBehavior behavior = GetComponent<UnitBehavior>();
        if (behavior != null)
        {
            behavior.health = health;
            behavior.maxHealth = health;
            behavior.attackDamage = attackDamage;
            behavior.moveSpeed = moveSpeed;
        }
        
        // Set up visual appearance (basic cube for now)
        SetupVisuals();
    }
    
    void SetupVisuals()
    {
        // Create a simple visual representation
        if (GetComponent<Renderer>() == null)
        {
            gameObject.AddComponent<MeshRenderer>();
            gameObject.AddComponent<MeshFilter>().mesh = CreateCubeMesh();
        }
        
        // Color based on team
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            UnitBehavior behavior = GetComponent<UnitBehavior>();
            if (behavior != null && behavior.isPlayerUnit)
            {
                renderer.material.color = Color.blue; // Player units are blue
            }
            else
            {
                renderer.material.color = Color.red; // Enemy units are red
            }
        }
    }
    
    Mesh CreateCubeMesh()
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Mesh mesh = cube.GetComponent<MeshFilter>().mesh;
        DestroyImmediate(cube);
        return mesh;
    }
}