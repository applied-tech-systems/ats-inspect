using System;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace InspectLoginDemo;

public partial class DlgExternalCode : XtraForm
{
   #region Constructors

   public DlgExternalCode()
   {
      InitializeComponent();
   }

   #endregion

   #region Properties

   #region Public Properties

   [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
   public string ExternalCode
   {
      get => _textExternalCode.Text.Trim();
      set => _textExternalCode.Text = value?.Trim();
   }

   #endregion

   #endregion

   #region Event Handlers

   #region Button Events

   private void _btnOkay_Click(object sender, EventArgs e)
   {
      if (string.IsNullOrEmpty(_textExternalCode.Text.Trim()))
      {
         return;
      }

      DialogResult = DialogResult.OK;
      Close();
   }

   #endregion

   #endregion
}