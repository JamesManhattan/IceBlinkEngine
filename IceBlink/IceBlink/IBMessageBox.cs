﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using IceBlinkCore;

namespace IceBlink
{
    public partial class IBMessageBox : IBForm
    {
        private Game game;

        public IBMessageBox()
        {
            InitializeComponent();
        }
        
        /// <summary>
        /// setMessage method is used to display a message on
        /// the form. The message is displayed in a Label control.
        /// </summary>
        /// <param name="messageText">Message which needs to be displayed to user.</param>
        private void setMessage(string messageText)
        {
            this.lblMessageText.Text = messageText;
        }

        /// <summary>
        /// setBoxSize method is used to adjust the 
        /// form size based on the message length.
        /// </summary>
        /// <param name="messageText">Message which needs to be displayed to user.</param>
        private void setBoxSize(string messageText)
        {
            //label1.Text = messageText.Length.ToString();
            if (messageText.Length < 60)
            {
                this.Width = 300;
                this.Height = 150;
                this.MaximumSize = new Size(200, 150);
                this.MinimumSize = new Size(200, 150);
            }
            else if (messageText.Length < 240)
            {
                this.Width = 450;
                this.Height = 105 + (messageText.Length / 30) * 20;
                if (this.Height < 150) { this.Height = 150; }
                this.MaximumSize = new Size(this.Width, this.Height);
                this.MinimumSize = new Size(this.Width, this.Height);
            }
            else
            {
                this.Width = 600;
                this.Height = 105 + (messageText.Length / 40) * 20;
                if (this.Height < 150) { this.Height = 150; }
                this.MaximumSize = new Size(this.Width, this.Height);
                this.MinimumSize = new Size(this.Width, this.Height);
            }
        }

        /// <summary>
        /// This method is used to add button(s) on the message box.
        /// </summary>
        /// <param name="MessageButton">MessageButton is type of enumMessageButton
        /// through which determines the types of button which need to be displayed.</param>
        private void addButton(Game game, enumMessageButton MessageButton)
        {
            switch (MessageButton)
            {
                case enumMessageButton.OK:
                    {
                        IceBlinkButtonMedium btnOk = new IceBlinkButtonMedium();
                        btnOk.setupAll(game);
                        btnOk.Text = "";
                        btnOk.TextIB = "OK";
                        btnOk.Font = game.module.ModuleTheme.ModuleFont;
                        btnOk.DialogResult = DialogResult.OK;
                        //btnOk.FlatStyle = FlatStyle.Popup;
                        //btnOk.FlatAppearance.BorderSize = 0;
                        btnOk.SetBounds((this.ClientSize.Width / 2) - 40, this.ClientSize.Height - 35, 80, 25);
                        this.Controls.Add(btnOk);
                    }
                    break;
                case enumMessageButton.OKCancel:
                    {
                        IceBlinkButtonMedium btnOk = new IceBlinkButtonMedium();
                        btnOk.setupAll(game);
                        btnOk.Text = "";
                        btnOk.TextIB = "OK";
                        btnOk.Font = game.module.ModuleTheme.ModuleFont;
                        btnOk.DialogResult = DialogResult.OK;
                        //btnOk.FlatStyle = FlatStyle.Popup;
                        //btnOk.FlatAppearance.BorderSize = 0;
                        btnOk.SetBounds((this.ClientSize.Width / 2) - 85, this.ClientSize.Height - 35, 80, 25);
                        this.Controls.Add(btnOk);

                        IceBlinkButtonMedium btnCancel = new IceBlinkButtonMedium();
                        btnCancel.setupAll(game);
                        btnCancel.Text = "";
                        btnCancel.TextIB = "CANCEL";
                        btnCancel.Font = game.module.ModuleTheme.ModuleFont;
                        btnCancel.DialogResult = DialogResult.Cancel;
                        //btnCancel.FlatStyle = FlatStyle.Popup;
                        //btnCancel.FlatAppearance.BorderSize = 0;
                        btnCancel.SetBounds((this.ClientSize.Width / 2) + 5, this.ClientSize.Height - 35, 80, 25);
                        this.Controls.Add(btnCancel);
                    }
                    break;
                case enumMessageButton.YesNo:
                    {
                        IceBlinkButtonMedium btnNo = new IceBlinkButtonMedium();
                        btnNo.setupAll(game);
                        btnNo.Text = "";
                        btnNo.TextIB = "NO";
                        btnNo.Font = game.module.ModuleTheme.ModuleFont;
                        btnNo.DialogResult = DialogResult.No;
                        //btnNo.FlatStyle = FlatStyle.Popup;
                        //btnNo.FlatAppearance.BorderSize = 0;
                        btnNo.SetBounds((this.ClientSize.Width / 2) + 5, this.ClientSize.Height - 35, 80, 25);
                        this.Controls.Add(btnNo);

                        IceBlinkButtonMedium btnYes = new IceBlinkButtonMedium();
                        btnYes.setupAll(game);
                        btnYes.Text = "";
                        btnYes.TextIB = "YES";
                        btnYes.Font = game.module.ModuleTheme.ModuleFont;
                        btnYes.DialogResult = DialogResult.Yes;
                        //btnYes.FlatStyle = FlatStyle.Popup;
                        //btnYes.FlatAppearance.BorderSize = 0;                        
                        btnYes.SetBounds((this.ClientSize.Width / 2) - 85, this.ClientSize.Height - 35, 80, 25);
                        this.Controls.Add(btnYes);
                    }
                    break;
            }
        }

