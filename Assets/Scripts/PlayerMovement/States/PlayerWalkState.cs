using UnityEngine;

public class PlayerWalkState : PlayerMovementBaseState
{
    public PlayerWalkState(PlayerMovementStateManager currentContext, PlayerStateFactory playerStateFactory)
    : base (currentContext, playerStateFactory)
    {
        
    }
    public override void enterState(){}

    public override void updateState(){}

    public override void exitState(){}

    public override void checkSwitchState(){}

    public override void initializeSubstate(){}
}
