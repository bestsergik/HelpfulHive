namespace HelpfulHive.Services
{
    public class AnimationService
    {
        public event Action<string> OnAnimationRequested;

        public void RequestAnimation(string animationName)
        {
            OnAnimationRequested?.Invoke(animationName);
        }
    }
}
