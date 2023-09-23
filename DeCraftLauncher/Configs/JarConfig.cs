﻿using DeCraftLauncher.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using static DeCraftLauncher.Utils.JarUtils;

namespace DeCraftLauncher.Configs
{

    public class JarConfig
    {
        public string friendlyName = "";
        public string jarFileName;
        public List<EntryPoint> entryPoints = new List<EntryPoint>();
        public List<string> foundMods = new List<string>();
        public string LWJGLVersion = "2.9.3";
        public string playerName = "DECRAFT_Player";
        public bool entryPointsScanned = false;
        public string instanceDirName = "";
        public string jvmArgs = "-Djava.util.Arrays.useLegacyMergeSort=true";
        public string proxyHost = "";
        public uint windowW = 960;
        public uint windowH = 540;
        public string maxJavaVersion = "";
        public string minJavaVersion = "";

        //advanced options
        public string sessionID = "0";
        public string gameArgs = "";
        public bool cwdIsDotMinecraft = true;
        public bool appletEmulateHTTP = true;
        public string documentBaseUrl = "http://www.minecraft.net/play.jsp";
        public bool appletRedirectSkins = true;
        public string appletSkinRedirectPath = "C:\\skincache";
        public bool jarHasLWJGLClasses = false;
        public string jarBuiltInLWJGLDLLs = "";

        public JarConfig(string jarFileName)
        {
            this.jarFileName = jarFileName;
            instanceDirName = jarFileName;
        }

        private XmlNode SetVal(XmlNode a, string v)
        {
            a.InnerText = v;
            return a;
        }

        public void SaveToXMLDefault()
        {
            SaveToXML($"{MainWindow.configDir}/{jarFileName}.xml");
        }

        public void SaveToXML(string path)
        {
            XmlDocument newXml = new XmlDocument();
            newXml.LoadXml("<?xml version=\"1.0\"?>\n<JarConfig>\n</JarConfig>");
            XmlNode rootElement = newXml.GetElementsByTagName("JarConfig")[0];
            rootElement.AppendChild(Util.GenElementChild(newXml, "FriendlyName", friendlyName));
            rootElement.AppendChild(Util.GenElementChild(newXml, "LWJGLVersion", LWJGLVersion));
            rootElement.AppendChild(Util.GenElementChild(newXml, "PlayerName", playerName));
            rootElement.AppendChild(Util.GenElementChild(newXml, "InstanceDirectory", instanceDirName));
            rootElement.AppendChild(Util.GenElementChild(newXml, "JVMArgs", jvmArgs));
            rootElement.AppendChild(Util.GenElementChild(newXml, "WindowW", windowW.ToString()));
            rootElement.AppendChild(Util.GenElementChild(newXml, "WindowH", windowH + ""));
            rootElement.AppendChild(Util.GenElementChild(newXml, "ProxyHost", proxyHost));

            //advanced options
            rootElement.AppendChild(Util.GenElementChild(newXml, "PassSessionID", sessionID));
            rootElement.AppendChild(Util.GenElementChild(newXml, "GameArgs", gameArgs));
            rootElement.AppendChild(Util.GenElementChild(newXml, "JavaCwdIsDotMinecraft", cwdIsDotMinecraft.ToString()));
            rootElement.AppendChild(Util.GenElementChild(newXml, "AppletEmulateHTTP", appletEmulateHTTP.ToString()));
            rootElement.AppendChild(Util.GenElementChild(newXml, "AppletDocumentURL", documentBaseUrl));
            rootElement.AppendChild(Util.GenElementChild(newXml, "AppletRedirectToLocalSkins", appletRedirectSkins.ToString()));
            rootElement.AppendChild(Util.GenElementChild(newXml, "AppletSkinRedirectLocalPath", appletSkinRedirectPath));


            //generated by jar scanner
            XmlNode nodeValsAutogenerated = rootElement.AppendChild(Util.GenElementChild(newXml, "AutoGenerated"));
            nodeValsAutogenerated.AppendChild(Util.GenElementChild(newXml, "EntryPointsScanned", entryPointsScanned.ToString()));
            nodeValsAutogenerated.AppendChild(Util.GenElementChild(newXml, "MaxJavaVersion", maxJavaVersion));
            nodeValsAutogenerated.AppendChild(Util.GenElementChild(newXml, "MinJavaVersion", minJavaVersion));
            nodeValsAutogenerated.AppendChild(Util.GenElementChild(newXml, "JarHasLWJGL", jarHasLWJGLClasses.ToString()));
            nodeValsAutogenerated.AppendChild(Util.GenElementChild(newXml, "JarLWJGLNativesDir", jarBuiltInLWJGLDLLs));

            XmlNode nodeFoundMods = nodeValsAutogenerated.AppendChild(Util.GenElementChild(newXml, "MLMods"));
            foreach (string x in foundMods)
            {
                nodeFoundMods.AppendChild(Util.GenElementChild(newXml, "MLMod", x));
            }

            XmlNode entryPointsList = nodeValsAutogenerated.AppendChild(Util.GenElementChild(newXml, "EntryPoints"));
            foreach (EntryPoint a in entryPoints)
            {
                XmlNode nEntryPoint = entryPointsList.AppendChild(Util.GenElementChild(newXml, "EntryPoint"));
                nEntryPoint.AppendChild(Util.GenElementChild(newXml, "ClassPath", a.classpath));
                nEntryPoint.AppendChild(Util.GenElementChild(newXml, "LaunchType", ((int)a.type).ToString()));
                nEntryPoint.AppendChild(Util.GenElementChild(newXml, "ClassInfo", a.additionalInfo));
            }

            newXml.Save(path);
        }

