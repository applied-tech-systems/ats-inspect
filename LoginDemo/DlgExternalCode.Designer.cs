namespace InspectLoginDemo
{
   partial class DlgExternalCode
   {
      /// <summary>
      /// Required designer variable.
      /// </summary>
      private System.ComponentModel.IContainer components = null;

      /// <summary>
      /// Clean up any resources being used.
      /// </summary>
      /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
      protected override void Dispose(bool disposing)
      {
         if (disposing && (components != null))
         {
            components.Dispose();
         }
         base.Dispose(disposing);
      }

      #region Windows Form Designer generated code

      /// <summary>
      /// Required method for Designer support - do not modify
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
         this.components = new System.ComponentModel.Container();
         this._textExternalCode = new DevExpress.XtraEditors.TextEdit();
         this._btnCancel = new DevExpress.XtraEditors.SimpleButton();
         this._btnOkay = new DevExpress.XtraEditors.SimpleButton();
         this._dxValidationProvider = new DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider(this.components);
         ((System.ComponentModel.ISupportInitialize)(this._textExternalCode.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this._dxValidationProvider)).BeginInit();
         this.SuspendLayout();
         // 
         // _textExternalCode
         // 
         this._textExternalCode.Location = new System.Drawing.Point(20, 21);
         this._textExternalCode.Margin = new System.Windows.Forms.Padding(5);
         this._textExternalCode.Name = "_textExternalCode";
         this._textExternalCode.Properties.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
         this._textExternalCode.Size = new System.Drawing.Size(713, 38);
         this._textExternalCode.TabIndex = 0;
         // 
         // _btnCancel
         // 
         this._btnCancel.Location = new System.Drawing.Point(608, 78);
         this._btnCancel.Margin = new System.Windows.Forms.Padding(5);
         this._btnCancel.Name = "_btnCancel";
         this._btnCancel.Size = new System.Drawing.Size(125, 41);
         this._btnCancel.TabIndex = 2;
         this._btnCancel.Text = "&Cancel";
         // 
         // _btnOkay
         // 
         this._btnOkay.Location = new System.Drawing.Point(473, 78);
         this._btnOkay.Margin = new System.Windows.Forms.Padding(5);
         this._btnOkay.Name = "_btnOkay";
         this._btnOkay.Size = new System.Drawing.Size(125, 41);
         this._btnOkay.TabIndex = 1;
         this._btnOkay.Text = "&OK";
         this._btnOkay.Click += new System.EventHandler(this._btnOkay_Click);
         // 
         // DlgExternalCode
         // 
         this.AcceptButton = this._btnOkay;
         this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 23F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.CancelButton = this._btnCancel;
         this.ClientSize = new System.Drawing.Size(753, 139);
         this.Controls.Add(this._btnOkay);
         this.Controls.Add(this._btnCancel);
         this.Controls.Add(this._textExternalCode);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
         this.Margin = new System.Windows.Forms.Padding(5);
         this.MaximizeBox = false;
         this.MinimizeBox = false;
         this.Name = "DlgExternalCode";
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
         this.Text = "Please enter External Code";
         ((System.ComponentModel.ISupportInitialize)(this._textExternalCode.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this._dxValidationProvider)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private DevExpress.XtraEditors.TextEdit _textExternalCode;
      private DevExpress.XtraEditors.SimpleButton _btnCancel;
      private DevExpress.XtraEditors.SimpleButton _btnOkay;
      private DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider _dxValidationProvider;
   }
}