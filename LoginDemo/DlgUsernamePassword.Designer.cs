namespace InspectLoginDemo
{
   partial class DlgUsernamePassword
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
         this._textUsername = new DevExpress.XtraEditors.TextEdit();
         this._btnCancel = new DevExpress.XtraEditors.SimpleButton();
         this._btnOkay = new DevExpress.XtraEditors.SimpleButton();
         this._dxValidationProvider = new DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider(this.components);
         this._labelUsername = new DevExpress.XtraEditors.LabelControl();
         this._textPassword = new DevExpress.XtraEditors.TextEdit();
         this._labelPassword = new DevExpress.XtraEditors.LabelControl();
         ((System.ComponentModel.ISupportInitialize)(this._textUsername.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this._dxValidationProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this._textPassword.Properties)).BeginInit();
         this.SuspendLayout();
         // 
         // _textUsername
         // 
         this._textUsername.Location = new System.Drawing.Point(123, 21);
         this._textUsername.Margin = new System.Windows.Forms.Padding(5);
         this._textUsername.Name = "_textUsername";
         this._textUsername.Size = new System.Drawing.Size(610, 38);
         this._textUsername.TabIndex = 1;
         // 
         // _btnCancel
         // 
         this._btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
         this._btnCancel.Location = new System.Drawing.Point(608, 131);
         this._btnCancel.Margin = new System.Windows.Forms.Padding(5);
         this._btnCancel.Name = "_btnCancel";
         this._btnCancel.Size = new System.Drawing.Size(125, 41);
         this._btnCancel.TabIndex = 5;
         this._btnCancel.Text = "&Cancel";
         // 
         // _btnOkay
         // 
         this._btnOkay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
         this._btnOkay.Location = new System.Drawing.Point(473, 131);
         this._btnOkay.Margin = new System.Windows.Forms.Padding(5);
         this._btnOkay.Name = "_btnOkay";
         this._btnOkay.Size = new System.Drawing.Size(125, 41);
         this._btnOkay.TabIndex = 4;
         this._btnOkay.Text = "&OK";
         this._btnOkay.Click += new System.EventHandler(this._btnOkay_Click);
         // 
         // _labelUsername
         // 
         this._labelUsername.Location = new System.Drawing.Point(24, 28);
         this._labelUsername.Name = "_labelUsername";
         this._labelUsername.Size = new System.Drawing.Size(84, 23);
         this._labelUsername.TabIndex = 0;
         this._labelUsername.Text = "Username";
         // 
         // _textPassword
         // 
         this._textPassword.Location = new System.Drawing.Point(123, 69);
         this._textPassword.Margin = new System.Windows.Forms.Padding(5);
         this._textPassword.Name = "_textPassword";
         this._textPassword.Properties.UseSystemPasswordChar = true;
         this._textPassword.Size = new System.Drawing.Size(610, 38);
         this._textPassword.TabIndex = 3;
         // 
         // _labelPassword
         // 
         this._labelPassword.Location = new System.Drawing.Point(24, 76);
         this._labelPassword.Name = "_labelPassword";
         this._labelPassword.Size = new System.Drawing.Size(78, 23);
         this._labelPassword.TabIndex = 2;
         this._labelPassword.Text = "Password";
         // 
         // DlgUsernamePassword
         // 
         this.AcceptButton = this._btnOkay;
         this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 23F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.CancelButton = this._btnCancel;
         this.ClientSize = new System.Drawing.Size(753, 192);
         this.Controls.Add(this._labelPassword);
         this.Controls.Add(this._labelUsername);
         this.Controls.Add(this._btnOkay);
         this.Controls.Add(this._btnCancel);
         this.Controls.Add(this._textPassword);
         this.Controls.Add(this._textUsername);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
         this.Margin = new System.Windows.Forms.Padding(5);
         this.MaximizeBox = false;
         this.MinimizeBox = false;
         this.Name = "DlgUsernamePassword";
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
         this.Text = "Please enter Username/Password";
         ((System.ComponentModel.ISupportInitialize)(this._textUsername.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this._dxValidationProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this._textPassword.Properties)).EndInit();
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private DevExpress.XtraEditors.TextEdit _textUsername;
      private DevExpress.XtraEditors.SimpleButton _btnCancel;
      private DevExpress.XtraEditors.SimpleButton _btnOkay;
      private DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider _dxValidationProvider;
      private DevExpress.XtraEditors.LabelControl _labelUsername;
      private DevExpress.XtraEditors.TextEdit _textPassword;
      private DevExpress.XtraEditors.LabelControl _labelPassword;
   }
}