using System;
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO.Compression;
using System.Net;
using System.ComponentModel;
using System.Threading;
using System.Globalization;
using System.Resources;

namespace GTAModManager
{
    // Enum pentru limbi
    public enum Language
    {
        English,
        Romanian,
        Spanish
    }

    public class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }

    public class MainForm : Form
    {
        private TextBox gtaPathTextBox;
        private Button browseButton;
        private Button installModButton;
        private Button uninstallModButton;
        private Button launchGameButton;
        private Button aboutButton;
        private ListBox installedModsListBox;
        private Label statusLabel;
        private FolderBrowserDialog folderBrowserDialog;
        private OpenFileDialog openFileDialog;
        private ComboBox languageComboBox;
        private Panel headerPanel;

        private string gtaPath = "";
        private string modsPath = "";
        private List<string> installedMods = new List<string>();
        private bool MinimizeOnLaunch = false;
        private Language currentLanguage = Language.English;
        private string appVersion = "1.2.0";
        
        // Strings pentru mai multe limbi
        private Dictionary<string, Dictionary<Language, string>> translations = new Dictionary<string, Dictionary<Language, string>>();

        public MainForm()
        {
            InitializeTranslations();
            InitializeComponents();
            LoadSettings();
            ApplyTheme();
            
            // Set application icon if file exists
            try
            {
                string iconPath = Path.Combine(Application.StartupPath, "256x256.ico");
                if (File.Exists(iconPath))
                {
                    this.Icon = new Icon(iconPath);
                }
                else
                {
                    // Try to use PNG as icon if ICO doesn't exist
                    string pngPath = Path.Combine(Application.StartupPath, "256x256.png");
                    if (File.Exists(pngPath))
                    {
                        Bitmap bitmap = new Bitmap(pngPath);
                        IntPtr hIcon = bitmap.GetHicon();
                        this.Icon = Icon.FromHandle(hIcon);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Could not load icon: " + ex.Message);
            }
        }
        
        private void InitializeTranslations()
        {
            // Adăugăm traduceri pentru diverse texte din aplicație
            translations["GTA 3 Mod Manager"] = new Dictionary<Language, string>
            {
                { Language.English, "GTA 3 Mod Manager" },
                { Language.Romanian, "Manager Moduri GTA 3" },
                { Language.Spanish, "Administrador de Mods GTA 3" }
            };
            
            translations["GTA 3 Installation Path:"] = new Dictionary<Language, string>
            {
                { Language.English, "GTA 3 Installation Path:" },
                { Language.Romanian, "Calea de instalare GTA 3:" },
                { Language.Spanish, "Ruta de instalación de GTA 3:" }
            };
            
            translations["Browse..."] = new Dictionary<Language, string>
            {
                { Language.English, "Browse..." },
                { Language.Romanian, "Răsfoire..." },
                { Language.Spanish, "Explorar..." }
            };
            
            translations["Installed Mods:"] = new Dictionary<Language, string>
            {
                { Language.English, "Installed Mods:" },
                { Language.Romanian, "Moduri instalate:" },
                { Language.Spanish, "Mods instalados:" }
            };
            
            translations["Install New Mod"] = new Dictionary<Language, string>
            {
                { Language.English, "Install New Mod" },
                { Language.Romanian, "Instalează Mod Nou" },
                { Language.Spanish, "Instalar Nuevo Mod" }
            };
            
            translations["Uninstall Selected Mod"] = new Dictionary<Language, string>
            {
                { Language.English, "Uninstall Selected Mod" },
                { Language.Romanian, "Dezinstalează Modul Selectat" },
                { Language.Spanish, "Desinstalar Mod Seleccionado" }
            };
            
            translations["Launch GTA 3"] = new Dictionary<Language, string>
            {
                { Language.English, "Launch GTA 3" },
                { Language.Romanian, "Pornește GTA 3" },
                { Language.Spanish, "Iniciar GTA 3" }
            };
            
            translations["Ready"] = new Dictionary<Language, string>
            {
                { Language.English, "Ready" },
                { Language.Romanian, "Gata" },
                { Language.Spanish, "Listo" }
            };
            
            translations["Language"] = new Dictionary<Language, string>
            {
                { Language.English, "Language" },
                { Language.Romanian, "Limba" },
                { Language.Spanish, "Idioma" }
            };
            
            // Adaugă mai multe traduceri după nevoie
        }

        private string GetTranslation(string key)
        {
            if (translations.ContainsKey(key) && translations[key].ContainsKey(currentLanguage))
            {
                return translations[key][currentLanguage];
            }
            return key; // Dacă nu există traducere, returnăm textul original
        }

        // Aplicăm tema pentru un aspect modern
        private void ApplyTheme()
        {
            // Premium look with professional color scheme 
            Color primaryColor = Color.FromArgb(25, 118, 210);     // Rich blue
            Color secondaryColor = Color.FromArgb(21, 101, 192);   // Darker blue
            Color accentColor = Color.FromArgb(0, 200, 83);        // Vibrant green
            Color deleteColor = Color.FromArgb(229, 57, 53);       // Refined red
            Color textColor = Color.White;
            Color backColor = Color.FromArgb(250, 250, 250);       // Almost white
            Color panelColor = Color.FromArgb(255, 255, 255);      // White
            Color borderColor = Color.FromArgb(224, 224, 224);     // Light gray
            
            // Apply professional styling to form
            this.BackColor = backColor;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            
            // Header with gradient effect but no shadow
            headerPanel.BackColor = primaryColor;
            headerPanel.Paint += (s, e) => {
                // Create gradient background
                Rectangle rect = new Rectangle(0, 0, headerPanel.Width, headerPanel.Height);
                using (LinearGradientBrush brush = new LinearGradientBrush(
                    rect, primaryColor, secondaryColor, LinearGradientMode.Horizontal))
                {
                    e.Graphics.FillRectangle(brush, rect);
                }
            };
            
            // Actualizăm doar stilul butoanelor, fără a modifica poziționarea
            foreach (Control control in this.Controls)
            {
                if (control is Button button)
                {
                    StyleButtonPremium(button, accentColor, textColor);
                    
                    // Special button colors
                    if (button == uninstallModButton)
                        StyleButtonPremium(button, deleteColor, textColor);
                    else if (button == launchGameButton)
                        StyleButtonPremium(button, primaryColor, textColor);
                }
            }
            
            // Stilizăm controalele din panoul de conținut
            foreach (Control parentControl in this.Controls)
            {
                if (parentControl is Panel contentPanel && parentControl != headerPanel)
                {
                    foreach (Control control in contentPanel.Controls)
                    {
                        if (control is Button button)
                        {
                            StyleButtonPremium(button, accentColor, textColor);
                            
                            // Special button colors
                            if (button == uninstallModButton)
                                StyleButtonPremium(button, deleteColor, textColor);
                            else if (button == launchGameButton)
                                StyleButtonPremium(button, primaryColor, textColor);
                            else if (button == browseButton)
                                StyleButtonPremium(button, primaryColor, textColor);
                        }
                        else if (control is Label label)
                        {
                            label.ForeColor = Color.FromArgb(60, 60, 60);
                        }
                    }
                }
            }
            
            // Style header elements
            foreach (Control control in headerPanel.Controls)
            {
                if (control is Button button)
                {
                    button.FlatStyle = FlatStyle.Flat;
                    button.FlatAppearance.BorderSize = 0;
                    button.BackColor = Color.FromArgb(21, 101, 192);
                    button.ForeColor = Color.White;
                    button.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
                    button.Cursor = Cursors.Hand;
                }
                else if (control is Label)
                {
                    control.ForeColor = Color.White;
                    
                    // Eliminate shadow on title
                    if (control.Font.Size > 14)
                    {
                        // No shadow effect
                    }
                }
            }
        }
        
        private void StyleButtonPremium(Button button, Color backColor, Color foreColor)
        {
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.BackColor = backColor;
            button.ForeColor = foreColor;
            button.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);
            button.Cursor = Cursors.Hand;
            
            // Add hover effect
            button.MouseEnter += (s, e) => {
                button.BackColor = ControlPaint.Light(backColor, 0.1f);
            };
            
            button.MouseLeave += (s, e) => {
                button.BackColor = backColor;
            };
            
            // Add pressed effect
            button.MouseDown += (s, e) => {
                button.BackColor = ControlPaint.Dark(backColor, 0.1f);
            };
            
            button.MouseUp += (s, e) => {
                button.BackColor = backColor;
            };
            
            // Add rounded corners without shadow effect
            button.Paint += (s, e) => {
                System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
                int radius = 6;
                
                // Create rounded rectangle
                path.AddArc(0, 0, radius, radius, 180, 90);
                path.AddArc(button.Width - radius, 0, radius, radius, 270, 90);
                path.AddArc(button.Width - radius, button.Height - radius, radius, radius, 0, 90);
                path.AddArc(0, button.Height - radius, radius, radius, 90, 90);
                button.Region = new Region(path);
            };
        }

        private void InitializeComponents()
        {
            // Window setup
            this.Text = GetTranslation("GTA 3 Mod Manager");
            this.Size = new Size(700, 550);  // Înălțime mărită
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            this.AutoScaleMode = AutoScaleMode.Font;
            
            // Modern header panel
            headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 70,
                BackColor = Color.FromArgb(25, 118, 210)
            };
            this.Controls.Add(headerPanel);
            
            // Title with professional font
            Label titleLabel = new Label
            {
                Text = GetTranslation("GTA 3 Mod Manager"),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 22F, FontStyle.Bold),
                Location = new Point(25, 15),
                Size = new Size(400, 40),
                AutoSize = true
            };
            headerPanel.Controls.Add(titleLabel);
            
            // Modern language selector
            languageComboBox = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = new Point(500, 22),
                Size = new Size(120, 25),
                FlatStyle = FlatStyle.Flat
            };
            languageComboBox.Items.Add("English");
            languageComboBox.Items.Add("Română");
            languageComboBox.Items.Add("Español");
            languageComboBox.SelectedIndex = 0;
            languageComboBox.SelectedIndexChanged += LanguageComboBox_SelectedIndexChanged;
            headerPanel.Controls.Add(languageComboBox);
            
            Label langLabel = new Label
            {
                Text = GetTranslation("Language") + ":",
                ForeColor = Color.White,
                Location = new Point(420, 25),
                Size = new Size(70, 20)
            };
            headerPanel.Controls.Add(langLabel);

            // Help button with premium look
            aboutButton = new Button
            {
                Text = "?",
                Location = new Point(640, 20),
                Size = new Size(32, 32),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(21, 101, 192),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold)
            };
            aboutButton.Click += AboutButton_Click;
            headerPanel.Controls.Add(aboutButton);
            
            // Add circular shape to About button
            aboutButton.Paint += (s, e) => {
                System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
                path.AddEllipse(0, 0, aboutButton.Width, aboutButton.Height);
                aboutButton.Region = new Region(path);
            };

            // Container panel pentru conținutul principal - ajută la organizare
            Panel contentPanel = new Panel
            {
                Location = new Point(0, headerPanel.Height),
                Size = new Size(this.ClientSize.Width, this.ClientSize.Height - headerPanel.Height),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                BackColor = this.BackColor
            };
            this.Controls.Add(contentPanel);

            // Resetăm toate coordonatele relative la panoul de conținut
            int padding = 20;
            int topMargin = 20;

            // Section: GTA 3 Path with professional typography
            Label pathLabel = new Label
            {
                Text = GetTranslation("GTA 3 Installation Path:"),
                Location = new Point(padding, topMargin),
                Size = new Size(250, 22),
                Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold)
            };
            contentPanel.Controls.Add(pathLabel);

            gtaPathTextBox = new TextBox
            {
                Location = new Point(padding, pathLabel.Bottom + 10),
                Size = new Size(contentPanel.Width - padding * 2 - 110, 22),
                ReadOnly = true,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 10F)
            };
            contentPanel.Controls.Add(gtaPathTextBox);

            browseButton = new Button
            {
                Text = GetTranslation("Browse..."),
                Location = new Point(gtaPathTextBox.Right + 10, gtaPathTextBox.Top - 5),
                Size = new Size(100, 32),
                FlatStyle = FlatStyle.Flat
            };
            browseButton.Click += BrowseButton_Click;
            contentPanel.Controls.Add(browseButton);

            // Section: Installed Mods with improved design
            Label modsListLabel = new Label
            {
                Text = GetTranslation("Installed Mods:"),
                Location = new Point(padding, gtaPathTextBox.Bottom + 25),
                Size = new Size(200, 22),
                Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold)
            };
            contentPanel.Controls.Add(modsListLabel);

            installedModsListBox = new ListBox
            {
                Location = new Point(padding, modsListLabel.Bottom + 10),
                Size = new Size(contentPanel.Width - padding * 2, 220),
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 10F),
                IntegralHeight = false,
                ItemHeight = 24  // Înălțime fixă pentru fiecare element
            };
            
            // Simplificăm afișarea elementelor din ListBox pentru a evita probleme
            installedModsListBox.DrawMode = DrawMode.OwnerDrawFixed;
            installedModsListBox.DrawItem += (s, e) => {
                if (e.Index < 0) return;
                
                e.DrawBackground();
                
                // Fundal curat, fără alternare de culori
                Rectangle bounds = e.Bounds;
                using (Brush backBrush = new SolidBrush(Color.White))
                {
                    e.Graphics.FillRectangle(backBrush, bounds);
                }
                
                // Evidențiere selecție fără umbră
                if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                {
                    using (Brush selectionBrush = new SolidBrush(Color.FromArgb(235, 245, 255)))
                    {
                        e.Graphics.FillRectangle(selectionBrush, bounds);
                    }
                    
                    // Bordură selecție simplă
                    using (Pen borderPen = new Pen(Color.FromArgb(25, 118, 210), 1))
                    {
                        e.Graphics.DrawRectangle(borderPen, bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
                    }
                }
                
                // Desenăm textul fără umbre
                if (e.Index < installedModsListBox.Items.Count)
                {
                    string text = installedModsListBox.Items[e.Index].ToString();
                    using (Brush textBrush = new SolidBrush(Color.FromArgb(0, 0, 0))) // Black text
                    {
                        // Poziționare verticală simplă
                        int textY = bounds.Y + (int)((bounds.Height - e.Graphics.MeasureString(text, installedModsListBox.Font).Height) / 2);
                        e.Graphics.DrawString(text, installedModsListBox.Font, textBrush, 
                            new Point(bounds.X + 10, textY));
                    }
                }
            };
            
            contentPanel.Controls.Add(installedModsListBox);

            // Action buttons with improved layout
            int buttonY = installedModsListBox.Bottom + 20;
            int buttonWidth = (contentPanel.Width - padding * 2 - 20) / 3;
            
            installModButton = new Button
            {
                Text = GetTranslation("Install New Mod"),
                Location = new Point(padding, buttonY),
                Size = new Size(buttonWidth, 40),
                FlatStyle = FlatStyle.Flat
            };
            installModButton.Click += InstallModButton_Click;
            contentPanel.Controls.Add(installModButton);

            uninstallModButton = new Button
            {
                Text = GetTranslation("Uninstall Selected Mod"),
                Location = new Point(installModButton.Right + 10, buttonY),
                Size = new Size(buttonWidth, 40),
                Enabled = false,
                FlatStyle = FlatStyle.Flat
            };
            uninstallModButton.Click += UninstallModButton_Click;
            contentPanel.Controls.Add(uninstallModButton);

            launchGameButton = new Button
            {
                Text = GetTranslation("Launch GTA 3"),
                Location = new Point(uninstallModButton.Right + 10, buttonY),
                Size = new Size(buttonWidth, 40),
                Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            launchGameButton.Click += LaunchGameButton_Click;
            contentPanel.Controls.Add(launchGameButton);

            statusLabel = new Label
            {
                Text = GetTranslation("Ready"),
                Location = new Point(padding, buttonY + 50),
                Size = new Size(contentPanel.Width - padding * 2, 20),
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                ForeColor = Color.FromArgb(100, 100, 100)
            };
            contentPanel.Controls.Add(statusLabel);

            folderBrowserDialog = new FolderBrowserDialog
            {
                Description = "Select GTA 3 Installation Folder"
            };

            openFileDialog = new OpenFileDialog
            {
                Filter = "Mod Files (*.zip)|*.zip|All files (*.*)|*.*",
                Title = "Select a GTA 3 Mod to Install"
            };

            installedModsListBox.SelectedIndexChanged += (s, e) =>
            {
                uninstallModButton.Enabled = installedModsListBox.SelectedIndex != -1;
            };
            
            // Corectare pentru stilizarea controalelor wrapper
            this.Load += (s, e) => {
                ApplyTheme();
            };
        }
        
        private void LanguageComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Schimbăm limba aplicației
            switch (languageComboBox.SelectedIndex)
            {
                case 0:
                    currentLanguage = Language.English;
                    break;
                case 1:
                    currentLanguage = Language.Romanian;
                    break;
                case 2:
                    currentLanguage = Language.Spanish;
                    break;
                default:
                    currentLanguage = Language.English;
                    break;
            }
            
            // Actualizăm textele din interfață
            UpdateUITexts();
        }
        
        private void UpdateUITexts()
        {
            // Actualizăm textele pentru limba selectată
            this.Text = GetTranslation("GTA 3 Mod Manager");
            
            // Actualizăm textele componentelor din interfață
            foreach (Control control in this.Controls)
            {
                if (control is Button button)
                {
                    if (button == installModButton)
                        button.Text = GetTranslation("Install New Mod");
                    else if (button == uninstallModButton)
                        button.Text = GetTranslation("Uninstall Selected Mod");
                    else if (button == launchGameButton)
                        button.Text = GetTranslation("Launch GTA 3");
                    else if (button == browseButton)
                        button.Text = GetTranslation("Browse...");
                }
                else if (control is Label label)
                {
                    if (label.Text == "GTA 3 Installation Path:" || label.Text == "Calea de instalare GTA 3:" || label.Text == "Ruta de instalación de GTA 3:")
                        label.Text = GetTranslation("GTA 3 Installation Path:");
                    else if (label.Text == "Installed Mods:" || label.Text == "Moduri instalate:" || label.Text == "Mods instalados:")
                        label.Text = GetTranslation("Installed Mods:");
                    else if (label.Text == "Ready" || label.Text == "Gata" || label.Text == "Listo")
                        label.Text = GetTranslation("Ready");
                }
            }
            
            // Actualizăm textele pentru headerPanel
            foreach (Control control in headerPanel.Controls)
            {
                if (control is Label label && control != languageComboBox)
                {
                    if (label.Font.Size > 12) // Acesta este titlul
                        label.Text = $"{GetTranslation("GTA 3 Mod Manager")} v{appVersion}";
                    else if (label.Text == "Language:" || label.Text == "Limba:" || label.Text == "Idioma:")
                        label.Text = GetTranslation("Language") + ":";
                }
            }
        }

        private void LoadSettings()
        {
            // Try to find GTA 3 installation path
            string[] possiblePaths = new string[]
            {
                @"C:\Program Files\Rockstar Games\Grand Theft Auto III",
                @"C:\Program Files (x86)\Rockstar Games\Grand Theft Auto III",
                @"C:\Program Files\Steam\steamapps\common\Grand Theft Auto 3",
                @"D:\Steam\steamapps\common\Grand Theft Auto 3",
                @"E:\Steam\steamapps\common\Grand Theft Auto 3"
            };

            foreach (string path in possiblePaths)
            {
                if (Directory.Exists(path) && File.Exists(Path.Combine(path, "gta3.exe")))
                {
                    gtaPath = path;
                    gtaPathTextBox.Text = gtaPath;
                    break;
                }
            }

            // Create mods directory if not exists
            if (!string.IsNullOrEmpty(gtaPath))
            {
                modsPath = Path.Combine(gtaPath, "mods");
                if (!Directory.Exists(modsPath))
                {
                    Directory.CreateDirectory(modsPath);
                }
                RefreshModsList();
            }
        }

        private void BrowseButton_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                string selectedPath = folderBrowserDialog.SelectedPath;
                if (File.Exists(Path.Combine(selectedPath, "gta3.exe")))
                {
                    gtaPath = selectedPath;
                    gtaPathTextBox.Text = gtaPath;
                    
                    modsPath = Path.Combine(gtaPath, "mods");
                    if (!Directory.Exists(modsPath))
                    {
                        Directory.CreateDirectory(modsPath);
                    }
                    RefreshModsList();
                    statusLabel.Text = "GTA 3 installation found!";
                }
                else
                {
                    MessageBox.Show("The selected folder doesn't contain GTA 3 (gta3.exe not found).", 
                        "Invalid Folder", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void AboutButton_Click(object sender, EventArgs e)
        {
            string aboutMessage = $"GTA 3 Mod Manager\n\nA tool to manage mods for Grand Theft Auto III.";
            MessageBox.Show(aboutMessage, "About GTA 3 Mod Manager", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void InstallModButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(gtaPath))
            {
                MessageBox.Show("Please select your GTA 3 installation folder first.", 
                    "GTA 3 Path Not Set", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string modFile = openFileDialog.FileName;
                string modName = Path.GetFileNameWithoutExtension(modFile);
                string modFolder = Path.Combine(modsPath, modName);

                try
                {
                    // Verifică dacă modul există deja
                    if (installedMods.Contains(modName))
                    {
                        DialogResult result = MessageBox.Show($"Mod \"{modName}\" is already installed. Do you want to reinstall it?", 
                            "Mod Already Installed", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        
                        if (result == DialogResult.No)
                            return;
                            
                        // Șterge modulul existent pentru reinstalare
                        if (Directory.Exists(modFolder))
                        {
                            Directory.Delete(modFolder, true);
                        }
                        
                        installedMods.Remove(modName);
                        installedModsListBox.Items.Remove(modName);
                    }

                    // Create backup if it doesn't exist
                    string backupFolder = Path.Combine(gtaPath, "backup");
                    if (!Directory.Exists(backupFolder))
                    {
                        statusLabel.Text = "Creating backup...";
                        Directory.CreateDirectory(backupFolder);
                        
                        // Backup important game files
                        foreach (string file in Directory.GetFiles(gtaPath, "*.img"))
                        {
                            File.Copy(file, Path.Combine(backupFolder, Path.GetFileName(file)), true);
                        }
                        foreach (string file in Directory.GetFiles(gtaPath, "*.dat"))
                        {
                            File.Copy(file, Path.Combine(backupFolder, Path.GetFileName(file)), true);
                        }
                        foreach (string file in Directory.GetFiles(gtaPath, "*.cfg"))
                        {
                            File.Copy(file, Path.Combine(backupFolder, Path.GetFileName(file)), true);
                        }
                        foreach (string file in Directory.GetFiles(gtaPath, "*.ini"))
                        {
                            File.Copy(file, Path.Combine(backupFolder, Path.GetFileName(file)), true);
                        }
                    }

                    // Create mod folder
                    if (!Directory.Exists(modFolder))
                    {
                        Directory.CreateDirectory(modFolder);
                    }
                    
                    // Extract mod to its folder
                    statusLabel.Text = $"Installing mod: {modName}...";
                    ZipFile.ExtractToDirectory(modFile, modFolder);
                    
                    // Copy mod files to game directory
                    CopyDirectory(modFolder, gtaPath);
                    
                    installedMods.Add(modName);
                    installedModsListBox.Items.Add(modName);
                    
                    statusLabel.Text = $"Mod '{modName}' installed successfully!";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error installing mod: {ex.Message}", 
                        "Installation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    statusLabel.Text = "Mod installation failed.";
                }
            }
        }

        private void UninstallModButton_Click(object sender, EventArgs e)
        {
            if (installedModsListBox.SelectedIndex == -1)
                return;

            string modName = installedModsListBox.SelectedItem.ToString();
            string modFolder = Path.Combine(modsPath, modName);

            DialogResult result = MessageBox.Show($"Are you sure you want to uninstall mod \"{modName}\"?", 
                "Confirm Uninstall", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                
            if (result == DialogResult.No)
                return;

            try
            {
                statusLabel.Text = $"Uninstalling mod: {modName}...";
                this.Cursor = Cursors.WaitCursor;
                
                // Pasul 1: Restaurăm mai întâi backup-ul original al jocului
                string backupFolder = Path.Combine(gtaPath, "backup");
                if (Directory.Exists(backupFolder))
                {
                    foreach (string file in Directory.GetFiles(backupFolder))
                    {
                        File.Copy(file, Path.Combine(gtaPath, Path.GetFileName(file)), true);
                    }
                }
                
                // Pasul 2: Ștergem toate fișierele modului din directorul jocului
                if (Directory.Exists(modFolder))
                {
                    // Identificăm toate fișierele din folderul modului
                    List<string> modFiles = GetAllFiles(modFolder);
                    
                    // Ștergem fiecare fișier corespunzător din directorul jocului
                    foreach (string modFile in modFiles)
                    {
                        string relativePath = modFile.Substring(modFolder.Length + 1);
                        string targetFile = Path.Combine(gtaPath, relativePath);
                        
                        if (File.Exists(targetFile))
                        {
                            try
                            {
                                File.Delete(targetFile);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Nu s-a putut șterge fișierul {targetFile}: {ex.Message}");
                            }
                        }
                    }
                    
                    // Ștergem și directoarele goale
                    CleanEmptyDirectories(gtaPath);
                    
                    // Pasul 3: Ștergem folderul modului din directorul de moduri
                    Directory.Delete(modFolder, true);
                }
                
                // Pasul 4: Reinstalăm celelalte moduri în ordine
                foreach (string remainingMod in installedModsListBox.Items)
                {
                    if (remainingMod != modName)
                    {
                        string remainingModFolder = Path.Combine(modsPath, remainingMod);
                        if (Directory.Exists(remainingModFolder))
                        {
                            CopyDirectory(remainingModFolder, gtaPath);
                        }
                    }
                }
                
                // Actualizăm lista
                installedMods.Remove(modName);
                installedModsListBox.Items.Remove(modName);
                
                statusLabel.Text = $"Mod '{modName}' uninstalled successfully!";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error uninstalling mod: {ex.Message}", 
                    "Uninstallation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                statusLabel.Text = "Mod uninstallation failed.";
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void RefreshModsList()
        {
            if (string.IsNullOrEmpty(modsPath) || !Directory.Exists(modsPath))
                return;

            // Curățăm lista înainte de reîncărcare
            installedModsListBox.BeginUpdate();
            installedModsListBox.Items.Clear();
            installedMods.Clear();

            foreach (string folder in Directory.GetDirectories(modsPath))
            {
                string modName = Path.GetFileName(folder);
                installedMods.Add(modName);
                installedModsListBox.Items.Add(modName);
            }
            
            installedModsListBox.EndUpdate();
            
            // Actualizăm status
            statusLabel.Text = $"{installedMods.Count} {(installedMods.Count == 1 ? "mod" : "moduri")} instalate";
        }

        // Metodă pentru a obține toate fișierele dintr-un director și subdirectoare
        private List<string> GetAllFiles(string directory)
        {
            List<string> files = new List<string>();
            
            // Adăugăm toate fișierele din acest director
            files.AddRange(Directory.GetFiles(directory));
            
            // Adăugăm recursiv fișierele din subdirectoare
            foreach (string subDir in Directory.GetDirectories(directory))
            {
                files.AddRange(GetAllFiles(subDir));
            }
            
            return files;
        }

        // Metodă pentru a curăța directoarele goale
        private void CleanEmptyDirectories(string directory)
        {
            foreach (string subDir in Directory.GetDirectories(directory))
            {
                CleanEmptyDirectories(subDir);
                
                // Verificăm dacă directorul este gol după curățarea subdirectoarelor
                if (Directory.GetFiles(subDir).Length == 0 && Directory.GetDirectories(subDir).Length == 0)
                {
                    try
                    {
                        Directory.Delete(subDir, false);
                    }
                    catch
                    {
                        // Ignorăm erorile la ștergerea directoarelor
                    }
                }
            }
        }

        private void CopyDirectory(string sourceDir, string targetDir)
        {
            foreach (string file in Directory.GetFiles(sourceDir))
            {
                string fileName = Path.GetFileName(file);
                string destFile = Path.Combine(targetDir, fileName);
                File.Copy(file, destFile, true);
            }

            foreach (string directory in Directory.GetDirectories(sourceDir))
            {
                string dirName = Path.GetFileName(directory);
                string destDir = Path.Combine(targetDir, dirName);
                
                if (!Directory.Exists(destDir))
                {
                    Directory.CreateDirectory(destDir);
                }
                
                CopyDirectory(directory, destDir);
            }
        }

        private void LaunchGameButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(gtaPath) && File.Exists(Path.Combine(gtaPath, "gta3.exe")))
            {
                try
                {
                    // Verificăm dacă jocul este pe Steam
                    bool isSteamVersion = gtaPath.Contains("Steam") || gtaPath.Contains("steamapps");
                    
                    if (isSteamVersion)
                    {
                        // Lansăm jocul prin protocolul Steam
                        // Cod Steam App ID pentru GTA 3: 12100
                        Process.Start("steam://run/12100");
                        statusLabel.Text = "GTA 3 lansat prin Steam! Verificați Steam-ul.";
                    }
                    else
                    {
                        // Lansare directă pentru versiunea non-Steam
                        ProcessStartInfo startInfo = new ProcessStartInfo
                        {
                            FileName = Path.Combine(gtaPath, "gta3.exe"),
                            WorkingDirectory = gtaPath,
                            Arguments = "-nomipmap -refresh:60" // Argumente pentru stabilitate mai bună
                        };
                        Process.Start(startInfo);
                        statusLabel.Text = "GTA 3 lansat cu succes!";
                    }
                    
                    if (MinimizeOnLaunch)
                    {
                        this.WindowState = FormWindowState.Minimized;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Eroare la lansarea GTA 3: {ex.Message}", 
                        "Eroare lansare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Executabilul GTA 3 nu a fost găsit.", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Metodă pentru repararea jocului
        private void RepairGame()
        {
            try
            {
                // Restabilire din backup
                string backupFolder = Path.Combine(gtaPath, "backup");
                if (Directory.Exists(backupFolder))
                {
                    foreach (string file in Directory.GetFiles(backupFolder))
                    {
                        File.Copy(file, Path.Combine(gtaPath, Path.GetFileName(file)), true);
                    }
                    MessageBox.Show("Jocul a fost reparat din backup-ul existent.", 
                        "Reparare finalizată", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Nu există un backup pentru repararea jocului.", 
                        "Eroare reparare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eroare la repararea jocului: {ex.Message}", 
                    "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
} 