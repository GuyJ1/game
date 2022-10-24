/// @author: Jayden Wang
/// @date: 10/17/2022
/// @description: A variable sized list that holds player actions for calculating enemy behavior. 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActionList : MonoBehaviour
{
    ///////////////////////
    // Private Variables //
    ///////////////////////

    private Queue<PlayerAction> _playerActions;
    private int _capacity;
    
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Constructor
    public PlayerActionList()
    {
        _playerActions = new Queue<PlayerAction>();
        _capacity = 50;
    }

    //Destructor
    ~PlayerActionList()
    {
        //Not likely needed but always good to have for debugging / just in case
    }

    /////////////
    // Methods //
    /////////////
    public void add(PlayerAction newAction)
    {
        //For now, we're experimenting with size 50 queue for holding player actions. If we're below 50, just enqueue. Otherwise, get rid of the oldest action and then enqueue.
        if(_playerActions.Count < _capacity)
        {
            _playerActions.Enqueue(newAction);
        }
        else
        {
            _playerActions.Dequeue();
            _playerActions.Enqueue(newAction);
        }
    }

    public PlayerAction Peek()
    {
        return _playerActions.Peek();
    }


}

