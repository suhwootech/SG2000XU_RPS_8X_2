namespace SG2000X
{
    partial class vwEqu_05Out
    {
        /// <summary> 
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            // Dispose시 함수내의 모든 변수 해제
            _Release();

            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 구성 요소 디자이너에서 생성한 코드

        /// <summary> 
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(vwEqu_05Out));
            this.pnlIO_Out_OnLoader = new System.Windows.Forms.Panel();
            this.pnlIO_Out_OffLoader = new System.Windows.Forms.Panel();
            this.pnlIO_Out_Main = new System.Windows.Forms.Panel();
            this.lblIO_Out_OffLoader = new System.Windows.Forms.Label();
            this.lblIO_Out_OnLoader = new System.Windows.Forms.Label();
            this.lblIO_Out_Main = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // pnlIO_Out_OnLoader
            // 
            resources.ApplyResources(this.pnlIO_Out_OnLoader, "pnlIO_Out_OnLoader");
            this.pnlIO_Out_OnLoader.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlIO_Out_OnLoader.Name = "pnlIO_Out_OnLoader";
            // 
            // pnlIO_Out_OffLoader
            // 
            resources.ApplyResources(this.pnlIO_Out_OffLoader, "pnlIO_Out_OffLoader");
            this.pnlIO_Out_OffLoader.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlIO_Out_OffLoader.Name = "pnlIO_Out_OffLoader";
            // 
            // pnlIO_Out_Main
            // 
            resources.ApplyResources(this.pnlIO_Out_Main, "pnlIO_Out_Main");
            this.pnlIO_Out_Main.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlIO_Out_Main.Name = "pnlIO_Out_Main";
            // 
            // lblIO_Out_OffLoader
            // 
            resources.ApplyResources(this.lblIO_Out_OffLoader, "lblIO_Out_OffLoader");
            this.lblIO_Out_OffLoader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(199)))), ((int)(((byte)(201)))));
            this.lblIO_Out_OffLoader.ForeColor = System.Drawing.Color.White;
            this.lblIO_Out_OffLoader.Name = "lblIO_Out_OffLoader";
            // 
            // lblIO_Out_OnLoader
            // 
            resources.ApplyResources(this.lblIO_Out_OnLoader, "lblIO_Out_OnLoader");
            this.lblIO_Out_OnLoader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(199)))), ((int)(((byte)(201)))));
            this.lblIO_Out_OnLoader.ForeColor = System.Drawing.Color.White;
            this.lblIO_Out_OnLoader.Name = "lblIO_Out_OnLoader";
            // 
            // lblIO_Out_Main
            // 
            resources.ApplyResources(this.lblIO_Out_Main, "lblIO_Out_Main");
            this.lblIO_Out_Main.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(199)))), ((int)(((byte)(201)))));
            this.lblIO_Out_Main.ForeColor = System.Drawing.Color.White;
            this.lblIO_Out_Main.Name = "lblIO_Out_Main";
            // 
            // vwEqu_05Out
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.pnlIO_Out_OnLoader);
            this.Controls.Add(this.pnlIO_Out_OffLoader);
            this.Controls.Add(this.pnlIO_Out_Main);
            this.Controls.Add(this.lblIO_Out_OffLoader);
            this.Controls.Add(this.lblIO_Out_OnLoader);
            this.Controls.Add(this.lblIO_Out_Main);
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.Color.Black;
            this.Name = "vwEqu_05Out";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlIO_Out_OnLoader;
        private System.Windows.Forms.Panel pnlIO_Out_OffLoader;
        private System.Windows.Forms.Panel pnlIO_Out_Main;
        private System.Windows.Forms.Label lblIO_Out_OffLoader;
        private System.Windows.Forms.Label lblIO_Out_OnLoader;
        private System.Windows.Forms.Label lblIO_Out_Main;
    }
}
