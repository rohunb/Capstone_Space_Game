﻿using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class AI_Ship : TurnBasedUnit, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    #region Fields
    
    private bool receivedMoveCommand;
    private bool receivedAttackCommand;

    //TEMP
    private float range = 50.0f;
    //TEMP
    private float damagePerAttack = 35.0f;

    private PlayerShip targetShip;

    //references
    public AI_Attack ai_Attack { get; private set; }

    List<ShipComponent> activeComponents = new List<ShipComponent>();

    //Events
    public delegate void ShipClickEvent(AI_Ship ship);
    public event ShipClickEvent OnShipClick = new ShipClickEvent((AI_Ship) => { });
    public delegate void ShipMouseEnterEvent(AI_Ship ship);
    public event ShipMouseEnterEvent OnShipMouseEnter = new ShipMouseEnterEvent((AI_Ship) => { });
    public delegate void ShipMouseExitEvent(AI_Ship ship);
    public event ShipMouseExitEvent OnShipMouseExit = new ShipMouseExitEvent((AI_Ship) => { });


    PlayerShip targetPlayer;

    #endregion Fields

    #region Methods
    #region PublicMethods
    public void Init(ShipBlueprint shipBP, ShipMove shipMove, AI_Attack ai_Attack)
    {
        base.Init(shipBP, shipMove);
        this.ai_Attack = ai_Attack;

        //foreach (ShipComponent component in shipBP.slot_component_table.Values)
        //{
        //    activeComponents.Add(component);
        //    component.Init();
        //}

        trans = transform;
    }

    protected override void PreTurnActions()
    {
        
    }
    public override IEnumerator ExecuteTurn()
    {
        yield return StartCoroutine(base.ExecuteTurn());
        #if FULL_DEBUG
        Debug.Log("AI unit turn");
        #endif

        PreTurnActions();

        targetPlayer = TargetEnemy(TurnBasedCombatSystem.Instance.playerShips);
        ShipComponent targetComponent = TargetComponent(targetPlayer);

        //move phase
        Move(targetPlayer, targetComponent.Placement);
        if (receivedMoveCommand)
        {
            yield return StartCoroutine(shipMove.Move());
            receivedMoveCommand = false;
            #if FULL_DEBUG
            Debug.Log(name + "- Movement end");
            #endif
        }
        //attack phase
        Attack();
        if (receivedAttackCommand)
        {
            activeComponents = components;


            yield return StartCoroutine(ai_Attack.Attack(targetComponent, damagePerAttack, activeComponents));
            
            receivedAttackCommand = false;
            #if FULL_DEBUG
            Debug.Log(name + "- Attack end");
            #endif
        }
        PostTurnActions();
    }

    public void RetargetNewComponent()
    {
        Debug.Log("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
        activeComponents = components;
        ShipComponent targetComponent = TargetComponent(targetPlayer);
        StartCoroutine(ai_Attack.Attack(targetComponent, damagePerAttack, activeComponents));
    }

    protected override void PostTurnActions()
    {
        
    }

    public void Move(PlayerShip targetPlayer, AI_Fleet.PlacementType _placement)
    {
        if (!receivedMoveCommand)
        {
            #if FULL_DEBUG
            Debug.Log("Move command received " + ShipBPMetaData.BlueprintName);
            #endif
            Vector3 enemyPosition = targetPlayer.transform.position;
            switch (_placement)
            {
                case AI_Fleet.PlacementType.FORWARD:
                    
                    shipMove.destination = enemyPosition + (Vector3.forward * range);
                    break;
                case AI_Fleet.PlacementType.AFT:
                    shipMove.destination = enemyPosition + (-Vector3.back * range);
                    break;
                case AI_Fleet.PlacementType.PORT:
                    shipMove.destination = enemyPosition + (Vector3.left * range);
                    break;
                case AI_Fleet.PlacementType.STARBOARD:
                    shipMove.destination = enemyPosition + (Vector3.right * range);
                    break;
                case AI_Fleet.PlacementType.COUNT:
                default:
                    break;
            }
            receivedMoveCommand = true;
        }
    }
    public void Attack()
    {
        if (!receivedAttackCommand)
        {
            #if FULL_DEBUG
            Debug.Log("Attack command received " + ShipBPMetaData.BlueprintName);
            #endif
            receivedAttackCommand = true;
        }
    }

    private PlayerShip TargetEnemy(List<PlayerShip> playerShips)
    {
        if (playerShips == null || playerShips.Count == 0)
        {
            return null;
        }
        else
        {
            if (targetShip == null)
            {
                Vector3 selfPos = trans.position;
                
                //determine which enemy to target: strongest, weakest, closest, furthest
                float confidence = Random.Range(0.0f, 1.0f);

                //case 1 ... closeset
                if (confidence > 1 - AIManager.tgtClosest)
                {
                    //closest enemy
                    return playerShips.Aggregate((current, next) => Vector3.Distance(current.transform.position, selfPos) < Vector3.Distance(next.transform.position, selfPos) ? current : next);
                }

                //case 2 ... farthest
                if (confidence > 1 - AIManager.tgtClosest + AIManager.tgtFarthest)
                {
                    //farthest enemy
                    return playerShips.Aggregate((current, next) =>Vector3.Distance(current.transform.position, selfPos) > Vector3.Distance(next.transform.position, selfPos) ? current : next);
                }

                //case 3 ... strongest
                if (confidence > 1 - AIManager.tgtClosest + AIManager.tgtFarthest + AIManager.tgtStrongest)
                {
                    //strongest enemy
                    return playerShips.Aggregate((current, next) => current.HullHP > next.HullHP ? current : next);
                }

                //assume case 4... weakest enemy

                //weakest enemy
                return playerShips.Aggregate((current, next) => current.HullHP < next.HullHP ? current : next);
            }

            //target ship wasn't null so return your current target
            return targetShip;
        }     
    }

    private ShipComponent TargetComponent(PlayerShip _ship)
    {
        ShipComponent _targetComponent = null ;

        float confidence = Random.Range(0.0f, 1.0f);

        //get count for player fleet
        // get count for enemy fleet

        //calculate basis for confidence

        //adjust based on archetypes

        //for now I'm calling the line of confidence at 0.5 this will change as the AI gets more complex -A

        //setup a 10 step system ranging from dullard with a club to military commander with laser guided intel
        //set confidence to choose from those 10 options
        Debug.LogWarning(confidence);
        if (confidence >= 0.5f)
        {
            //go in order of: weapons, defensive, support, engineering
            if (_ship.Components.Where(c => c.active && c.CompType == ComponentType.Weapon).ToList().Count > 0)
            {
                _targetComponent = _ship.Components.Where(c => c.active && c.CompType == ComponentType.Weapon).Aggregate((curr, next) => curr.CompHP <= next.CompHP ? curr : next);
            }
            else if (_ship.Components.Where(c => c.active && c.CompType == ComponentType.Defense).ToList().Count > 0)
            {
               _targetComponent = _ship.Components.Where(c => c.active && c.CompType == ComponentType.Defense).Aggregate((curr, next) => curr.CompHP <= next.CompHP ? curr : next);
            }
            else if (_ship.Components.Where(c => c.active && c.CompType == ComponentType.Support).ToList().Count > 0)
            {
                _targetComponent = _ship.Components.Where(c => c.active && c.CompType == ComponentType.Support).Aggregate((curr, next) => curr.CompHP <= next.CompHP ? curr : next);
            }
            else if (_ship.Components.Where(c => c.active && c.CompType == ComponentType.Engineering).ToList().Count > 0)
            {
                _targetComponent = _ship.Components.Where(c => c.active && c.CompType == ComponentType.Engineering).Aggregate((curr, next) => curr.CompHP <= next.CompHP ? curr : next);
            }

            Debug.LogError(_targetComponent);

            if (_targetComponent == null)
            {
                Debug.LogError("Something is very wrong. all component lists empty. AI cannot target components on an empty ship");
            }
        }
        else
        {
            // go in order of: engineering, support, weapons, defensive
            if (_ship.Components.Where(c => c.active && c.CompType == ComponentType.Engineering).ToList().Count > 0)
            {
                _targetComponent = _ship.Components.Where(c => c.active && c.CompType == ComponentType.Engineering).Aggregate((curr, next) => curr.CompHP <= next.CompHP ? curr : next);
            }
            else if (_ship.Components.Where(c => c.active && c.CompType == ComponentType.Support).ToList().Count > 0)
            {
               _targetComponent = _ship.Components.Where(c => c.active && c.CompType == ComponentType.Support).Aggregate((curr, next) => curr.CompHP <= next.CompHP ? curr : next);
            }
            else if (_ship.Components.Where(c => c.active && c.CompType == ComponentType.Weapon).ToList().Count > 0)
            {
                _targetComponent = _ship.Components.Where(c => c.active && c.CompType == ComponentType.Weapon).Aggregate((curr, next) => curr.CompHP <= next.CompHP ? curr : next);
            }
            else if (_ship.Components.Where(c => c.active && c.CompType == ComponentType.Defense).ToList().Count > 0)
            {
                _targetComponent = _ship.Components.Where(c => c.active && c.CompType == ComponentType.Defense).Aggregate((curr, next) => curr.CompHP <= next.CompHP ? curr : next);
            }

            Debug.LogError(_targetComponent);

            if (_targetComponent == null)
            {
                Debug.LogError("Something is very wrong. all component lists empty. AI cannot target components on an empty ship");
            }
        }

        return _targetComponent;
    }

    #endregion PublicMethods
    #endregion Methods

    #region InternalCallbacks
    public void OnPointerEnter(PointerEventData eventData)
    {
        //Debug.Log("Mouse over " + name);
        OnShipMouseEnter(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //Debug.Log("Mouse exit " + name);
        OnShipMouseExit(this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //Debug.Log("Mouse click " + name);
        OnShipClick(this);
    }
    #endregion InternalCallbacks
}
