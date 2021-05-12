namespace Ariadna
{
    public class UiQuickActionItem
    {
        public UiSplitButtonItem? SplitButtonItem { get; set; }

        public string Header { get; set; }

        public string Id { get; set; }
        public string Description { get; set; }
        public string DisableReason { get; set; }

        public string AlternativeHeader { get; set; }
        public string AlternativeDescription { get; set; }
        public string AlternativeDisableReason { get; set; }
    }
}