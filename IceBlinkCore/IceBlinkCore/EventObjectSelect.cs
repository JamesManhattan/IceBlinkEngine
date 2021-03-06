﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using IceBlinkToolset;
using System.IO;

namespace IceBlinkCore
{
    public partial class EventObjectSelect : Form
    {
        public IWindowsFormsEditorService _wfes;
        public EventObjEditorReturnObject returnObject = new EventObjEditorReturnObject();
        public ParentForm prntForm;
        public Area currentArea;
        public List<string> scriptList = new List<string>();
        public List<string> convoObjList = new List<string>();
        public bool firstTimeThrough = true;

        public EventObjectSelect(EventObjEditorReturnObject retObj, IWindowsFormsEditorService w)
        {
            InitializeComponent();
            this.TopLevel = false;
            this._wfes = w;
            this.returnObject = retObj;
            this.prntForm = returnObject.prntForm;
            refreshCmbBoxes();
            if (cmbObjectTagFilename.Items.Count > 0)
                cmbObjectTagFilename.SelectedIndex = 0;
        }

        #region Methods
        public void refreshCmbBoxes()
        {
            if (returnObject.EventType == Trigger.TriggerType.Encounter)
            {
                cmbObjectTagFilename.BeginUpdate();
                cmbObjectTagFilename.DataSource = null;
                cmbObjectTagFilename.DataSource = prntForm.encountersList.encounters;
                cmbObjectTagFilename.DisplayMember = "EncounterName";
                cmbObjectTagFilename.EndUpdate();
            }
            else if (returnObject.EventType == Trigger.TriggerType.Container)
            {
                cmbObjectTagFilename.BeginUpdate();
                cmbObjectTagFilename.DataSource = null;
                cmbObjectTagFilename.DataSource = prntForm.containersList.containers;
                cmbObjectTagFilename.DisplayMember = "ContainerTag";
                cmbObjectTagFilename.EndUpdate();
            }
            else if (returnObject.EventType == Trigger.TriggerType.Shop)
            {
                cmbObjectTagFilename.BeginUpdate();
                cmbObjectTagFilename.DataSource = null;
                cmbObjectTagFilename.DataSource = prntForm.shopsList.shopsList;
                cmbObjectTagFilename.DisplayMember = "ShopTag";
                cmbObjectTagFilename.EndUpdate();
            }
            else if (returnObject.EventType == Trigger.TriggerType.Conversation)
            {
                fillConversableObjectsList();
                cmbObjectTagFilename.BeginUpdate();
                cmbObjectTagFilename.DataSource = null;
                cmbObjectTagFilename.DataSource = convoObjList;
                cmbObjectTagFilename.EndUpdate();
            }
            else if (returnObject.EventType == Trigger.TriggerType.Narration)
            {
                cmbObjectTagFilename.BeginUpdate();
                cmbObjectTagFilename.DataSource = null;
                cmbObjectTagFilename.DataSource = prntForm.mod.ModuleConvosList;
                cmbObjectTagFilename.EndUpdate();
            }
            else if (returnObject.EventType == Trigger.TriggerType.Transition)
            {
                cmbObjectTagFilename.BeginUpdate();
                cmbObjectTagFilename.DataSource = null;
                cmbObjectTagFilename.DataSource = prntForm.mod.ModuleAreasList;
                cmbObjectTagFilename.EndUpdate();
            }
            else if (returnObject.EventType == Trigger.TriggerType.Script)
            {
                fillScriptList();
                cmbObjectTagFilename.BeginUpdate();
                cmbObjectTagFilename.DataSource = null;
                cmbObjectTagFilename.DataSource = scriptList;
                cmbObjectTagFilename.EndUpdate();
            }            
            else // type is None
            {
                //cmbObjectTagFilename.BeginUpdate();
                //cmbObjectTagFilename.DataSource = null;
                //cmbObjectTagFilename.DataSource = scriptList;
                //cmbObjectTagFilename.EndUpdate();
            }
        }        
        private void fillScriptList()
        {
            scriptList.Clear();
            string jobDir = "";
            if (prntForm.mod.ModuleName != "NewModule")
            {
                jobDir = prntForm._mainDirectory + "\\modules\\" + prntForm.mod.ModuleFolderName + "\\scripts";
            }
            else
            {
                jobDir = prntForm._mainDirectory + "\\data\\NewModule\\scripts";
            }
            foreach (string f in Directory.GetFiles(jobDir, "*.cs"))
            {
                string filename = Path.GetFileName(f);
                scriptList.Add(filename);
            }
            string defaultDir = prntForm._mainDirectory + "\\data\\scripts";
            foreach (string f in Directory.GetFiles(defaultDir, "*.cs"))
            {
                string filename = Path.GetFileName(f);
                if (!scriptList.Contains(filename))
                {
                    scriptList.Add(filename);
                }
            }
        }
        private void fillConversableObjectsList()
        {
            convoObjList.Clear();
            foreach (CreatureRefs crtRef in returnObject.currentArea.AreaCreatureRefsList)
            {
                Creature crt = prntForm.creaturesList.getCreatureByResRef(crtRef.CreatureResRef);
                if ((crt.ConversationChk) && (crt.ConversationTag != ""))
                {
                    convoObjList.Add(crtRef.CreatureTag);
                }
            }
            foreach (PropRefs prpRef in returnObject.currentArea.AreaPropRefsList)
            {
                Prop prp = prntForm.propsList.getPropByResRef(prpRef.PropResRef);
                if ((prp.PropConversationChk) && (prp.PropConversationTag != ""))
                {
                    convoObjList.Add(prpRef.PropTag);
                }
            }
        }
        private void refreshPanel()
        {
            if (returnObject.EventType == Trigger.TriggerType.Script)
            {
                gbScriptParms.Enabled = true;
                gbTransitionLocation.Enabled = false;
            }
            else if (returnObject.EventType == Trigger.TriggerType.Transition)
            {
                gbTransitionLocation.Enabled = true;
                gbScriptParms.Enabled = false;
            }
            else
            {
                gbTransitionLocation.Enabled = false;
                gbScriptParms.Enabled = false;
            }
            parm1.Text = returnObject.Parm1;
            parm2.Text = returnObject.Parm2;
            parm3.Text = returnObject.Parm3;
            parm4.Text = returnObject.Parm4;
            transitionX.Value = returnObject.TransPoint.X;
            transitionY.Value = returnObject.TransPoint.Y;
            cmbObjectTagFilename.SelectedIndex = cmbObjectTagFilename.FindStringExact(returnObject.FilenameOrTag);
        }
        #endregion
        
