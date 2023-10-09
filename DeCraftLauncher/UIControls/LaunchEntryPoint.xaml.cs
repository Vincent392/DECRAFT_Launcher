﻿using DeCraftLauncher.Configs;
using DeCraftLauncher.Configs.UI;
using DeCraftLauncher.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
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
using System.Windows.Navigation;

namespace DeCraftLauncher.UIControls
{
    /// <summary>
    /// Logika interakcji dla klasy LaunchEntryPoint.xaml
    /// </summary>
    public partial class LaunchEntryPoint : UserControl
    {
        JarUtils.EntryPoint entryPoint;
        JarConfig jarConfig;
        MainWindow caller;

        public string GetDescription()
        {
            if (entryPoint.type == JarUtils.EntryPointType.CUSTOM_LAUNCH_COMMAND)
            {
                return Util.CleanStringForXAML(entryPoint.classpath);
            }
            switch (entryPoint.classpath)
            {
                case "com.mojang.rubydung.RubyDung":
                case "com.mojang.minecraft.RubyDung":
                case "com.mojang.minecraft.Minecraft":
                case "net.minecraft.client.Minecraft":
                    return MainWindow.locale.GetLocString("l.ui.entry_point_desc_direct_launch");
                case "com.mojang.minecraft.MinecraftApplet":
                case "net.minecraft.client.MinecraftApplet":
                    return MainWindow.locale.GetLocString("l.ui.entry_point_desc_launch_applet");
                case "com.mojang.minecraft.server.MinecraftServer":
                case "net.minecraft.server.MinecraftServer":
                    return MainWindow.locale.GetLocString("l.ui.entry_point_desc_launch_server");
                case "net.minecraft.isom.IsomPreviewApplet":
                    return MainWindow.locale.GetLocString("l.ui.entry_point_desc_launch_isom");
                case "Start":
                    return MainWindow.locale.GetLocString("l.ui.entry_point_desc_launch_rmcp_wrapper");
                case "net.minecraft.client.main.Main":
                case "net.minecraft.data.Main":
                    return MainWindow.locale.GetLocString("l.ui.entry_point_desc_onesix_unsupported");
                case "decraft_internal.AppletWrapper":
                    return "What";
                default:
                    if (entryPoint.classpath.StartsWith("com.jdotsoft.jarloader"))
                    {
                        return MainWindow.locale.GetLocString("l.ui.entry_point_desc_launch_jarloader");
                    }
                    return MainWindow.locale.GetLocString("l.ui.entry_point_desc_unknown");
            }
        }

        public LaunchEntryPoint(JarUtils.EntryPoint target, MainWindow caller, JarConfig jarConfig)
        {
            InitializeComponent();
            this.caller = caller;
            this.jarConfig = jarConfig;
            this.entryPoint = target;

            string cleanClassPath = Util.CleanStringForXAML(entryPoint.classpath);
            cleanClassPath = cleanClassPath.Length > 50 ? cleanClassPath.Substring(0, 50) + "..." : cleanClassPath;
            classname.Content = entryPoint.type == JarUtils.EntryPointType.CUSTOM_LAUNCH_COMMAND ? "Custom" : cleanClassPath;
            classname.ToolTip = Util.CleanStringForXAML(entryPoint.classpath);

            desc.Content = GetDescription();
            mode.Content =
                entryPoint.type == JarUtils.EntryPointType.STATIC_VOID_MAIN ? MainWindow.locale.GetLocString("l.ui.entry_point_type_main_function")
                : entryPoint.type == JarUtils.EntryPointType.RUNNABLE ? "(Runnable)"
                : entryPoint.type == JarUtils.EntryPointType.APPLET ? MainWindow.locale.GetLocString("l.ui.entry_point_type_applet")
                : entryPoint.type == JarUtils.EntryPointType.CUSTOM_LAUNCH_COMMAND ? MainWindow.locale.GetLocString("l.ui.entry_point_type_custom_command")
                : MainWindow.locale.GetLocString("l.ui.entry_point_type_unknown");
            moreInfo.Content = Util.CleanStringForXAML(target.additionalInfo);
            switch (entryPoint.type)
            {
                case JarUtils.EntryPointType.CUSTOM_LAUNCH_COMMAND:
                    launchAdvancedButton.Content = MainWindow.locale.GetLocString("btn.ui.entry_point_adv_remove_custom_command");
                    break;
                case JarUtils.EntryPointType.APPLET:
                    launchAdvancedButton.Content = MainWindow.locale.GetLocString("btn.ui.entry_point_adv_applet_parameters");
                    break;
                default:
                    launchAdvancedButton.Visibility = Visibility.Hidden;
                    break;
            }
            
        }

