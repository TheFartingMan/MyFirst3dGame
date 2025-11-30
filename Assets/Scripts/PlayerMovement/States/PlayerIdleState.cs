using UnityEngine;

public class PlayerIdleState : PlayerMovementBaseState
{
    public PlayerIdleState(PlayerMovementStateManager currentContext, PlayerStateFactory playerStateFactory)
    : base (currentContext, playerStateFactory)
    {
        
    }
    public override void enterState(){}

    public override void updateState(){}

    public override void exitState(){}

    public override void checkSwitchState(){}

    public override void initializeSubstate(){}
}
