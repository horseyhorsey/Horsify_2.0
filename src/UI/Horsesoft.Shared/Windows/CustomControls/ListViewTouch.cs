using System.Windows.Controls;
using System.Windows.Input;

namespace Horsesoft.Horsify.Resource.Windows.CustomControls
{
    /// <summary>
    /// This overrides the OnManipulationBoundaryFeedback to stop the window moving when user scrolls to top or bottom
    /// </summary>
    public class ListViewTouch : ListView
    {
        protected override void OnManipulationBoundaryFeedback(ManipulationBoundaryFeedbackEventArgs e)
        {
            base.OnManipulationBoundaryFeedback(e);

            e.Handled = true;
        }
    }
}
