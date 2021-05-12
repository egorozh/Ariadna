namespace Ariadna
{
    /// <summary>
    /// An enum representing the result of a Message Dialog.
    /// </summary>
    public enum AriadnaMessageDialogResult
    {
        Canceled = -1,
        Negative = 0,
        Affirmative = 1,
        FirstAuxiliary,
        SecondAuxiliary
    }
}