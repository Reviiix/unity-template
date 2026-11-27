namespace UnityTemplate.Accessibility
{
    /// <summary>Gives the screen reader a spoken label for the object it sits on, in place of the label worked out from its text.</summary>
    public interface IScreenReaderLabelSource
    {
        string SpokenLabel { get; }
    }
}
