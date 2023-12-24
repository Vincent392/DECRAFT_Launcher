﻿using DeCraftLauncher.Configs;
using DeCraftLauncher.Utils;
using SourceChord.FluentWPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace DeCraftLauncher.Configs.UI
{
    /// <summary>
    /// Logika interakcji dla klasy AppletParametersOptions.xaml
    /// </summary>
    public partial class WindowAppletParametersOptions : AcrylicWindow
    {
        string targetClassName;
        JarConfig targetJarConfig;
        MainWindow parent;

        public WindowAppletParametersOptions(string className, MainWindow parent, JarConfig jar)
        {
            this.parent = parent;
            targetClassName = className;
            targetJarConfig = jar;
            InitializeComponent();
            Util.UpdateAcrylicWindowBackground(this);

            GlobalVars.L.Translate(
                    label_header,
                    btn_launch
                );
        }

        private void AddToDictionaryIfStringNotEmpty(Dictionary<string,string> target, string key, string value, bool addAnyway = false)
        {
            if (value != "" || addAnyway)
            {
                target[key] = value;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            AddToDictionaryIfStringNotEmpty(parameters, "server", tbox_param_server.Text);
            AddToDictionaryIfStringNotEmpty(parameters, "port", tbox_param_port.Text);
            AddToDictionaryIfStringNotEmpty(parameters, "mppass", tbox_param_mppass.Text, tbox_param_server.Text != "");
            AddToDictionaryIfStringNotEmpty(parameters, "loadmap_user", tbox_param_loadmap_user.Text);
            AddToDictionaryIfStringNotEmpty(parameters, "loadmap_id", tbox_param_loadmap_id.Text);
            AddToDictionaryIfStringNotEmpty(parameters, "fullscreen", tbox_param_fullscreen.Text);
            if (targetJarConfig.EvalPrereqs())
            {
                AppletWrapper.TryLaunchAppletWrapper(targetClassName, parent, targetJarConfig, parameters);
            }
            this.Close();
        }
    }
}
