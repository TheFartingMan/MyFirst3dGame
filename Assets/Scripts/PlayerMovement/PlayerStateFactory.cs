public class PlayerStateFactory
{
    PlayerMovementStateManager context;

    public PlayerStateFactory(PlayerMovementStateManager currentContext)
    {
        context = currentContext;
    }

    public PlayerMovementBaseState idle()
    {
        return new PlayerIdleState(context, this);
    }
    public PlayerMovementBaseState walk()
    {
        return new PlayerWalkState(context, this);
    }
    public PlayerMovementBaseState jump()
    {
        return new PlayerJumpState(context, this);
    }
    public PlayerMovementBaseState grounded()
    {
        return new PlayerGroundState(context, this);
    }

}