        private void launchButton_Click(object sender, RoutedEventArgs e)
        {
            caller.SaveCurrentJarConfig();
            if (jarConfig.LWJGLVersion == "+ built-in")
            {
                MainWindow.EnsureDir($"{MainWindow.currentDirectory}/lwjgl/_temp_builtin");
                MainWindow.EnsureDir($"{MainWindow.currentDirectory}/lwjgl/_temp_builtin/native");
                ZipArchive zip = ZipFile.OpenRead(Path.GetFullPath(MainWindow.jarDir + "/" + jarConfig.jarFileName));
                var dllFilesToExtract = (from x in zip.Entries where x.FullName.StartsWith($"{jarConfig.jarBuiltInLWJGLDLLs}") && x.Name.EndsWith(".dll") select x);
                DirectoryInfo nativesdir = new DirectoryInfo($"{MainWindow.currentDirectory}/lwjgl/_temp_builtin/native");
                foreach (FileInfo f in nativesdir.EnumerateFiles())
                {
                    f.Delete();
                }

                foreach (ZipArchiveEntry dllFile in dllFilesToExtract)
                {
                    dllFile.ExtractToFile($"{MainWindow.currentDirectory}/lwjgl/_temp_builtin/native/{dllFile.Name}");
                }
                Console.WriteLine("Extracted temp LWJGL natives");
            }

            MainWindow.EnsureDir(MainWindow.instanceDir + "/" + jarConfig.instanceDirName);
            if (jarConfig.cwdIsDotMinecraft)
            {
                MainWindow.EnsureDir(MainWindow.instanceDir + "/" + jarConfig.instanceDirName + "/.minecraft");
            }

            if (entryPoint.type == JarUtils.EntryPointType.STATIC_VOID_MAIN)
            {
                if (Keyboard.IsKeyDown(Key.LeftShift))
                {
                    //todo: make this an advanced option
                    MainFunctionWrapper.LaunchMainFunctionWrapper(entryPoint.classpath, jarConfig);
                } 
                else 
                {
                    JavaExec mainFunctionExec = new JavaExec(entryPoint.classpath);

                    mainFunctionExec.classPath.Add(Path.GetFullPath(MainWindow.jarDir + "/" + jarConfig.jarFileName));
                    if (jarConfig.LWJGLVersion != "+ built-in")
                    {
                        //mainFunctionExec.classPath.Add($"{MainWindow.currentDirectory}/lwjgl/{jarConfig.LWJGLVersion}/*");

                        //the above is a much more efficient way of doing this right???
                        //yeah
                        //but it doesn't work with java 5 which can't handle wildcards

                        try
                        {
                            foreach (string f in Directory.GetFiles($"{MainWindow.currentDirectory}/lwjgl/{jarConfig.LWJGLVersion}/"))
                            {
                                if (f.EndsWith(".jar"))
                                {
                                    mainFunctionExec.classPath.Add(f);
                                }
                            }
                        } catch (Exception)
                        {
                            // don't care i think
                        }
                    }

                    if (jarConfig.proxyHost != "")
                    {
                        mainFunctionExec.jvmArgs.Add($"-Dhttp.proxyHost={jarConfig.proxyHost.Replace(" ", "%20")}");
                    }
                    mainFunctionExec.jvmArgs.Add($"-Djava.library.path=\"{MainWindow.currentDirectory}/lwjgl/{(jarConfig.LWJGLVersion == "+ built-in" ? "_temp_builtin" : jarConfig.LWJGLVersion)}/native\"");
                    mainFunctionExec.jvmArgs.Add(jarConfig.jvmArgs);

                    mainFunctionExec.programArgs.Add($"\"{jarConfig.playerName}\"");
                    mainFunctionExec.programArgs.Add(jarConfig.sessionID);
                    mainFunctionExec.programArgs.Add(jarConfig.gameArgs);
                    Console.WriteLine("Running command: java " + mainFunctionExec.GetFullArgsString());

                    string emulatedAppDataDir = Path.GetFullPath($"{MainWindow.currentDirectory}/{MainWindow.instanceDir}/{jarConfig.instanceDirName}");
                    mainFunctionExec.appdataDir = emulatedAppDataDir;
                    mainFunctionExec.workingDirectory = $"{emulatedAppDataDir}{(jarConfig.cwdIsDotMinecraft ? "/.minecraft" : "")}";
                    try
                    {
                        new WindowProcessLog(mainFunctionExec.Start()).Show();
                    }
                    catch (Win32Exception w32e)
                    {
                        MessageBox.Show($"Error launching java process: {w32e.Message}\n\nVerify that Java is installed in \"Runtime settings\".");
                    }
                }
            } 
            else if (entryPoint.type == JarUtils.EntryPointType.APPLET)
            {
                AppletWrapper.TryLaunchAppletWrapper(entryPoint.classpath, jarConfig);
            }
            else if (entryPoint.type == JarUtils.EntryPointType.CUSTOM_LAUNCH_COMMAND)
            {
                new WindowProcessLog(new JavaExec(null).StartWithCustomArgsString(entryPoint.classpath)).Show();
            }
            else
            {
                throw new NotImplementedException("What");
            }
        }

        private void launchAdvancedButton_Click(object sender, RoutedEventArgs e)
        {
            caller.SaveCurrentJarConfig();
            if (entryPoint.type == JarUtils.EntryPointType.APPLET)
            {
                new WindowAppletParametersOptions(entryPoint.classpath, jarConfig).Show();
            } else if (entryPoint.type == JarUtils.EntryPointType.CUSTOM_LAUNCH_COMMAND)
            {
                if (MessageBox.Show($"Remove the custom launch command?\n\njava {this.entryPoint.classpath}", "DECRAFT", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    jarConfig.entryPoints.Remove(this.entryPoint);
                    jarConfig.SaveToXMLDefault();
                    caller.UpdateLaunchOptionsSegment();
                }
            }
        }
    }
}