        public static JarConfig LoadFromXML(string path, string jarName)
        {
            JarConfig newJarConf = new JarConfig(jarName);

            XmlDocument newXml = new XmlDocument();
            newXml.Load(path);
            XmlNode rootNode = newXml.SelectSingleNode("JarConfig");
            if (rootNode != null)
            {

                newJarConf.friendlyName = Util.GetInnerOrDefault(rootNode, "FriendlyName");
                newJarConf.LWJGLVersion = Util.GetInnerOrDefault(rootNode, "LWJGLVersion", "2.9.3");
                newJarConf.playerName = Util.GetInnerOrDefault(rootNode, "PlayerName", "DECRAFT_player");
                newJarConf.jvmArgs = Util.GetInnerOrDefault(rootNode, "JVMArgs", "-Djava.util.Arrays.useLegacyMergeSort=true");
                newJarConf.instanceDirName = Util.GetInnerOrDefault(rootNode, "InstanceDirectory", jarName);
                newJarConf.windowW = uint.Parse(Util.GetInnerOrDefault(rootNode, "WindowW", "960", "uint"));
                newJarConf.windowH = uint.Parse(Util.GetInnerOrDefault(rootNode, "WindowH", "540", "uint"));
                newJarConf.proxyHost = Util.GetInnerOrDefault(rootNode, "ProxyHost");

                newJarConf.sessionID = Util.GetInnerOrDefault(rootNode, "SessionID", "0");
                newJarConf.gameArgs = Util.GetInnerOrDefault(rootNode, "GameArgs");
                newJarConf.cwdIsDotMinecraft = bool.Parse(Util.GetInnerOrDefault(rootNode, "JavaCwdIsDotMinecraft", "true", "bool"));
                newJarConf.appletEmulateHTTP = bool.Parse(Util.GetInnerOrDefault(rootNode, "AppletEmulateHTTP", "true", "bool"));
                newJarConf.documentBaseUrl = Util.GetInnerOrDefault(rootNode, "AppletDocumentURL", "http://www.minecraft.net/play.jsp");
                newJarConf.appletRedirectSkins = bool.Parse(Util.GetInnerOrDefault(rootNode, "AppletRedirectToLocalSkins", "true", "bool"));
                newJarConf.appletSkinRedirectPath = Util.GetInnerOrDefault(rootNode, "AppletSkinRedirectLocalPath", "C:\\skincache");

                //generated by jar scanner
                XmlNode nodeValsAutogenerated = rootNode.SelectSingleNode("AutoGenerated");
                if (nodeValsAutogenerated != null)
                {
                    newJarConf.entryPointsScanned = bool.Parse(Util.GetInnerOrDefault(nodeValsAutogenerated, "EntryPointsScanned", "false", "bool"));
                    newJarConf.maxJavaVersion = Util.GetInnerOrDefault(nodeValsAutogenerated, "MaxJavaVersion");
                    newJarConf.minJavaVersion = Util.GetInnerOrDefault(nodeValsAutogenerated, "MinJavaVersion");
                    newJarConf.jarHasLWJGLClasses = bool.Parse(Util.GetInnerOrDefault(nodeValsAutogenerated, "JarHasLWJGL", "false", "bool"));
                    newJarConf.jarBuiltInLWJGLDLLs = Util.GetInnerOrDefault(nodeValsAutogenerated, "JarLWJGLNativesDir");

                    XmlNode mlModsNode = nodeValsAutogenerated.SelectSingleNode("MLMods");
                    if (mlModsNode != null)
                    {
                        foreach (XmlNode foundModNode in mlModsNode.SelectNodes("MLMod"))
                        {
                            newJarConf.foundMods.Add(foundModNode.InnerText);
                        }
                    }
                }
                foreach (XmlNode a in newXml.GetElementsByTagName("EntryPoint"))
                {
                    EntryPoint b = new EntryPoint();
                    string classPath = Util.GetInnerOrDefault(a, "ClassPath", null);
                    string type = Util.GetInnerOrDefault(a, "LaunchType", null, "int");
                    string additionalInfo = Util.GetInnerOrDefault(a, "ClassInfo", "");
                    if (classPath == null || type == null)
                    {
                        continue;
                    }
                    b.classpath = classPath;
                    try
                    {
                        b.type = (EntryPointType)int.Parse(type);
                    }
                    catch (FormatException)
                    {
                        continue;
                    }
                    b.additionalInfo = additionalInfo;
                    newJarConf.entryPoints.Add(b);
                }
            }

            return newJarConf;
        }
    }
}
