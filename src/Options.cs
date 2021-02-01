﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using PluginTranslation;
using KeePass.Resources;

namespace EarlyUpdateCheck
{
	public partial class Options : UserControl
	{
		public EarlyUpdateCheckExt Plugin = null;
		public Options()
		{
			InitializeComponent();
			gCheckSync.Text = PluginTranslate.Active;
			cbCheckSync.Text = PluginTranslate.CheckSync;
			tbCheckSyncDesc.Lines = PluginTranslate.CheckSyncDesc.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
			gOneClickUpdate.Text = PluginTranslate.PluginUpdateOneClick;
			tbOneClickUpdateDesc.Lines = PluginTranslate.PluginUpdateOneClickDesc.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
			if (string.IsNullOrEmpty(KeePass.Program.Translation.Properties.Iso6391Code))
				cbDownloadCurrentTranslation.Text = string.Format(PluginTranslate.TranslationDownload_DownloadCurrent, "English");
			else
				cbDownloadCurrentTranslation.Text = string.Format(PluginTranslate.TranslationDownload_DownloadCurrent, KeePass.Program.Translation.Properties.NameNative);
			bUpdateTranslations.Text = PluginTranslate.TranslationDownload_Update;
			if (PluginUpdateHandler.Shieldify) KeePass.UI.UIUtil.SetShield(bUpdateTranslations, true);
		}

		private void bUpdateTranslations_Click(object sender, EventArgs e)
		{
			List<OwnPluginUpdate> lPlugins = new List<OwnPluginUpdate>();
			foreach (PluginUpdate pu in PluginUpdateHandler.Plugins)
			{
				OwnPluginUpdate opu = pu as OwnPluginUpdate;
				if (opu == null) continue;
				if (!PluginUpdateHandler.VersionsEqual(pu.VersionInstalled, pu.VersionAvailable)) continue;
				if (opu.Translations.Count == 0) continue;
				if (!lPlugins.Contains(opu)) lPlugins.Add(opu);
			}
			if (lPlugins.Count == 0)
			{
				PluginTools.PluginDebug.AddInfo("No plugins where translations can be updated", 0);
				return;
			}
			using (TranslationUpdateForm t = new TranslationUpdateForm())
			{
				t.InitEx(lPlugins);
				if (t.ShowDialog() == DialogResult.OK)
				{
					Plugin.UpdatePluginTranslations(PluginConfig.DownloadActiveLanguage, t.SelectedPlugins);
				}
			}
		}

		private void gCheckSync_CheckedChanged(object sender, RookieUI.CheckedGroupCheckEventArgs e)
		{
			cbCheckSync.Enabled = gCheckSync.Checked;
		}

		private void Options_Load(object sender, EventArgs e)
		{
			tpEUCOptions.Text = KPRes.Options;
			tpEUC3rdParty.Text = KPRes.More;
			lFile.Text = KPRes.File;
			tbFile.Text = UpdateInfoParser.PluginInfoFile;
			if (System.IO.File.Exists(UpdateInfoParser.PluginInfoFile))
			{
				lFile.Links.Add(0, lFile.Text.Length);
				lFile.LinkClicked += LFile_LinkClicked;
			}
			foreach (var p in PluginUpdateHandler.Plugins)
			{
				string s = p.Title;
				if (p is OtherPluginUpdate) s += " - " + p.UpdateMode.ToString();
				lv3rdPartyPlugins.Items.Add(s);
			}
		}
		private void LFile_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			PluginTools.Tools.OpenUrl(UpdateInfoParser.PluginInfoFile);
		}
	}
}
