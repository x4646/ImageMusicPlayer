using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using ImageMusicPlayer.Models;
using ImageMusicPlayer.Interfaces;

namespace ImageMusicPlayer
{
    public partial class MusicManagerForm : Form
    {
        private readonly IMusicLibraryManager libraryManager;
        private readonly IPlaylistManager playlistManager;
        private readonly IMusicPlayer musicPlayer;



        public MusicManagerForm(IMusicLibraryManager libMgr, IPlaylistManager plMgr, IMusicPlayer player)
        {
            InitializeComponent();
            libraryManager = libMgr;
            playlistManager = plMgr;
            musicPlayer = player;

            libraryManager.LoadLibrary();
            playlistManager.LoadPlaylist();

            BuildTreeView();
            RefreshPlaylist();
            comboPlayMode.SelectedIndex = 0;

            PlayLast();

            trackBarVolume.Value = ((int)musicPlayer.GetVolume());
            labelVolume.Text = $"音量：{trackBarVolume.Value}%";
        }

        private void trackBarVolume_Scroll(object sender, EventArgs e)
        {
            int volume = trackBarVolume.Value;
            musicPlayer.SetVolume(volume);
            labelVolume.Text = $"音量：{volume}%";
        }

        private void PlayLast()
        {
            string? lastPath = musicPlayer.LoadLastPlayed();
            if (!string.IsNullOrEmpty(lastPath) && File.Exists(lastPath))
            {
                // 若存在该音乐，同时播放列表包含它，恢复播放位置
                if (playlistManager.Playlist.Contains(lastPath))
                {
                    musicPlayer.PlayPathList = playlistManager.Playlist.ToList();
                }
            }
        }

        private void BuildTreeView()
        {
            treeViewLibrary.Nodes.Clear();
            foreach (var root in libraryManager.RootFolders)
            {
                var rootNode = CreateTreeNode(root);
                treeViewLibrary.Nodes.Add(rootNode);
            }
            treeViewLibrary.ExpandAll();
        }

        private TreeNode CreateTreeNode(MusicFolderNode node)
        {
            TreeNode treeNode = new TreeNode(node.Name)
            {
                Tag = node,
                Checked = false
            };

            foreach (var file in node.MusicFiles)
            {
                TreeNode fileNode = new TreeNode(Path.GetFileName(file))
                {
                    Tag = file,
                    Checked = false
                };
                treeNode.Nodes.Add(fileNode);
            }

            foreach (var sub in node.SubFolders)
            {
                treeNode.Nodes.Add(CreateTreeNode(sub));
            }

            return treeNode;
        }

        private void treeViewLibrary_AfterCheck(object? sender, TreeViewEventArgs e)
        {
            treeViewLibrary.AfterCheck -= treeViewLibrary_AfterCheck;

            if (e.Node?.Tag is MusicFolderNode folder)
            {
                // 异步递归打勾或取消
                Task.Run(() => SetSubtreeCheckedAsync(e.Node, e.Node.Checked));
            }
            else if (e.Node?.Tag is string filePath)
            {
                if (e.Node.Checked)
                    playlistManager.Add(filePath);
                else
                    playlistManager.Remove(filePath);

                playlistManager.SavePlaylist();
                Invoke(new Action(RefreshPlaylist));
            }

            treeViewLibrary.AfterCheck += treeViewLibrary_AfterCheck;
        }


        private void SetSubtreeCheckedAsync(TreeNode node, bool isChecked)
        {
            foreach (TreeNode child in node.Nodes)
            {
                // 设置 UI 属性必须在主线程执行
                this.Invoke(new Action(() => child.Checked = isChecked));

                if (child.Tag is string file)
                {
                    if (isChecked)
                        playlistManager.Add(file);
                    else
                        playlistManager.Remove(file);
                }
                else if (child.Nodes.Count > 0)
                {
                    SetSubtreeCheckedAsync(child, isChecked);
                }
            }

            playlistManager.SavePlaylist();
            this.Invoke(new Action(RefreshPlaylist));
        }


        private void SetSubtreeChecked(TreeNode node, bool isChecked)
        {
            foreach (TreeNode child in node.Nodes)
            {
                child.Checked = isChecked;
                if (child.Tag is string file)
                {
                    if (isChecked)
                        playlistManager.Add(file);
                    else
                        playlistManager.Remove(file);
                }
                else
                {
                    SetSubtreeChecked(child, isChecked);
                }
            }

            playlistManager.SavePlaylist();
            RefreshPlaylist();
        }