        #region Event Handlers
        private void cmbObjectTagFilename_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if ((returnObject != null) && (cmbObjectTagFilename.SelectedItem != null))
                {
                    if (!firstTimeThrough)
                    {
                        returnObject.FilenameOrTag = cmbObjectTagFilename.SelectedItem.ToString();
                        if (returnObject.FilenameOrTag == "none")
                        {
                            resetParametersAndLocation();
                        }
                    }
                    firstTimeThrough = false;
                    if (returnObject.EventType == Trigger.TriggerType.Script)
                    {
                        //load script into rtxt for browsing
                        string jobDir = "";
                        if (File.Exists(prntForm._mainDirectory + "\\modules\\" + prntForm.mod.ModuleFolderName + "\\scripts\\" + cmbObjectTagFilename.SelectedItem.ToString()))
                        {
                            jobDir = prntForm._mainDirectory + "\\modules\\" + prntForm.mod.ModuleFolderName + "\\scripts";
                        }
                        else
                        {
                            jobDir = prntForm._mainDirectory + "\\data\\scripts";
                        }
                        try
                        {
                            rtxtScript.LoadFile(jobDir + "\\" + cmbObjectTagFilename.SelectedItem.ToString(), RichTextBoxStreamType.PlainText);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.ToString());
                        }
                        //rtxtScript.LoadFile();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error, please let us know: " + ex.ToString());
                prntForm.game.errorLog(ex.ToString());
            }
        }
        private void EventObjectSelect_FormClosed(object sender, FormClosedEventArgs e)
        {
            _wfes.CloseDropDown();
        }
        private void parm1_TextChanged(object sender, EventArgs e)
        {
            returnObject.Parm1 = parm1.Text;
        }
        private void parm2_TextChanged(object sender, EventArgs e)
        {
            returnObject.Parm2 = parm2.Text;
        }
        private void parm3_TextChanged(object sender, EventArgs e)
        {
            returnObject.Parm3 = parm3.Text;
        }
        private void parm4_TextChanged(object sender, EventArgs e)
        {
            returnObject.Parm4 = parm4.Text;
        }
        private void transitionX_ValueChanged(object sender, EventArgs e)
        {
            Point newTranPnt = returnObject.TransPoint;
            newTranPnt.X = (int)transitionX.Value;
            returnObject.TransPoint = newTranPnt;
            //returnObject.TransPoint.X = (int)transitionX.Value;
        }
        private void transitionY_ValueChanged(object sender, EventArgs e)
        {
            Point newTranPnt = returnObject.TransPoint;
            newTranPnt.Y = (int)transitionY.Value;
            returnObject.TransPoint = newTranPnt;
            //returnObject.TransPoint.Y = (int)transitionY.Value;
        }
        #endregion

        private void EventObjectSelect_Load(object sender, EventArgs e)
        {
            refreshPanel();
        }
        private void resetParametersAndLocation()
        {
            returnObject.Parm1 = "none";
            returnObject.Parm2 = "none";
            returnObject.Parm3 = "none";
            returnObject.Parm4 = "none";
            parm1.Text = "none";
            parm2.Text = "none";
            parm3.Text = "none";
            parm4.Text = "none";
            transitionX.Value = 0;
            transitionY.Value = 0;
            returnObject.TransPoint = new Point(0, 0);
        }
        private void btnReset_Click(object sender, EventArgs e)
        {
            resetParametersAndLocation();
        }
    }
}
