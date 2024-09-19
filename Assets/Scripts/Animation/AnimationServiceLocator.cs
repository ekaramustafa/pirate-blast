public static class AnimationServiceLocator
{
    private static IAnimationService _animationService;

    public static IAnimationService GetAnimationService()
    {
        return _animationService ??= new AnimationService();
    }

    public static void SetAnimationService(IAnimationService service)
    {
        _animationService = service;
    }
}