        private void btnAddFolder_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    libraryManager.AddFolder(dialog.SelectedPath);
                    BuildTreeView();
                }
            }
        }

        private void btnRemoveFolder_Click(object sender, EventArgs e)
        {
            if (treeViewLibrary.SelectedNode != null && treeViewLibrary.SelectedNode.Tag is MusicFolderNode folder)
            {
                var result = MessageBox.Show("确定移除此文件夹？", "确认", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes && folder.FullPath != null)
                {
                    libraryManager.RemoveFolder(folder.FullPath);
                    BuildTreeView();
                }
            }
            else
            {
                MessageBox.Show("请选择一个根文件夹节点。");
            }
        }

        private void RefreshPlaylist()
        {
            listViewPlaylist.Items.Clear();
            foreach (var file in playlistManager.Playlist)
            {
                listViewPlaylist.Items.Add(new ListViewItem(Path.GetFileName(file)) { Tag = file });
            }
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            if (listViewPlaylist.SelectedItems.Count > 0)
            {
                var path = listViewPlaylist.SelectedItems[0].Tag?.ToString();
                if (path == null) return;
                musicPlayer.PlayPathList = [.. playlistManager.Playlist];
                musicPlayer.Play(path);
                HighlightCurrentPlaying(path);
            }
        }

        private void BtnPause_Click(object sender, EventArgs e)
        {
            musicPlayer.Pause();
        }

        private void BtnNext_Click(object sender, EventArgs e)
        {
            musicPlayer.PlayNext();
            HighlightCurrentPlaying(musicPlayer.CurrentPath);
        }

        private void BtnPrevious_Click(object sender, EventArgs e)
        {
            musicPlayer.PlayPrevious();
            HighlightCurrentPlaying(musicPlayer.CurrentPath);
        }

        private void ListViewPlaylist_DoubleClick(object sender, EventArgs e)
        {
            if (listViewPlaylist.SelectedItems.Count > 0)
            {
                var path = listViewPlaylist.SelectedItems[0].Tag?.ToString();
                if (path == null) return;
                musicPlayer.PlayPathList = [.. playlistManager.Playlist];
                musicPlayer.Play(path);
                HighlightCurrentPlaying(path);
            }
        }

        private void ComboPlayMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboPlayMode.SelectedItem?.ToString())
            {
                case "正序": musicPlayer.Mode = PlayMode.Sequence; break;
                case "循环": musicPlayer.Mode = PlayMode.Loop; break;
                case "随机": musicPlayer.Mode = PlayMode.Random; break;
            }
        }

        private void HighlightCurrentPlaying(string? path)
        {
            if (string.IsNullOrEmpty(path)) return;

            foreach (ListViewItem item in listViewPlaylist.Items)
            {
                if (item.Tag?.ToString() == path)
                {
                    item.Selected = true;
                    item.Focused = true;
                    item.EnsureVisible();
                }
                else
                {
                    item.Selected = false;
                }
            }

            this.Text = $"正在播放：{Path.GetFileName(path)}";
        }


        private void BtnClearPlaylist_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("是否清空整个播放列表？", "确认", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                playlistManager.Clear();
                playlistManager.SavePlaylist();
                RefreshPlaylist();

                // 同时取消 TreeView 中所有勾选的音乐节点
                ClearAllCheckedMusicNodes(treeViewLibrary.Nodes);
            }
        }

        private static void ClearAllCheckedMusicNodes(TreeNodeCollection nodes)
        {
            foreach (TreeNode node in nodes)
            {
                if (node.Tag is string musicPath && node.Checked)
                {
                    node.Checked = false;
                }
                else if (node.Tag is MusicFolderNode || node.Nodes.Count > 0)
                {
                    ClearAllCheckedMusicNodes(node.Nodes);
                }
            }
        }


        private void TreeViewLibrary_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            treeViewLibrary.SelectedNode = e.Node;
        }

        private void btnRemoveMuisc_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listViewPlaylist.SelectedItems)
            {
               listViewPlaylist.Items.Remove(item);
                playlistManager.Remove(item.Tag.ToString());
            }
            playlistManager.SavePlaylist();
        }

    }
}
