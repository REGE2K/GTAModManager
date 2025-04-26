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
    
    // 🎮 Enum for GTA game selection
    public enum GTAGame
    {
        GTA3,
        ViceCity,
        SanAndreas,
        GTA4,
        GTA5,
        GTALibertyCityStories,
        GTAViceCityStories
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
        private ComboBox gameSelectionComboBox; // 🎮 New ComboBox for game selection
        private Panel headerPanel;

        private string gtaPath = "";
        private string modsPath = "";
        private List<string> installedMods = new List<string>();
        private bool MinimizeOnLaunch = false;
        private Language currentLanguage = Language.English;
        private GTAGame currentGame = GTAGame.GTA3; // 🎮 Default game selection
        private string appVersion = "1.3.0";
        
        // Game executable names
        private Dictionary<GTAGame, string> gameExecutables = new Dictionary<GTAGame, string>
        {
            { GTAGame.GTA3, "gta3.exe" },
            { GTAGame.ViceCity, "gta-vc.exe" },
            { GTAGame.SanAndreas, "gta_sa.exe" },
            { GTAGame.GTA4, "GTAIV.exe" },
            { GTAGame.GTA5, "GTA5.exe" },
            { GTAGame.GTALibertyCityStories, "GTALCS.exe" },
            { GTAGame.GTAViceCityStories, "GTAVCS.exe" }
        };
        
        // Game icons for display in UI
        private Dictionary<GTAGame, string> gameIcons = new Dictionary<GTAGame, string>
        {
            { GTAGame.GTA3, "🔷" },
            { GTAGame.ViceCity, "🌴" },
            { GTAGame.SanAndreas, "🏙️" },
            { GTAGame.GTA4, "🏙️" },
            { GTAGame.GTA5, "🌇" },
            { GTAGame.GTALibertyCityStories, "🏢" },
            { GTAGame.GTAViceCityStories, "🌆" }
        };
        
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
            translations["GTA Mod Manager"] = new Dictionary<Language, string>
            {
                { Language.English, "GTA Mod Manager" },
                { Language.Romanian, "Manager Moduri GTA" },
                { Language.Spanish, "Administrador de Mods GTA" }
            };
            
            translations["GTA Installation Path:"] = new Dictionary<Language, string>
            {
                { Language.English, "GTA Installation Path:" },
                { Language.Romanian, "Calea de instalare GTA:" },
                { Language.Spanish, "Ruta de instalación de GTA:" }
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
            
            translations["Launch GTA"] = new Dictionary<Language, string>
            {
                { Language.English, "Launch GTA" },
                { Language.Romanian, "Pornește GTA" },
                { Language.Spanish, "Iniciar GTA" }
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
            
            translations["Game"] = new Dictionary<Language, string>
            {
                { Language.English, "Game" },
                { Language.Romanian, "Joc" },
                { Language.Spanish, "Juego" }
            };
            
            // Game Names
            translations["GTA III"] = new Dictionary<Language, string>
            {
                { Language.English, "GTA III" },
                { Language.Romanian, "GTA III" },
                { Language.Spanish, "GTA III" }
            };
            
            translations["GTA Vice City"] = new Dictionary<Language, string>
            {
                { Language.English, "GTA Vice City" },
                { Language.Romanian, "GTA Vice City" },
                { Language.Spanish, "GTA Vice City" }
            };
            
            translations["GTA San Andreas"] = new Dictionary<Language, string>
            {
                { Language.English, "GTA San Andreas" },
                { Language.Romanian, "GTA San Andreas" },
                { Language.Spanish, "GTA San Andreas" }
            };
            
            translations["GTA IV"] = new Dictionary<Language, string>
            {
                { Language.English, "GTA IV" },
                { Language.Romanian, "GTA IV" },
                { Language.Spanish, "GTA IV" }
            };
            
            translations["GTA V"] = new Dictionary<Language, string>
            {
                { Language.English, "GTA V" },
                { Language.Romanian, "GTA V" },
                { Language.Spanish, "GTA V" }
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
            this.Text = GetTranslation("GTA Mod Manager");
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
                Text = GetTranslation("GTA Mod Manager"),
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
                Location = new Point(380, 18),
                Size = new Size(100, 25),
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
                Location = new Point(320, 21),
                Size = new Size(60, 20)
            };
            headerPanel.Controls.Add(langLabel);
            
            // Add game selection dropdown 🎮
            gameSelectionComboBox = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = new Point(200, 18),
                Size = new Size(110, 25),
                FlatStyle = FlatStyle.Flat
            };
            
            // Add GTA game options
            gameSelectionComboBox.Items.Add(gameIcons[GTAGame.GTA3] + " GTA III");
            gameSelectionComboBox.Items.Add(gameIcons[GTAGame.ViceCity] + " GTA Vice City");
            gameSelectionComboBox.Items.Add(gameIcons[GTAGame.SanAndreas] + " GTA San Andreas");
            gameSelectionComboBox.Items.Add(gameIcons[GTAGame.GTA4] + " GTA IV");
            gameSelectionComboBox.Items.Add(gameIcons[GTAGame.GTA5] + " GTA V");
            gameSelectionComboBox.Items.Add(gameIcons[GTAGame.GTALibertyCityStories] + " GTA LCS");
            gameSelectionComboBox.Items.Add(gameIcons[GTAGame.GTAViceCityStories] + " GTA VCS");
            
            gameSelectionComboBox.SelectedIndex = 0;
            gameSelectionComboBox.SelectedIndexChanged += GameSelectionComboBox_SelectedIndexChanged;
            headerPanel.Controls.Add(gameSelectionComboBox);
            
            Label gameLabel = new Label
            {
                Text = GetTranslation("Game") + ":",
                ForeColor = Color.White,
                Location = new Point(150, 21),
                Size = new Size(50, 20)
            };
            headerPanel.Controls.Add(gameLabel);
            
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
                Text = GetTranslation("GTA Installation Path:"),
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
                Text = GetTranslation("Launch GTA"),
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
                Description = "Select GTA Installation Folder"
            };

            openFileDialog = new OpenFileDialog
            {
                Filter = "Mod Files (*.zip)|*.zip|All files (*.*)|*.*",
                Title = "Select a GTA Mod to Install"
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
        
        private void GameSelectionComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Update current game based on selection
            currentGame = (GTAGame)gameSelectionComboBox.SelectedIndex;
            
            // Update the mod path and installation path for the selected game
            gtaPath = "";
            gtaPathTextBox.Text = "";
            
            // Clear installed mods list
            installedModsListBox.Items.Clear();
            installedMods.Clear();
            
            // Find the game installation
            FindGameInstallation(currentGame);
            
            // Update launch button text with game name
            string gameName = gameSelectionComboBox.SelectedItem.ToString().Substring(2); // Remove emoji
            launchGameButton.Text = $"{GetTranslation("Launch")} {gameName}";
            
            // Update status
            statusLabel.Text = $"{gameIcons[currentGame]} {GetTranslation("Selected")} {gameName}";
        }
        
        private void FindGameInstallation(GTAGame game)
        {
            // Get possible installation paths for the selected game
            string[] possiblePaths = GetPossibleGamePaths(game);
            
            // Try to find the game in these paths
            foreach (string path in possiblePaths)
            {
                string exeName = gameExecutables[game];
                if (Directory.Exists(path) && File.Exists(Path.Combine(path, exeName)))
                {
                    gtaPath = path;
                    gtaPathTextBox.Text = gtaPath;
                    
                    // Create/update mods directory
                    modsPath = Path.Combine(gtaPath, "mods");
                    if (!Directory.Exists(modsPath))
                    {
                        Directory.CreateDirectory(modsPath);
                    }
                    
                    // Refresh the mods list
                    RefreshModsList();
                    
                    statusLabel.Text = $"{gameIcons[game]} {GetTranslation("Game found at")} {gtaPath}";
                    return;
                }
            }
            
            statusLabel.Text = $"{gameIcons[game]} {GetTranslation("Game not found, please select installation folder")}";
        }
        
        private string[] GetPossibleGamePaths(GTAGame game)
        {
            List<string> paths = new List<string>();
            
            // Add Steam paths
            string[] steamDrives = { @"C:", @"D:", @"E:", @"F:" };
            foreach (string drive in steamDrives)
            {
                switch (game)
                {
                    case GTAGame.GTA3:
                        paths.Add($@"{drive}\Program Files\Rockstar Games\Grand Theft Auto III");
                        paths.Add($@"{drive}\Program Files (x86)\Rockstar Games\Grand Theft Auto III");
                        paths.Add($@"{drive}\Program Files\Steam\steamapps\common\Grand Theft Auto 3");
                        paths.Add($@"{drive}\Steam\steamapps\common\Grand Theft Auto 3");
                        break;
                    case GTAGame.ViceCity:
                        paths.Add($@"{drive}\Program Files\Rockstar Games\Grand Theft Auto Vice City");
                        paths.Add($@"{drive}\Program Files (x86)\Rockstar Games\Grand Theft Auto Vice City");
                        paths.Add($@"{drive}\Program Files\Steam\steamapps\common\Grand Theft Auto Vice City");
                        paths.Add($@"{drive}\Steam\steamapps\common\Grand Theft Auto Vice City");
                        break;
                    case GTAGame.SanAndreas:
                        paths.Add($@"{drive}\Program Files\Rockstar Games\GTA San Andreas");
                        paths.Add($@"{drive}\Program Files (x86)\Rockstar Games\GTA San Andreas");
                        paths.Add($@"{drive}\Program Files\Steam\steamapps\common\Grand Theft Auto San Andreas");
                        paths.Add($@"{drive}\Steam\steamapps\common\Grand Theft Auto San Andreas");
                        break;
                    case GTAGame.GTA4:
                        paths.Add($@"{drive}\Program Files\Rockstar Games\Grand Theft Auto IV");
                        paths.Add($@"{drive}\Program Files (x86)\Rockstar Games\Grand Theft Auto IV");
                        paths.Add($@"{drive}\Program Files\Steam\steamapps\common\Grand Theft Auto IV");
                        paths.Add($@"{drive}\Steam\steamapps\common\Grand Theft Auto IV");
                        break;
                    case GTAGame.GTA5:
                        paths.Add($@"{drive}\Program Files\Rockstar Games\Grand Theft Auto V");
                        paths.Add($@"{drive}\Program Files (x86)\Rockstar Games\Grand Theft Auto V");
                        paths.Add($@"{drive}\Program Files\Epic Games\GTAV");
                        paths.Add($@"{drive}\Program Files\Steam\steamapps\common\Grand Theft Auto V");
                        paths.Add($@"{drive}\Steam\steamapps\common\Grand Theft Auto V");
                        break;
                    // Add paths for other games as needed
                }
            }
            
            return paths.ToArray();
        }
        
        private void UpdateUITexts()
        {
            // Update window title
            this.Text = GetTranslation("GTA Mod Manager");
            
            // Update UI component texts
            foreach (Control control in this.Controls)
            {
                if (control is Button button)
                {
                    if (button == installModButton)
                        button.Text = GetTranslation("Install New Mod");
                    else if (button == uninstallModButton)
                        button.Text = GetTranslation("Uninstall Selected Mod");
                    else if (button == launchGameButton)
                    {
                        // Update launch button with game name
                        string gameName = gameSelectionComboBox.SelectedItem.ToString().Substring(2); // Remove emoji
                        button.Text = $"{GetTranslation("Launch")} {gameName}";
                    }
                    else if (button == browseButton)
                        button.Text = GetTranslation("Browse...");
                }
                else if (control is Label label)
                {
                    if (label.Text == "GTA 3 Installation Path:" || label.Text == "Calea de instalare GTA 3:" || label.Text == "Ruta de instalación de GTA 3:" ||
                        label.Text == "GTA Installation Path:" || label.Text == "Calea de instalare GTA:" || label.Text == "Ruta de instalación de GTA:")
                        label.Text = GetTranslation("GTA Installation Path:");
                    else if (label.Text == "Installed Mods:" || label.Text == "Moduri instalate:" || label.Text == "Mods instalados:")
                        label.Text = GetTranslation("Installed Mods:");
                    else if (label.Text == "Ready" || label.Text == "Gata" || label.Text == "Listo")
                        label.Text = GetTranslation("Ready");
                }
            }
            
            // Update header panel texts
            foreach (Control control in headerPanel.Controls)
            {
                if (control is Label label && control != languageComboBox && control != gameSelectionComboBox)
                {
                    if (label.Font.Size > 12) // This is the title
                        label.Text = GetTranslation("GTA Mod Manager");
                    else if (label.Text == "Language:" || label.Text == "Limba:" || label.Text == "Idioma:")
                        label.Text = GetTranslation("Language") + ":";
                    else if (label.Text == "Game:" || label.Text == "Joc:" || label.Text == "Juego:")
                        label.Text = GetTranslation("Game") + ":";
                }
            }
            
            // Update game selection combobox with translated game names and icons
            if (gameSelectionComboBox != null)
            {
                gameSelectionComboBox.Items.Clear();
                gameSelectionComboBox.Items.Add(gameIcons[GTAGame.GTA3] + " " + GetTranslation("GTA III"));
                gameSelectionComboBox.Items.Add(gameIcons[GTAGame.ViceCity] + " " + GetTranslation("GTA Vice City"));
                gameSelectionComboBox.Items.Add(gameIcons[GTAGame.SanAndreas] + " " + GetTranslation("GTA San Andreas"));
                gameSelectionComboBox.Items.Add(gameIcons[GTAGame.GTA4] + " " + GetTranslation("GTA IV"));
                gameSelectionComboBox.Items.Add(gameIcons[GTAGame.GTA5] + " " + GetTranslation("GTA V"));
                gameSelectionComboBox.Items.Add(gameIcons[GTAGame.GTALibertyCityStories] + " " + "GTA LCS");
                gameSelectionComboBox.Items.Add(gameIcons[GTAGame.GTAViceCityStories] + " " + "GTA VCS");
                gameSelectionComboBox.SelectedIndex = (int)currentGame;
            }
        }

        private void LoadSettings()
        {
            // Find GTA installation based on currently selected game
            FindGameInstallation(currentGame);
        }

        private void BrowseButton_Click(object sender, EventArgs e)
        {
            string executableName = gameExecutables[currentGame];
            folderBrowserDialog.Description = $"Select {gameSelectionComboBox.SelectedItem.ToString()} Installation Folder";
            
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                string selectedPath = folderBrowserDialog.SelectedPath;
                if (File.Exists(Path.Combine(selectedPath, executableName)))
                {
                    gtaPath = selectedPath;
                    gtaPathTextBox.Text = gtaPath;
                    
                    modsPath = Path.Combine(gtaPath, "mods");
                    if (!Directory.Exists(modsPath))
                    {
                        Directory.CreateDirectory(modsPath);
                    }
                    RefreshModsList();
                    statusLabel.Text = $"{gameIcons[currentGame]} {GetTranslation("Game installation found!")}";
                }
                else
                {
                    MessageBox.Show($"The selected folder doesn't contain {gameSelectionComboBox.SelectedItem.ToString()} ({executableName} not found).", 
                        "Invalid Folder", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void AboutButton_Click(object sender, EventArgs e)
        {
            string aboutMessage = $"GTA Mod Manager\n\nA tool to manage mods for GTA games.";
            MessageBox.Show(aboutMessage, "About GTA Mod Manager", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void InstallModButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(gtaPath))
            {
                MessageBox.Show("Please select your GTA installation folder first.", 
                    "GTA Path Not Set", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
            string executableName = gameExecutables[currentGame];
            
            if (!string.IsNullOrEmpty(gtaPath) && File.Exists(Path.Combine(gtaPath, executableName)))
            {
                try
                {
                    // Check if game is Steam version
                    bool isSteamVersion = gtaPath.Contains("Steam") || gtaPath.Contains("steamapps");
                    
                    if (isSteamVersion)
                    {
                        // Launch through Steam protocol - steam appids
                        Dictionary<GTAGame, string> steamAppIds = new Dictionary<GTAGame, string>
                        {
                            { GTAGame.GTA3, "12100" },
                            { GTAGame.ViceCity, "12110" },
                            { GTAGame.SanAndreas, "12120" },
                            { GTAGame.GTA4, "12210" },
                            { GTAGame.GTA5, "271590" }
                        };
                        
                        string appId = steamAppIds.ContainsKey(currentGame) ? steamAppIds[currentGame] : "";
                        if (!string.IsNullOrEmpty(appId))
                        {
                            Process.Start($"steam://run/{appId}");
                            statusLabel.Text = $"{gameIcons[currentGame]} {GetTranslation("Game launched through Steam")}";
                        }
                        else
                        {
                            // Fallback to direct launch
                            LaunchGameDirect();
                        }
                    }
                    else if (gtaPath.Contains("Epic"))
                    {
                        // Launch Epic Games version (particularly for GTA 5)
                        if (currentGame == GTAGame.GTA5)
                        {
                            Process.Start("com.epicgames.launcher://apps/9d2d0eb64d5c44529cece33fe2a46482?action=launch&silent=true");
                            statusLabel.Text = $"{gameIcons[currentGame]} {GetTranslation("Game launched through Epic Games")}";
                        }
                        else
                        {
                            LaunchGameDirect();
                        }
                    }
                    else
                    {
                        LaunchGameDirect();
                    }
                    
                    if (MinimizeOnLaunch)
                    {
                        this.WindowState = FormWindowState.Minimized;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error launching game: {ex.Message}", 
                        "Launch Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show($"Game executable ({executableName}) not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void LaunchGameDirect()
        {
            string executableName = gameExecutables[currentGame];
            
            // Launch parameters specific to each game
            Dictionary<GTAGame, string> launchParams = new Dictionary<GTAGame, string>
            {
                { GTAGame.GTA3, "-nomipmap -refresh:60" }, // GTA 3 specific params
                { GTAGame.ViceCity, "-nomipmap -refresh:60" }, // Vice City specific params
                { GTAGame.SanAndreas, "-skip" }, // San Andreas specific params
                { GTAGame.GTA4, "" }, // GTA 4 specific params
                { GTAGame.GTA5, "-StraightIntoFreemode" }, // GTA 5 specific params
                { GTAGame.GTALibertyCityStories, "" },
                { GTAGame.GTAViceCityStories, "" }
            };
            
            string parameters = launchParams.ContainsKey(currentGame) ? launchParams[currentGame] : "";
            
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = Path.Combine(gtaPath, executableName),
                WorkingDirectory = gtaPath,
                Arguments = parameters
            };
            Process.Start(startInfo);
            statusLabel.Text = $"{gameIcons[currentGame]} {GetTranslation("Game launched successfully!")}";
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