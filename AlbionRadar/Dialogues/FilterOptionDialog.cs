using System;
using System.Linq;
using System.Windows.Forms;
using AlbionDataHandlers.Enums;

namespace AlbionRadar
{
    /// <summary>
    /// Dialog window for selecting tier filter options.
    /// </summary>
    public partial class FilterOptionDialog : Form
    {
        public TierLevels SelectedTierFilter { get; private set; } = TierLevels.Tier5;

        public FilterOptionDialog()
        {
            InitializeComponent();
            LoadTierOptions();
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.lblTitle = new Label();
            this.lblDescription = new Label();
            this.cmbTierFilter = new ComboBox();
            this.lblTierFilter = new Label();
            this.btnOK = new Button();
            this.btnCancel = new Button();
            this.SuspendLayout();

            // Form properties
            this.Text = "AlbionRadar - Filter Options";
            this.Size = new System.Drawing.Size(400, 200);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ShowIcon = false;

            // Title Label
            this.lblTitle.Text = "Filter Configuration";
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(20, 20);
            this.lblTitle.Size = new System.Drawing.Size(360, 23);
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // Description Label
            this.lblDescription.Text = "Select the minimum tier level for mob filtering:";
            this.lblDescription.Location = new System.Drawing.Point(20, 55);
            this.lblDescription.Size = new System.Drawing.Size(360, 20);

            // Tier Filter Label
            this.lblTierFilter.Text = "Minimum Tier:";
            this.lblTierFilter.Location = new System.Drawing.Point(20, 85);
            this.lblTierFilter.Size = new System.Drawing.Size(100, 23);
            this.lblTierFilter.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            // Tier Filter ComboBox
            this.cmbTierFilter.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbTierFilter.Location = new System.Drawing.Point(130, 85);
            this.cmbTierFilter.Size = new System.Drawing.Size(200, 23);

            // OK Button
            this.btnOK.Text = "OK";
            this.btnOK.DialogResult = DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(220, 125);
            this.btnOK.Size = new System.Drawing.Size(75, 25);
            this.btnOK.Click += BtnOK_Click;

            // Cancel Button
            this.btnCancel.Text = "Cancel";
            this.btnCancel.DialogResult = DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(305, 125);
            this.btnCancel.Size = new System.Drawing.Size(75, 25);

            // Add controls to form
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.lblDescription);
            this.Controls.Add(this.lblTierFilter);
            this.Controls.Add(this.cmbTierFilter);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);

            this.ResumeLayout(false);
        }

        private void LoadTierOptions()
        {
            // Get all tier levels from the enum
            var tierLevels = Enum.GetValues(typeof(TierLevels))
                .Cast<TierLevels>()
                .OrderBy(t => (int)t)
                .ToArray();

            // Create display items for the combo box
            var tierItems = tierLevels.Select(tier => new TierItem
            {
                Tier = tier,
                DisplayName = GetTierDisplayName(tier)
            }).ToArray();

            cmbTierFilter.DataSource = tierItems;
            cmbTierFilter.DisplayMember = "DisplayName";
            cmbTierFilter.ValueMember = "Tier";

            // Set default selection to Tier5
            var defaultItem = tierItems.FirstOrDefault(item => item.Tier == TierLevels.Tier5);
            if (defaultItem != null)
            {
                cmbTierFilter.SelectedItem = defaultItem;
            }
        }

        private string GetTierDisplayName(TierLevels tier)
        {
            return tier switch
            {
                TierLevels.Tier1 => "Tier 1 (Beginner)",
                TierLevels.Tier2 => "Tier 2 (Novice)",
                TierLevels.Tier3 => "Tier 3 (Journeyman)",
                TierLevels.Tier4 => "Tier 4 (Adept)",
                TierLevels.Tier5 => "Tier 5 (Expert)",
                TierLevels.Tier6 => "Tier 6 (Master)",
                TierLevels.Tier7 => "Tier 7 (Grandmaster)",
                TierLevels.Tier8 => "Tier 8 (Elder)",
                _ => tier.ToString()
            };
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            if (cmbTierFilter.SelectedItem is TierItem selectedItem)
            {
                SelectedTierFilter = selectedItem.Tier;
            }
        }

        // Helper class for ComboBox items
        private class TierItem
        {
            public TierLevels Tier { get; set; }
            public string DisplayName { get; set; } = string.Empty;
        }

        // Designer components
        private System.ComponentModel.IContainer components = null;
        private Label lblTitle;
        private Label lblDescription;
        private Label lblTierFilter;
        private ComboBox cmbTierFilter;
        private Button btnOK;
        private Button btnCancel;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}