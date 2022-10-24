/// @author: Jayden Wang
/// @date: 10/17/2022
/// @description: A class object for easy resolution of player actions and their targets.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAction : MonoBehaviour
{
    ///////////////////////
    // Private Variables //
    ///////////////////////

    //The character whose turn is being recorded
    private CharacterStats _character;

    //The target of the character's actions (might be empty)
    private EnemyStats _target;

    //The ability the character used on its turn (might be empty)
    private Ability _action;

    //Whether or not the character moved this turn
    private bool _movement;
    
    /////////////////////
    // Default Methods //
    /////////////////////

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /////////////
    // Methods //
    /////////////

    //Default Constructor
    public PlayerAction()
    {
        _target = null;
        _action = null;
        _character = null;
        _movement = false;
    }

    //Parameterized Constructor
    public PlayerAction(CharacterStats character, EnemyStats target, Ability action, bool movement)
    {
        _target = target;
        _action = action;
        _character = character;
        _movement = movement;
    }

    //Destructor
    ~PlayerAction()
    {
        //Not likely needed but always good to have for debugging / just in case
    }

    public CharacterStats GetCharacter()
    {
        return _character;
    }

    public EnemyStats GetTarget()
    {
        return _target;
    }

    public Ability GetAbility()
    {
        return _action;
    }

    public bool GetMovement()
    {
        return _movement;
    }
}
