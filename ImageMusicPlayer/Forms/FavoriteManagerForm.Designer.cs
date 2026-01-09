namespace ImageMusicPlayer
{
    partial class FavoriteManagerForm
    {
        private System.Windows.Forms.ListView listViewFavorites;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnImport; // 新增导入按钮
        private System.Windows.Forms.Button btnDeleteImages; // 新增按钮：删除该文件夹图片
        private System.Windows.Forms.Button btnClose;

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelButtons;

        private System.Windows.Forms.Button btnAddFolder;
        private System.Windows.Forms.Button btnAddImage;

        private void InitializeComponent()
        {
            listViewFavorites = new ListView();
            btnAdd = new Button();
            btnDelete = new Button();
            btnImport = new Button();
            btnClose = new Button();
            btnDeleteImages = new Button();
            flowLayoutPanelButtons = new FlowLayoutPanel();
            btnAddFolder = new Button();
            btnAddImage = new Button();
            flowLayoutPanelButtons.SuspendLayout();
            SuspendLayout();
            // 
            // listViewFavorites
            // 
            listViewFavorites.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            listViewFavorites.Location = new Point(12, 12);
            listViewFavorites.Name = "listViewFavorites";
            listViewFavorites.Size = new Size(1698, 518);
            listViewFavorites.TabIndex = 1;
            listViewFavorites.UseCompatibleStateImageBehavior = false;
            listViewFavorites.View = View.List;
            listViewFavorites.DoubleClick += ListViewFavorites_DoubleClick;
            // 
            // btnAdd
            // 
            btnAdd.Location = new Point(203, 3);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new Size(115, 35);
            btnAdd.TabIndex = 2;
            btnAdd.Text = "添加";
            btnAdd.UseVisualStyleBackColor = true;
            btnAdd.Click += BtnAdd_Click;
            // 
            // btnDelete
            // 
            btnDelete.Location = new Point(735, 3);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new Size(89, 35);
            btnDelete.TabIndex = 3;
            btnDelete.Text = "删除";
            btnDelete.UseVisualStyleBackColor = true;
            btnDelete.Click += BtnDelete_Click;
            // 
            // btnImport
            // 
            btnImport.Location = new Point(480, 3);
            btnImport.Name = "btnImport";
            btnImport.Size = new Size(144, 35);
            btnImport.TabIndex = 4;
            btnImport.Text = "导入图片";
            btnImport.UseVisualStyleBackColor = true;
            btnImport.Click += BtnImport_Click;
            // 
            // btnClose
            // 
            btnClose.Location = new Point(830, 3);
            btnClose.Name = "btnClose";
            btnClose.RightToLeft = RightToLeft.Yes;
            btnClose.Size = new Size(110, 35);
            btnClose.TabIndex = 5;
            btnClose.Text = "关闭";
            btnClose.UseVisualStyleBackColor = true;
            btnClose.Click += BtnClose_Click;
            // 
            // btnDeleteImages
            // 
            btnDeleteImages.Location = new Point(630, 3);
            btnDeleteImages.Name = "btnDeleteImages";
            btnDeleteImages.Size = new Size(99, 35);
            btnDeleteImages.TabIndex = 0;
            btnDeleteImages.Text = "删除图片";
            btnDeleteImages.UseVisualStyleBackColor = true;
            btnDeleteImages.Click += BtnDeleteImages_Click;
            // 
            // flowLayoutPanelButtons
            // 
            flowLayoutPanelButtons.Controls.Add(btnAddFolder);
            flowLayoutPanelButtons.Controls.Add(btnAdd);
            flowLayoutPanelButtons.Controls.Add(btnAddImage);
            flowLayoutPanelButtons.Controls.Add(btnImport);
            flowLayoutPanelButtons.Controls.Add(btnDeleteImages);
            flowLayoutPanelButtons.Controls.Add(btnDelete);
            flowLayoutPanelButtons.Controls.Add(btnClose);
            flowLayoutPanelButtons.Location = new Point(12, 536);
            flowLayoutPanelButtons.Name = "flowLayoutPanelButtons";
            flowLayoutPanelButtons.RightToLeft = RightToLeft.No;
            flowLayoutPanelButtons.Size = new Size(1637, 48);
            flowLayoutPanelButtons.TabIndex = 6;
            // 
            // btnAddFolder
            // 
            btnAddFolder.Location = new Point(5, 5);
            btnAddFolder.Margin = new Padding(5);
            btnAddFolder.Name = "btnAddFolder";
            btnAddFolder.Size = new Size(190, 35);
            btnAddFolder.TabIndex = 0;
            btnAddFolder.Text = "添加文件夹收藏";
            btnAddFolder.Click += BtnAddFolder_Click;
            // 
            // btnAddImage
            // 
            btnAddImage.Location = new Point(326, 5);
            btnAddImage.Margin = new Padding(5);
            btnAddImage.Name = "btnAddImage";
            btnAddImage.Size = new Size(146, 35);
            btnAddImage.TabIndex = 1;
            btnAddImage.Text = "添加图片收藏";
            btnAddImage.Click += BtnAddImage_Click;
            // 
            // FavoriteManagerForm
            // 
            ClientSize = new Size(1722, 596);
            Controls.Add(listViewFavorites);
            Controls.Add(flowLayoutPanelButtons);
            Name = "FavoriteManagerForm";
            Text = "收藏管理";
            flowLayoutPanelButtons.ResumeLayout(false);
            ResumeLayout(false);
        }
    }
}
