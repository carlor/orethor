using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Speech.Synthesis;
using System.Collections.ObjectModel;

namespace Orhetor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            toolStripComboBox1.SelectedIndex = 0; // don't know why form builder won't do this
            giveMessage("Setting up opener...");
            openFileDialog1.Filter = "Text files (*.txt)|*.txt|All files|*";
            giveMessage("Loading voices...");
            ReadOnlyCollection<InstalledVoice> voices = speech.GetInstalledVoices();
            if (voices.Count == 0)
            {
                showError(
                    "You have no text-to-speech voices installed. Sorry.",
                    "Program won't work..."
                );
                speech.Dispose();
                Application.Exit();
            }
            else
            {
                foreach (InstalledVoice iv in voices)
                {
                    comboBox1.Items.Add(iv.VoiceInfo.Description);
                }
                comboBox1.SelectedItem = voices[0].VoiceInfo.Description;
            }
            speech.SpeakCompleted += new EventHandler<SpeakCompletedEventArgs>(this.speech_done);
            giveMessage("Waiting for you...");
        }

        private SpeechSynthesizer speech = new SpeechSynthesizer();

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            speech.Rate = trackBar1.Value;
        }

        private void trackBar2_ValueChanged(object sender, EventArgs e)
        {
            speech.Volume = trackBar2.Value;
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (toolStripButton1.Text == "Play")
            {
                giveMessage("Preparing for Speech...");
                groupBox2.Enabled = false;
                toolStripComboBox1.Enabled = false;
                toolStripButton1.Text = "Pause";
                speech.SpeakAsync(getTextToPlay());
                giveMessage("Speaking...");
            }
            else if (toolStripButton1.Text == "Pause")
            {
                giveMessage("Pausing...");
                toolStripButton1.Text = "Resume";
                speech.Pause();
                giveMessage("Paused...");
            }
            else
            {
                giveMessage("Resuming...");
                toolStripButton1.Text = "Pause";
                speech.Resume();
                giveMessage("Speaking...");
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            giveMessage("Stopping...");
            int oldvolume = speech.Volume;
            if (toolStripButton1.Text == "Resume")
            {
                // make sure that it dosen't stop on pause
                speech.Volume = 0;
                speech.Resume();
            }
            speech.SpeakAsyncCancelAll();
            speech_done(this, null);
            speech.Volume = oldvolume;
            giveMessage("Stopped.");
        }

        private void speech_done(object sender, SpeakCompletedEventArgs e)
        {
            toolStripButton1.Text = "Play";
            groupBox2.Enabled = true;
            toolStripComboBox1.Enabled = true;
            giveMessage("Stopped.");
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            speech.Dispose();
        }

        private string getTextToPlay()
        {
            string txt = textBox1.Text;
            switch (toolStripComboBox1.SelectedIndex)
            {
                case 0:
                    return txt;
                case 1:
                    return txt.Substring(textBox1.SelectionStart);
                case 2:
                    return textBox1.SelectedText;
                default:
                    showError("Something was wrong with the combo-box selection next to \"Play.\"", "Flow Error");
                    return "";
            }
        }

        private void showError(string message, string caption)
        {
            MessageBox.Show(
                this,
                message,
                caption,
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
        }

        private void giveMessage(string message)
        {
            toolStripStatusLabel1.Text = message;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog(this);
            string fname = openFileDialog1.FileName;
            if (File.Exists(fname))
            {
                textBox1.Text = File.ReadAllText(fname);
            }
            else
            {
                showError("File doesn't exist.", "Bad file");
            }
            
        }
    }
}