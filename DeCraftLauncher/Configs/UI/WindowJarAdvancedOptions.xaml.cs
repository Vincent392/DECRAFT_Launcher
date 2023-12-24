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

namespace DeCraftLauncher.Configs.UI
{
    /// <summary>
    /// Logika interakcji dla klasy JarAdvancedOptions.xaml
    /// </summary>
    public partial class WindowJarAdvancedOptions : AcrylicWindow
    {
        JarConfig targetConfig;
        MainWindow parent;
        public WindowJarAdvancedOptions(JarConfig target, MainWindow parent)
        {
            this.targetConfig = target;
            this.parent = parent;
            InitializeComponent();
            label_title.Content = $"Advanced options: {Util.CleanStringForXAML(target.jarFileName)}";
            LoadConfig();
            Util.UpdateAcrylicWindowBackground(this);

            GlobalVars.L.Translate(
                    this,
                    label_title,
                    label_sessionid,
                    label_gameargs,
                    label_wrapperheader,
                    label_emulatehttp,
                    label_custom_documentbase,
                    label_redirectskins,
                    label_processdir,
                    label_synthclassworkaround,
                    label_appletlaunchmethod,
                    btn_addcmdentrypoint,
                    btn_setjarlibs
                );
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            SaveConfig();
        }

        public void LoadConfig()
        {
            tbox_sessionid.Text = targetConfig.sessionID;
            tbox_gameargs.Text = targetConfig.gameArgs;
            checkbox_cwdisdotminecraft.IsChecked = targetConfig.cwdIsDotMinecraft;
            checkbox_emulatehttp.IsChecked = targetConfig.appletEmulateHTTP;
            tbox_appletdocumenturl.Text = targetConfig.documentBaseUrl;
            checkbox_redirecttolocalskins.IsChecked = targetConfig.appletRedirectSkins;
            tbox_skinredirectpath.Text = targetConfig.appletSkinRedirectPath;
            checkbox_workaroundretromcp.IsChecked = targetConfig.workaroundRetroMCP;
            (targetConfig.appletLaunchMode == JarConfig.AppletLaunchMode.appletviewer ? rbutton_appletmode_appletviewer
             : targetConfig.appletLaunchMode == JarConfig.AppletLaunchMode.AppletWrapper ? rbutton_appletmode_wrapper
             : rbutton_appletmode_wrapper).IsChecked = true;
        }

        public void SaveConfig()
        {
            targetConfig.sessionID = tbox_sessionid.Text;
            targetConfig.gameArgs = tbox_gameargs.Text;
            targetConfig.cwdIsDotMinecraft = checkbox_cwdisdotminecraft.IsChecked == true;
            targetConfig.appletEmulateHTTP = checkbox_emulatehttp.IsChecked == true;
            targetConfig.documentBaseUrl = tbox_appletdocumenturl.Text;
            targetConfig.appletRedirectSkins = checkbox_redirecttolocalskins.IsChecked == true;
            targetConfig.appletSkinRedirectPath = tbox_skinredirectpath.Text;
            targetConfig.workaroundRetroMCP = checkbox_workaroundretromcp.IsChecked == true;
            targetConfig.appletLaunchMode =
                rbutton_appletmode_wrapper.IsChecked == true ? JarConfig.AppletLaunchMode.AppletWrapper
                : rbutton_appletmode_appletviewer.IsChecked == true ? JarConfig.AppletLaunchMode.appletviewer
                : JarConfig.AppletLaunchMode.UNDEFINED;
            targetConfig.SaveToXMLDefault();
        }

        private void btn_addcmdentrypoint_Click(object sender, RoutedEventArgs e)
        {
            new WindowAddCustomLaunch(targetConfig, parent).Show();
        }

        private void btn_setjarlibs_Click(object sender, RoutedEventArgs e)
        {
            new WindowSetJarLibs(targetConfig).Show();
        }
    }
}
