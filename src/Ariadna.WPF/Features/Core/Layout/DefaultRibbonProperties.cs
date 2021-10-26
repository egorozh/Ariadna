namespace Ariadna;

public class DefaultRibbonProperties
{   
    #region Public Properties

    public string Header { get; set; } = "New Feature";
        
    public RibbonItemSize Size { get; set; }
        
    public string Description { get; set; } = "No description set";

    public string DisableReason { get; set; } = "The reason for the inactivity of the command is not specified";

    #endregion
}

public enum RibbonItemSize
{
    Large,

    Middle,

    Small
}