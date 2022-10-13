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

    // Crew that this character belongs to (should be set by CrewSystem)
    public GameObject crew;

    // Start is called before the first frame update
    void Start()
    {
        // Attach healthbar to canvas
        canvas = GameObject.Find("UI Menu");
        healthBar = Instantiate(healthBar, canvas.transform);

        // Set gradient color
        bool isPlayer = !crew.GetComponent<CrewSystem>().isPlayer;
        Gradient grad = new Gradient();

        // Populate the color keys at the relative time 0 and 1 (0 and 100%)
        GradientColorKey[] colorKey = new GradientColorKey[2];
        colorKey[0].color = isPlayer ? Color.yellow : new Color(255f/255f, 165f/255f, 0f, 1f);
        colorKey[0].time = 0.0f;
        colorKey[1].color = isPlayer ? Color.green : Color.red;
        colorKey[1].time = 1.0f;

        GradientAlphaKey[] alphaKey = new GradientAlphaKey[1];
        alphaKey[0].alpha = 0.5f;

        grad.SetKeys(colorKey, alphaKey);

        healthBar.gradient = grad;

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
