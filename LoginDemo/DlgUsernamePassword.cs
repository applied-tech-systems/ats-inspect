using System;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace InspectLoginDemo;

public partial class DlgUsernamePassword : XtraForm
{
   #region Constructors

   public DlgUsernamePassword()
   {
      InitializeComponent();
   }

   #endregion

   #region Properties

   #region Public Properties
   
   [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
   public string Username
   {
      get => _textUsername.Text.Trim();
      set => _textUsername.Text = value?.Trim();
   }
   
   [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
   public string Password
   {
      get => _textPassword.Text.Trim();
      set => _textPassword.Text = value?.Trim();
   }

   #endregion

   #endregion

   #region Event Handlers

   #region Button Events

   private void _btnOkay_Click(object sender, EventArgs e)
   {
      if (string.IsNullOrEmpty(_textUsername.Text.Trim()) || string.IsNullOrEmpty(_textPassword.Text.Trim()))
      {
         return;
      }

      DialogResult = DialogResult.OK;
      Close();
   }

   #endregion

   #endregion
}