namespace ATS.Inspect.Demo;

partial class DemoControl
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

   #region Component Designer generated code

   /// <summary>
   /// Required method for Designer support - do not modify 
   /// the contents of this method with the code editor.
   /// </summary>
   private void InitializeComponent()
   {
      _btnClickMe = new Button();
      SuspendLayout();
      // 
      // _btnClickMe
      // 
      _btnClickMe.Dock = DockStyle.Fill;
      _btnClickMe.Location = new Point(0, 0);
      _btnClickMe.Margin = new Padding(5, 6, 5, 6);
      _btnClickMe.Name = "_btnClickMe";
      _btnClickMe.Size = new Size(479, 406);
      _btnClickMe.TabIndex = 0;
      _btnClickMe.Text = "Click me!";
      _btnClickMe.UseVisualStyleBackColor = true;
      _btnClickMe.Click += _btnClickMe_Click;
      // 
      // DemoControl
      // 
      AutoScaleDimensions = new SizeF(12F, 30F);
      AutoScaleMode = AutoScaleMode.Font;
      Controls.Add(_btnClickMe);
      Margin = new Padding(5, 6, 5, 6);
      Name = "DemoControl";
      Size = new Size(479, 406);
      ResumeLayout(false);
   }

   #endregion

   private Button _btnClickMe;
}