using UnityEngine;

public class TankCatUnit : MonoBehaviour
{
    [Header("Tank Cat Stats")]
    public string unitName = "Tank Cat";
    public int cost = 150;
    public float cooldown = 5f;
    public float health = 30f;
    public float attackDamage = 5f;
    public float moveSpeed = 1.5f;
    
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
        
        // Set up visual appearance
        SetupVisuals();
    }
    
    void SetupVisuals()
    {
        // Create a bigger, more robust visual
        if (GetComponent<Renderer>() == null)
        {
            gameObject.AddComponent<MeshRenderer>();
            gameObject.AddComponent<MeshFilter>().mesh = CreateCubeMesh();
        }
        
        // Make it bigger and darker (tankier look)
        transform.localScale = new Vector3(1.5f, 1.2f, 1.5f);
        
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            UnitBehavior behavior = GetComponent<UnitBehavior>();
            if (behavior != null && behavior.isPlayerUnit)
            {
                renderer.material.color = new Color(0, 0, 0.8f); // Dark blue for player tank
            }
            else
            {
                renderer.material.color = new Color(0.8f, 0, 0); // Dark red for enemy tank
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