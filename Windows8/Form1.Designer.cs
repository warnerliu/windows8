namespace Windows8
{
    partial class Form1
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
            this.button1 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.reset = new System.Windows.Forms.Button();
            this.pic1 = new System.Windows.Forms.Button();
            this.pic2 = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pic3 = new System.Windows.Forms.Button();
            this.commit = new System.Windows.Forms.Button();
            this.testList = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.add1 = new System.Windows.Forms.Button();
            this.add2 = new System.Windows.Forms.Button();
            this.pointsProcess = new System.Windows.Forms.Button();
            this.testCombine = new System.Windows.Forms.Button();
            this.hack = new System.Windows.Forms.Button();
            this.pointCluster = new System.Windows.Forms.Button();
            this.lineCluster = new System.Windows.Forms.Button();
            this.circleCluster = new System.Windows.Forms.Button();
            this.partCluster = new System.Windows.Forms.Button();
            this.processResult = new System.Windows.Forms.Button();
            this.labStudy = new System.Windows.Forms.Button();
            this.labCluster = new System.Windows.Forms.Button();
            this.labAllStudy = new System.Windows.Forms.Button();
            this.labLineSegments = new System.Windows.Forms.Button();
            this.labCircle = new System.Windows.Forms.Button();
            this.pic2Cluster = new System.Windows.Forms.Button();
            this.pic3Cluster = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(1168, 69);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "注册完成";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(22, 727);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 3;
            this.button3.Text = "check";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // reset
            // 
            this.reset.Location = new System.Drawing.Point(1168, 202);
            this.reset.Name = "reset";
            this.reset.Size = new System.Drawing.Size(75, 23);
            this.reset.TabIndex = 4;
            this.reset.Text = "取消";
            this.reset.UseVisualStyleBackColor = true;
            this.reset.Click += new System.EventHandler(this.reset_Click);
            // 
            // pic1
            // 
            this.pic1.Location = new System.Drawing.Point(12, 69);
            this.pic1.Name = "pic1";
            this.pic1.Size = new System.Drawing.Size(75, 23);
            this.pic1.TabIndex = 5;
            this.pic1.Text = "picture1";
            this.pic1.UseVisualStyleBackColor = true;
            this.pic1.Click += new System.EventHandler(this.pic1_Click);
            // 
            // pic2
            // 
            this.pic2.Location = new System.Drawing.Point(13, 117);
            this.pic2.Name = "pic2";
            this.pic2.Size = new System.Drawing.Size(75, 23);
            this.pic2.TabIndex = 6;
            this.pic2.Text = "picture2";
            this.pic2.UseVisualStyleBackColor = true;
            this.pic2.Click += new System.EventHandler(this.pic2_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Control;
            this.panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.panel1.Location = new System.Drawing.Point(100, 44);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1024, 548);
            this.panel1.TabIndex = 0;
            this.panel1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseDown);
            this.panel1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseMove);
            this.panel1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseUp);
            // 
            // pic3
            // 
            this.pic3.Location = new System.Drawing.Point(13, 164);
            this.pic3.Name = "pic3";
            this.pic3.Size = new System.Drawing.Size(75, 23);
            this.pic3.TabIndex = 7;
            this.pic3.Text = "picture3";
            this.pic3.UseVisualStyleBackColor = true;
            this.pic3.Click += new System.EventHandler(this.pic3_Click);
            // 
            // commit
            // 
            this.commit.Location = new System.Drawing.Point(1168, 133);
            this.commit.Name = "commit";
            this.commit.Size = new System.Drawing.Size(75, 23);
            this.commit.TabIndex = 8;
            this.commit.Text = "认证完成";
            this.commit.UseVisualStyleBackColor = true;
            this.commit.Click += new System.EventHandler(this.commit_Click);
            // 
            // testList
            // 
            this.testList.Location = new System.Drawing.Point(13, 305);
            this.testList.Name = "testList";
            this.testList.Size = new System.Drawing.Size(75, 23);
            this.testList.TabIndex = 9;
            this.testList.Text = "Test";
            this.testList.UseVisualStyleBackColor = true;
            this.testList.Click += new System.EventHandler(this.testList_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(425, 727);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(96, 23);
            this.button2.TabIndex = 10;
            this.button2.Text = "圆聚簇";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // add1
            // 
            this.add1.Location = new System.Drawing.Point(148, 727);
            this.add1.Name = "add1";
            this.add1.Size = new System.Drawing.Size(96, 23);
            this.add1.TabIndex = 11;
            this.add1.Text = "点聚簇";
            this.add1.UseVisualStyleBackColor = true;
            this.add1.Click += new System.EventHandler(this.add1_Click);
            // 
            // add2
            // 
            this.add2.Location = new System.Drawing.Point(286, 727);
            this.add2.Name = "add2";
            this.add2.Size = new System.Drawing.Size(96, 23);
            this.add2.TabIndex = 12;
            this.add2.Text = "线聚簇";
            this.add2.UseVisualStyleBackColor = true;
            this.add2.Click += new System.EventHandler(this.add2_Click);
            // 
            // pointsProcess
            // 
            this.pointsProcess.Location = new System.Drawing.Point(683, 698);
            this.pointsProcess.Name = "pointsProcess";
            this.pointsProcess.Size = new System.Drawing.Size(75, 23);
            this.pointsProcess.TabIndex = 13;
            this.pointsProcess.Text = "全部聚簇";
            this.pointsProcess.UseVisualStyleBackColor = true;
            this.pointsProcess.Click += new System.EventHandler(this.pointsProcess_Click);
            // 
            // testCombine
            // 
            this.testCombine.Location = new System.Drawing.Point(886, 727);
            this.testCombine.Name = "testCombine";
            this.testCombine.Size = new System.Drawing.Size(75, 23);
            this.testCombine.TabIndex = 14;
            this.testCombine.Text = "组合算法";
            this.testCombine.UseVisualStyleBackColor = true;
            this.testCombine.Click += new System.EventHandler(this.testCombine_Click);
            // 
            // hack
            // 
            this.hack.Location = new System.Drawing.Point(1049, 727);
            this.hack.Name = "hack";
            this.hack.Size = new System.Drawing.Size(75, 23);
            this.hack.TabIndex = 15;
            this.hack.Text = "破解";
            this.hack.UseVisualStyleBackColor = true;
            this.hack.Click += new System.EventHandler(this.hack_Click);
            // 
            // pointCluster
            // 
            this.pointCluster.Location = new System.Drawing.Point(148, 698);
            this.pointCluster.Name = "pointCluster";
            this.pointCluster.Size = new System.Drawing.Size(96, 23);
            this.pointCluster.TabIndex = 16;
            this.pointCluster.Text = "pointCluster";
            this.pointCluster.UseVisualStyleBackColor = true;
            this.pointCluster.Click += new System.EventHandler(this.pointCluster_Click);
            // 
            // lineCluster
            // 
            this.lineCluster.Location = new System.Drawing.Point(286, 698);
            this.lineCluster.Name = "lineCluster";
            this.lineCluster.Size = new System.Drawing.Size(96, 23);
            this.lineCluster.TabIndex = 17;
            this.lineCluster.Text = "lineSegment";
            this.lineCluster.UseVisualStyleBackColor = true;
            this.lineCluster.Click += new System.EventHandler(this.lineCluster_Click);
            // 
            // circleCluster
            // 
            this.circleCluster.Location = new System.Drawing.Point(425, 698);
            this.circleCluster.Name = "circleCluster";
            this.circleCluster.Size = new System.Drawing.Size(96, 23);
            this.circleCluster.TabIndex = 18;
            this.circleCluster.Text = "circle";
            this.circleCluster.UseVisualStyleBackColor = true;
            this.circleCluster.Click += new System.EventHandler(this.circleCluster_Click);
            // 
            // partCluster
            // 
            this.partCluster.Location = new System.Drawing.Point(683, 726);
            this.partCluster.Name = "partCluster";
            this.partCluster.Size = new System.Drawing.Size(75, 23);
            this.partCluster.TabIndex = 19;
            this.partCluster.Text = "部分聚簇";
            this.partCluster.UseVisualStyleBackColor = true;
            this.partCluster.Click += new System.EventHandler(this.partCluster_Click);
            // 
            // processResult
            // 
            this.processResult.Location = new System.Drawing.Point(1168, 466);
            this.processResult.Name = "processResult";
            this.processResult.Size = new System.Drawing.Size(75, 23);
            this.processResult.TabIndex = 20;
            this.processResult.Text = "结果处理";
            this.processResult.UseVisualStyleBackColor = true;
            this.processResult.Click += new System.EventHandler(this.processResult_Click);
            // 
            // labStudy
            // 
            this.labStudy.Location = new System.Drawing.Point(307, 638);
            this.labStudy.Name = "labStudy";
            this.labStudy.Size = new System.Drawing.Size(75, 23);
            this.labStudy.TabIndex = 21;
            this.labStudy.Text = "labPoints";
            this.labStudy.UseVisualStyleBackColor = true;
            this.labStudy.Click += new System.EventHandler(this.labStudy_Click);
            // 
            // labCluster
            // 
            this.labCluster.Location = new System.Drawing.Point(719, 638);
            this.labCluster.Name = "labCluster";
            this.labCluster.Size = new System.Drawing.Size(96, 23);
            this.labCluster.TabIndex = 22;
            this.labCluster.Text = "lab部分聚簇";
            this.labCluster.UseVisualStyleBackColor = true;
            this.labCluster.Click += new System.EventHandler(this.labCluster_Click);
            // 
            // labAllStudy
            // 
            this.labAllStudy.Location = new System.Drawing.Point(719, 609);
            this.labAllStudy.Name = "labAllStudy";
            this.labAllStudy.Size = new System.Drawing.Size(96, 23);
            this.labAllStudy.TabIndex = 23;
            this.labAllStudy.Text = "lab全部聚簇";
            this.labAllStudy.UseVisualStyleBackColor = true;
            this.labAllStudy.Click += new System.EventHandler(this.labAllStudy_Click);
            // 
            // labLineSegments
            // 
            this.labLineSegments.Location = new System.Drawing.Point(435, 638);
            this.labLineSegments.Name = "labLineSegments";
            this.labLineSegments.Size = new System.Drawing.Size(75, 23);
            this.labLineSegments.TabIndex = 24;
            this.labLineSegments.Text = "labLine";
            this.labLineSegments.UseVisualStyleBackColor = true;
            this.labLineSegments.Click += new System.EventHandler(this.labLineSegments_Click);
            // 
            // labCircle
            // 
            this.labCircle.Location = new System.Drawing.Point(564, 638);
            this.labCircle.Name = "labCircle";
            this.labCircle.Size = new System.Drawing.Size(75, 23);
            this.labCircle.TabIndex = 25;
            this.labCircle.Text = "labCircle";
            this.labCircle.UseVisualStyleBackColor = true;
            this.labCircle.Click += new System.EventHandler(this.labCircle_Click);
            // 
            // pic2Cluster
            // 
            this.pic2Cluster.Location = new System.Drawing.Point(854, 609);
            this.pic2Cluster.Name = "pic2Cluster";
            this.pic2Cluster.Size = new System.Drawing.Size(75, 23);
            this.pic2Cluster.TabIndex = 26;
            this.pic2Cluster.Text = "pic2";
            this.pic2Cluster.UseVisualStyleBackColor = true;
            this.pic2Cluster.Click += new System.EventHandler(this.pic2Cluster_Click);
            // 
            // pic3Cluster
            // 
            this.pic3Cluster.Location = new System.Drawing.Point(963, 609);
            this.pic3Cluster.Name = "pic3Cluster";
            this.pic3Cluster.Size = new System.Drawing.Size(75, 23);
            this.pic3Cluster.TabIndex = 27;
            this.pic3Cluster.Text = "pic3";
            this.pic3Cluster.UseVisualStyleBackColor = true;
            this.pic3Cluster.Click += new System.EventHandler(this.pic3Cluster_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1264, 762);
            this.Controls.Add(this.pic3Cluster);
            this.Controls.Add(this.pic2Cluster);
            this.Controls.Add(this.labCircle);
            this.Controls.Add(this.labLineSegments);
            this.Controls.Add(this.labAllStudy);
            this.Controls.Add(this.labCluster);
            this.Controls.Add(this.labStudy);
            this.Controls.Add(this.processResult);
            this.Controls.Add(this.partCluster);
            this.Controls.Add(this.circleCluster);
            this.Controls.Add(this.lineCluster);
            this.Controls.Add(this.pointCluster);
            this.Controls.Add(this.hack);
            this.Controls.Add(this.testCombine);
            this.Controls.Add(this.pointsProcess);
            this.Controls.Add(this.add2);
            this.Controls.Add(this.add1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.testList);
            this.Controls.Add(this.commit);
            this.Controls.Add(this.pic3);
            this.Controls.Add(this.pic2);
            this.Controls.Add(this.pic1);
            this.Controls.Add(this.reset);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.panel1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button reset;
        private System.Windows.Forms.Button pic1;
        private System.Windows.Forms.Button pic2;
        private System.Windows.Forms.Button pic3;
        private System.Windows.Forms.Button commit;
        private System.Windows.Forms.Button testList;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button add1;
        private System.Windows.Forms.Button add2;
        private System.Windows.Forms.Button pointsProcess;
        private System.Windows.Forms.Button testCombine;
        private System.Windows.Forms.Button hack;
        private System.Windows.Forms.Button pointCluster;
        private System.Windows.Forms.Button lineCluster;
        private System.Windows.Forms.Button circleCluster;
        private System.Windows.Forms.Button partCluster;
        private System.Windows.Forms.Button processResult;
        private System.Windows.Forms.Button labStudy;
        private System.Windows.Forms.Button labCluster;
        private System.Windows.Forms.Button labAllStudy;
        private System.Windows.Forms.Button labLineSegments;
        private System.Windows.Forms.Button labCircle;
        private System.Windows.Forms.Button pic2Cluster;
        private System.Windows.Forms.Button pic3Cluster;
    }
}

