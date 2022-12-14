using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

public class GreenScreenMenuRenderer : ToolStripProfessionalRenderer
{
    public GreenScreenMenuRenderer() : base(new GreenScreenColors()) { }

    private void SetColors(ToolStripItem item)
    {
        if (item.Selected)
        {
            item.BackColor = Color.Green;
            item.ForeColor = Color.Black;
        }
        else
        {
            item.BackColor = Color.Black;
            item.ForeColor = Color.Green;
        }
    }

    protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
    {
        SetColors(e.Item);
        base.OnRenderItemText(e);
    }

    protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
    {
        SetColors(e.Item);
        base.OnRenderMenuItemBackground(e);
    }

    protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e)
    {
        SetColors(e.Item);
        base.OnRenderButtonBackground(e);
    }

    protected override void OnRenderItemImage(ToolStripItemImageRenderEventArgs e)
    {
        SetColors(e.Item);
        base.OnRenderItemImage(e);
    }

    protected override void OnRenderDropDownButtonBackground(ToolStripItemRenderEventArgs e)
    {
        e.Item.BackColor = Color.Black;
        base.OnRenderDropDownButtonBackground(e);
    }

    protected override void OnRenderItemBackground(ToolStripItemRenderEventArgs e)
    {
        e.Item.BackColor = Color.Black;
        base.OnRenderItemBackground(e);
    }

    protected override void OnRenderToolStripContentPanelBackground(ToolStripContentPanelRenderEventArgs e)
    {
        e.ToolStripContentPanel.BackColor = Color.Black;
        base.OnRenderToolStripContentPanelBackground(e);
    }

    protected override void OnRenderToolStripPanelBackground(ToolStripPanelRenderEventArgs e)
    {
        e.ToolStripPanel.BackColor = Color.Black;
        base.OnRenderToolStripPanelBackground(e);
    }

    protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
    {
        e.ToolStrip.BackColor = Color.Black;
        e.ToolStrip.ForeColor = Color.Green;
        base.OnRenderToolStripBackground(e);
    }
}
