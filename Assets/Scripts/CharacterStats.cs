using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{

    [SerializeField]
    public int HP;
    public int HPMAX;
    public int STR;
    public int DEF;
    public int SPD;
    public int MV;
    public string Name;

    // Actions
    public GameObject basicAttack, comboAttack; //Generic actions
    public List<GameObject> abilities; //Unique abilities

    // Healthbar
    public HealthBar healthBar;
    public float healthBarYOffset;

    // Canvas reference
    public GameObject canvas;

    // Character's logical position on the grid
    public Vector2Int gridPosition;

    // Start is called before the first frame update
    void Start()
    {
        // Attach healthbar to canvas
        canvas = GameObject.Find("UI Menu");
        healthBar = Instantiate(healthBar, canvas.transform);

        // Set health
        HP = HPMAX;
        healthBar.SetMaxHealth(HPMAX);
    }

    // Update is called once per frame
    void Update()
    {
        // Health Testing
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Damage(20);
        }
    }

    // Update healthbar position
    void LateUpdate()
    {
        // Offset
        Vector3 posOffset = new Vector3(0, healthBarYOffset, 0);

        // Update the healthbar position
        healthBar.transform.position = Camera.main.WorldToScreenPoint(transform.position + posOffset);
    }

    // Taking damage
    void Damage(int damage)
    {
        HP -= damage;
        healthBar.SetHealth(HP);
    }
}
