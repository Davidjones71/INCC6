/*
Copyright (c) 2014, Los Alamos National Security, LLC
All rights reserved.
Copyright 2014. Los Alamos National Security, LLC. This software was produced under U.S. Government contract 
DE-AC52-06NA25396 for Los Alamos National Laboratory (LANL), which is operated by Los Alamos National Security, 
LLC for the U.S. Department of Energy. The U.S. Government has rights to use, reproduce, and distribute this software.  
NEITHER THE GOVERNMENT NOR LOS ALAMOS NATIONAL SECURITY, LLC MAKES ANY WARRANTY, EXPRESS OR IMPLIED, 
OR ASSUMES ANY LIABILITY FOR THE USE OF THIS SOFTWARE.  If software is modified to produce derivative works, 
such modified software should be clearly marked, so as not to confuse it with the version available from LANL.

Additionally, redistribution and use in source and binary forms, with or without modification, are permitted provided 
that the following conditions are met:
•	Redistributions of source code must retain the above copyright notice, this list of conditions and the following 
disclaimer. 
•	Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following 
disclaimer in the documentation and/or other materials provided with the distribution. 
•	Neither the name of Los Alamos National Security, LLC, Los Alamos National Laboratory, LANL, the U.S. Government, 
nor the names of its contributors may be used to endorse or promote products derived from this software without specific 
prior written permission. 
THIS SOFTWARE IS PROVIDED BY LOS ALAMOS NATIONAL SECURITY, LLC AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED 
WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL LOS ALAMOS NATIONAL SECURITY, LLC OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, 
INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF 
SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY 
THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING 
IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/
using System;
using System.Windows.Forms;
using AnalysisDefs;
namespace NewUI
{
    using Integ = NCC.IntegrationHelpers;
    using NC = NCC.CentralizedState;
    public partial class IDDCollarAcquire : Form
    {
        NormParameters norm = new NormParameters();
        AcquireParameters ap = new AcquireParameters();
        Measurement passive = null;

        public IDDCollarAcquire(NormParameters npp)
        {
            InitializeComponent();
            ap = Integ.GetCurrentAcquireParams();
            INCCAnalysisParams.collar_combined_rec col = new INCCAnalysisParams.collar_combined_rec();
            
            Detector d= new Detector();
            Integ.GetCurrentAcquireDetectorPair(ref ap, ref d);
            npp.CopyTo(norm);
            // Main window checks if Collar is defined for material type. No check needed here.
            FillForm();
        }

        private void FillForm()
        {
            AnalysisMethods am = Integ.GetMethodSelections(ap);
            INCCAnalysisParams.collar_combined_rec col = new INCCAnalysisParams.collar_combined_rec();
            if (am != null)
            {
                // Grab the settings.
                if (am.HasMethod(AnalysisMethod.CollarAmLi))
                    col = (INCCAnalysisParams.collar_combined_rec)am.GetMethodParameters(AnalysisMethod.CollarAmLi);
                else
                    col = (INCCAnalysisParams.collar_combined_rec)am.GetMethodParameters(AnalysisMethod.CollarCf);
            }

            // Default is to request passive measurement info. Once that is entered or pulled from a measurement, we 
            // can enable the active, then the calculation. HN 5/3/2017
            ModeComboBox.SelectedIndex = col.collar.collar_mode == true ? 1 : 0;
            PassiveMeasurementRadioButton.Checked = true;
            PassiveMeasurementRadioButton.Enabled = false;
            ActiveMeasurementRadioButton.Checked = false;
            ActiveMeasurementRadioButton.Enabled = false;
            CalculateResultsRadioButton.Checked = false;
            CalculateResultsRadioButton.Enabled = false;

            //These are all filled based on 1) a live measurement or 2) a stored measurement
            ActiveSinglesTextBox.ReadOnly = true;
            ActiveSinglesTextBox.NumberFormat = NumericTextBox.Formatter.F3;
            ActiveSinglesErrorTextBox.ReadOnly = true;
            ActiveSinglesErrorTextBox.NumberFormat = NumericTextBox.Formatter.F3;
            ActiveDoublesTextBox.ReadOnly = true;
            ActiveDoublesTextBox.NumberFormat = NumericTextBox.Formatter.F3;
            ActiveDoublesErrorTextBox.ReadOnly = true;
            ActiveDoublesErrorTextBox.NumberFormat = NumericTextBox.Formatter.F3;

            PassiveSinglesTextBox.ReadOnly = true;
            PassiveSinglesTextBox.NumberFormat = NumericTextBox.Formatter.F3;
            PassiveSinglesErrorTextBox.ReadOnly = true;
            PassiveSinglesErrorTextBox.NumberFormat = NumericTextBox.Formatter.F3;
            PassiveDoublesTextBox.ReadOnly = true;
            PassiveDoublesTextBox.NumberFormat = NumericTextBox.Formatter.F3;
            PassiveDoublesErrorTextBox.ReadOnly = true;
            PassiveDoublesErrorTextBox.NumberFormat = NumericTextBox.Formatter.F3;
        }

        private void OKBtn_Click(object sender, EventArgs e)
        {

        }

        private void CancelBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void HelpBtn_Click(object sender, EventArgs e)
        {

        }

        private void FromDB_Click(object sender, EventArgs e)
        {
            DialogResult dlg = DialogResult.Cancel;
            IDDMeasurementList select = new IDDMeasurementList(AssaySelector.MeasurementOption.rates | AssaySelector.MeasurementOption.verification, false, IDDMeasurementList.EndGoal.GetSelection, Integ.GetCurrentAcquireDetector (), ap.ItemId);
            dlg = select.ShowDialog();
            if (dlg == DialogResult.OK)
            {
                //Get the selected measurement
                passive = select.GetCurrentSelection();

            }

            if (passive != null)
            {
                passive.CalcAvgAndSums();
                MultiplicityCountingRes mcr = (MultiplicityCountingRes)passive.CountingAnalysisResults[Integ.GetCurrentAcquireDetector().MultiplicityParams];

                PassiveSinglesTextBox.Value = mcr.DeadtimeCorrectedSinglesRate.v;
                PassiveSinglesErrorTextBox.Value = mcr.DeadtimeCorrectedSinglesRate.err;
                PassiveDoublesTextBox.Value = mcr.DeadtimeCorrectedDoublesRate.v;
                PassiveDoublesErrorTextBox.Value = mcr.DeadtimeCorrectedDoublesRate.err;
                
            }

        }
    }
}