        /// <summary>
        /// Show method (which is overloaded) is used to display the message.
        /// This is a static method so we don't need to create an
        /// object of this class to call this method.
        /// </summary>
        /// <param name="messageText">Message which needs to be displayed to user.</param>
        public static DialogResult Show(Game game, string messageText)
        {
            IBMessageBox frmMessage = new IBMessageBox();
            frmMessage.IceBlinkButtonResize.setupAll(game);
            frmMessage.IceBlinkButtonResize.Enabled = false;
            frmMessage.IceBlinkButtonResize.Visible = false;
            //frmMessage.IceBlinkButtonClose.Enabled = false;
            //frmMessage.IceBlinkButtonClose.Visible = false;
            frmMessage.IceBlinkButtonClose.setupAll(game);
            frmMessage.setupAll(game);
            frmMessage.BackColor = game.module.ModuleTheme.StandardBackColor;
            frmMessage.setMessage(messageText);
            frmMessage.setBoxSize(messageText);
            frmMessage.addButton(game, enumMessageButton.OK);
            frmMessage.StartPosition = FormStartPosition.CenterScreen;
            DialogResult dr = frmMessage.ShowDialog();
            return dr;
        }
        public static DialogResult Show(Game game, string messageText, enumMessageButton messageButton)
        {
            IBMessageBox frmMessage = new IBMessageBox();
            frmMessage.IceBlinkButtonResize.setupAll(game);
            frmMessage.IceBlinkButtonResize.Enabled = false;
            frmMessage.IceBlinkButtonResize.Visible = false;
            //frmMessage.IceBlinkButtonClose.Enabled = false;
            //frmMessage.IceBlinkButtonClose.Visible = false;
            frmMessage.IceBlinkButtonClose.setupAll(game);
            frmMessage.setupAll(game);
            frmMessage.BackColor = game.module.ModuleTheme.StandardBackColor;
            frmMessage.setMessage(messageText);
            frmMessage.setBoxSize(messageText);
            frmMessage.addButton(game, messageButton);
            frmMessage.StartPosition = FormStartPosition.CenterScreen;
            DialogResult dr = frmMessage.ShowDialog();
            return dr;
        }

        /*protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;
                return cp;
            }
        }*/
    }

    public enum enumMessageButton
    {
        OK,
        YesNo,
        OKCancel
    }
}
