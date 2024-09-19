public static class AnimationServiceLocator
{
    private static IAnimationService _animationService;
    private static IAnimationService _UIAnimationService;

    public static IAnimationService GetAnimationService()
    {
        return _animationService ??= new AnimationService();
    }

    public static IAnimationService GetUIAnimationService()
    {
        return _UIAnimationService ??= new UIAnimationService();

    }

    public static void SetAnimationService(IAnimationService service)
    {
        _animationService = service;
    }
}
