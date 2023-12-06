namespace UMSV
{
    partial class PortSipAdapter
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PortSipAdapter));
            this.Core = new AxPortSIPCoreLibLib.AxPortSIPCoreLib();
            this.Device = new AxDeviceManagerLibLib.AxDeviceManagerLib();
            ((System.ComponentModel.ISupportInitialize)(this.Core)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Device)).BeginInit();
            this.SuspendLayout();
            // 
            // Core
            // 
            this.Core.Enabled = true;
            this.Core.Location = new System.Drawing.Point(3, 3);
            this.Core.Name = "Core";
            this.Core.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("Core.OcxState")));
            this.Core.Size = new System.Drawing.Size(100, 50);
            this.Core.TabIndex = 0;
            // 
            // Device
            // 
            this.Device.Enabled = true;
            this.Device.Location = new System.Drawing.Point(3, 59);
            this.Device.Name = "Device";
            this.Device.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("Device.OcxState")));
            this.Device.Size = new System.Drawing.Size(100, 50);
            this.Device.TabIndex = 1;
            // 
            // PortSipAdapter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.Device);
            this.Controls.Add(this.Core);
            this.Name = "PortSipAdapter";
            this.Size = new System.Drawing.Size(0, 0);
            ((System.ComponentModel.ISupportInitialize)(this.Core)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Device)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public AxPortSIPCoreLibLib.AxPortSIPCoreLib Core;
        public AxDeviceManagerLibLib.AxDeviceManagerLib Device;
    }
}
