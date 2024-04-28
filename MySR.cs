using System.Windows.Forms;

namespace GogotSharp
{
    public class MySR : ToolStripSystemRenderer
    {

        //Just to resolve bug on menuStrip renderer
        //If the RendererMode is set to System - a white line appears under the menuStrip

        public MySR()
        {
        }

        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
        {
            //base.OnRenderToolStripBorder(e);
        }
    }
}
