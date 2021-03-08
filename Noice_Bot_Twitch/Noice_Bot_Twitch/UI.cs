using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Noice_Bot_Twitch
{
    public partial class UI : Form
    {
        private SettingsModel _settingsModel;

        public UI(SettingsModel settings)
        {
            _settingsModel = settings;
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            textBoxAddBlacklist.KeyDown += new KeyEventHandler(tbBlackListEnterPressed);    //Als Feature, damit man auch per Enter-taste den Namen hinzufügen kann

            //Blacklist
            foreach(string blacklistLine in _settingsModel.Blacklist)
            {
                LinkLabel lb = new LinkLabel();
                lb.Text = blacklistLine;
                lb.LinkClicked += new LinkLabelLinkClickedEventHandler(this.removeBlacklistName);
                flowLayoutPanelBlacklist.Controls.Add(lb);
            }
        }

        private void buttonStartSoundBoard_Click(object sender, EventArgs e)
        {

        }

        private void labelRunning_Click(object sender, EventArgs e)
        {

        }

        private void checkBoxTTS_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void tabPageSettings_Click(object sender, EventArgs e)
        {

        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        //Blacklist Actions

        private void tbBlackListEnterPressed(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;      //preventing Windows error sound on enter key
                buttonAddBlacklist_Click(sender, e);
            }
        }

        private void buttonAddBlacklist_Click(object sender, EventArgs e)
        {
            if (textBoxAddBlacklist.Text.Length == 0) return;

            LinkLabel lb = new LinkLabel();
            lb.Text = textBoxAddBlacklist.Text;
            lb.LinkClicked += new LinkLabelLinkClickedEventHandler(this.removeBlacklistName);
            flowLayoutPanelBlacklist.Controls.Add(lb);
            textBoxAddBlacklist.Text = "";
        }

        private void removeBlacklistName(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LinkLabel clickedLinkLabel = (LinkLabel)sender;
            flowLayoutPanelBlacklist.Controls.Remove(clickedLinkLabel);
        }

        private void buttonSaveBlacklist_Click(object sender, EventArgs e)
        {
            List<string> blackList = new List<string>();
            foreach(Control control in flowLayoutPanelBlacklist.Controls)
            {
                blackList.Add(control.Text);
            }
            _settingsModel.Blacklist = blackList;
            FileManager.SaveFile(FileManager.FileType.Blacklist, blackList);
        }

        //Whitelist Actions
    }
}
