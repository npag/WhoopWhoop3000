﻿using NAudio.CoreAudioApi;
using NAudio.Wave;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace Soundboard
{
    public partial class frmMain : Form
    {
        private IWavePlayer waveOut;
        private IWavePlayer waveOut2;
        private string[][] files;
        private int curFile = 0;
        private int curDir = 0;
        private MMDevice device;
        private MMDevice device2;

        public frmMain()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Program.NewKey += new Program.KeyHandler(keyEvent);
            Rectangle workingArea = Screen.GetWorkingArea(this);
            this.Location = new Point(workingArea.Right - Size.Width, workingArea.Bottom - Size.Height);
            device = new MMDeviceEnumerator().EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active).FirstOrDefault(d => d.ID == "{0.0.0.00000000}.{a14f7b68-1bb8-4de3-bfd6-38fc5fc43b2d}");
            device2 = new MMDeviceEnumerator().EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active).FirstOrDefault(d => d.ID == "{0.0.0.00000000}.{4ac56367-03a9-474e-b1e3-c6dfb1f36aad}");//{0.0.0.00000000}.{234779d1-4c87-4bc3-88c9-e03ea41dfbd0}

            update();
            showFile();
            tmrHide.Enabled = true;
        }

        private void keyEvent(string aKey)
        {
            if (aKey == "MediaPlayPause")
            {
                tmrHide.Enabled = false;
                this.Show();
                play();
                showFile();
                tmrHide.Enabled = true;
            }

            if (aKey == "MediaStop")
            {
                tmrHide.Enabled = false;
                this.Show();
                stop();
                showFile();
                tmrHide.Enabled = true;
            }

            if (aKey == "MediaNextTrack")
            {
                tmrHide.Enabled = false;
                this.Show();
                next();
                showFile();
                tmrHide.Enabled = true;
            }

            if (aKey == "MediaPreviousTrack")
            {
                tmrHide.Enabled = false;
                this.Show();
                previous();
                showFile();
                tmrHide.Enabled = true;
            }

            if (aKey == "F7")
            {
                tmrHide.Enabled = false;
                this.Show();
                previousDir();
                showFile();
                tmrHide.Enabled = true;
            }

            if (aKey == "F8")
            {
                tmrHide.Enabled = false;
                this.Show();
                nextDir();
                showFile();
                tmrHide.Enabled = true;
            }
        }

        private void update()
        {
            string[] dirs = Directory.GetDirectories(Directory.GetCurrentDirectory() + "\\sounds");
            files = new string[dirs.Count()][];
            for (int i = 0; i < dirs.Count(); i++)
            {
                files[i] = Directory.GetFiles(dirs[i], "*", SearchOption.AllDirectories);
            }
            curFile = 0;
            curDir = 0;
        }

        private void play()
        {
            try
            {
                if (waveOut != null)
                {
                    if (waveOut.PlaybackState == PlaybackState.Playing)
                    {
                        waveOut.Stop();
                        waveOut2.Stop();
                    }
                }
                waveOut = new WasapiOut(device, AudioClientShareMode.Shared, false, 0);

                AudioFileReader audioFileReader = new AudioFileReader(files[curDir][curFile]);

                waveOut.Init(audioFileReader);
                waveOut.Play();

                waveOut2 = new WasapiOut(device2, AudioClientShareMode.Shared, false, 0);

                AudioFileReader audioFileReader2 = new AudioFileReader(files[curDir][curFile]);
                audioFileReader2.Volume = 0.2F;
                waveOut2.Init(audioFileReader2);
                waveOut2.Play();
            }
            catch
            {
                update();
            }
        }

        private void next()
        {
            if (curFile < files[curDir].Count() - 1) curFile++;
            else nextDir();
        }

        private void nextDir()
        {
            if (curDir < files.Count() - 1) curDir++;
            else curDir = 0;
            curFile = 0;
        }

        private void stop()
        {
            waveOut.Stop();
            waveOut2.Stop();
        }

        private void previous()
        {
            if (curFile > 0) curFile--;
            else
            {
                previousDir();
                curFile = files[curDir].Count() - 1;
            }
        }

        private void previousDir()
        {
            if (curDir > 0) curDir--;
            else curDir = files.Count() - 1;
            curFile = 0;
        }

        private void showFile()
        {
            string path = files[curDir][curFile];
            string[] pathSplit = path.Split('\\');
            if (pathSplit[pathSplit.Count() - 2] != "sounds") lblFolder.Text = pathSplit[pathSplit.Count() - 2];
            else lblFolder.Text = "-";
            string[] fileSplit = pathSplit[pathSplit.Count() - 1].Split('.');
            lblText.Text = fileSplit[0];
            for (int counter = 0; counter < 100; counter++)
            {
                this.Opacity = 0.01F * counter;
                Thread.Sleep(1);
            }
        }

        private void tmrHide_Tick(object sender, EventArgs e)
        {
            for (int counter = 100; counter > 0; counter--)
            {
                this.Opacity = 0.01F * counter;
                Thread.Sleep(1);
            }
            this.Hide();
            tmrHide.Enabled = false;
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            notifyIcon.Visible = false;
            Environment.Exit(0);
        }

        private void updateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            update();
        }
    }
}