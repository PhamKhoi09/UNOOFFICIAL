namespace UnoOnline
{
    partial class FinalRanking
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FinalRanking));
            this.NameTop1 = new System.Windows.Forms.Label();
            this.Point1 = new System.Windows.Forms.Label();
            this.NameTop2 = new System.Windows.Forms.Label();
            this.NameTop3 = new System.Windows.Forms.Label();
            this.NameTop4 = new System.Windows.Forms.Label();
            this.Point2 = new System.Windows.Forms.Label();
            this.Point3 = new System.Windows.Forms.Label();
            this.Point4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // NameTop1
            // 
            this.NameTop1.AutoSize = true;
            this.NameTop1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NameTop1.Location = new System.Drawing.Point(240, 275);
            this.NameTop1.Name = "NameTop1";
            this.NameTop1.Size = new System.Drawing.Size(126, 25);
            this.NameTop1.TabIndex = 0;
            this.NameTop1.Text = "Ng chơi top 1";
            // 
            // Point1
            // 
            this.Point1.AutoSize = true;
            this.Point1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Point1.Location = new System.Drawing.Point(270, 314);
            this.Point1.Name = "Point1";
            this.Point1.Size = new System.Drawing.Size(57, 25);
            this.Point1.TabIndex = 1;
            this.Point1.Text = "Điểm";
            // 
            // NameTop2
            // 
            this.NameTop2.AutoSize = true;
            this.NameTop2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NameTop2.Location = new System.Drawing.Point(384, 363);
            this.NameTop2.Name = "NameTop2";
            this.NameTop2.Size = new System.Drawing.Size(126, 25);
            this.NameTop2.TabIndex = 2;
            this.NameTop2.Text = "Ng chơi top 1";
            // 
            // NameTop3
            // 
            this.NameTop3.AutoSize = true;
            this.NameTop3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NameTop3.Location = new System.Drawing.Point(152, 436);
            this.NameTop3.Name = "NameTop3";
            this.NameTop3.Size = new System.Drawing.Size(126, 25);
            this.NameTop3.TabIndex = 3;
            this.NameTop3.Text = "Ng chơi top 1";
            // 
            // NameTop4
            // 
            this.NameTop4.AutoSize = true;
            this.NameTop4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NameTop4.Location = new System.Drawing.Point(52, 436);
            this.NameTop4.Name = "NameTop4";
            this.NameTop4.Size = new System.Drawing.Size(126, 25);
            this.NameTop4.TabIndex = 4;
            this.NameTop4.Text = "Ng chơi top 1";
            // 
            // Point2
            // 
            this.Point2.AutoSize = true;
            this.Point2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Point2.Location = new System.Drawing.Point(422, 407);
            this.Point2.Name = "Point2";
            this.Point2.Size = new System.Drawing.Size(57, 25);
            this.Point2.TabIndex = 5;
            this.Point2.Text = "Điểm";
            // 
            // Point3
            // 
            this.Point3.AutoSize = true;
            this.Point3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Point3.Location = new System.Drawing.Point(176, 475);
            this.Point3.Name = "Point3";
            this.Point3.Size = new System.Drawing.Size(57, 25);
            this.Point3.TabIndex = 6;
            this.Point3.Text = "Điểm";
            // 
            // Point4
            // 
            this.Point4.AutoSize = true;
            this.Point4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Point4.Location = new System.Drawing.Point(81, 475);
            this.Point4.Name = "Point4";
            this.Point4.Size = new System.Drawing.Size(57, 25);
            this.Point4.TabIndex = 7;
            this.Point4.Text = "Điểm";
            // 
            // FinalRanking
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.ClientSize = new System.Drawing.Size(600, 555);
            this.Controls.Add(this.Point4);
            this.Controls.Add(this.Point3);
            this.Controls.Add(this.Point2);
            this.Controls.Add(this.NameTop4);
            this.Controls.Add(this.NameTop3);
            this.Controls.Add(this.NameTop2);
            this.Controls.Add(this.Point1);
            this.Controls.Add(this.NameTop1);
            this.DoubleBuffered = true;
            this.Name = "FinalRanking";
            this.Text = "FinalRanking";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FinalRanking_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label NameTop1;
        private System.Windows.Forms.Label Point1;
        private System.Windows.Forms.Label NameTop2;
        private System.Windows.Forms.Label NameTop3;
        private System.Windows.Forms.Label NameTop4;
        private System.Windows.Forms.Label Point2;
        private System.Windows.Forms.Label Point3;
        private System.Windows.Forms.Label Point4;
    }
}