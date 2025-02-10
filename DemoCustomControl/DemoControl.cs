namespace ATS.Inspect.Demo;

public partial class DemoControl : UserControl
{
   #region Constructors

   public DemoControl()
   {
      InitializeComponent();
   }

   #endregion

   #region Event Handlers

   #region Button Events

   private void _btnClickMe_Click(object sender, EventArgs e)
   {
      MessageBox.Show("You clicked on the button!");
   }

   #endregion

   #endregion
}