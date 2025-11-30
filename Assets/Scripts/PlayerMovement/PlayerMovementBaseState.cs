using UnityEngine;
public abstract class PlayerMovementBaseState
{
    
    protected PlayerMovementStateManager ctx;
    protected PlayerStateFactory factory;
    public PlayerMovementBaseState(PlayerMovementStateManager currentContext, PlayerStateFactory playerStateFactory)
    {
        ctx = currentContext;
        factory = playerStateFactory;
    }
    public abstract void enterState();

    public abstract void updateState();

    public abstract void exitState();

    public abstract void checkSwitchState();

    public abstract void initializeSubstate();

    void updateStates() { }
    protected void switchSates(PlayerMovementBaseState newState)
    {
        exitState();

        newState.enterState();

        ctx.CurrentState = newState;
    }
    protected void setSuperState() { }
    protected void setSubStates(){}
}
