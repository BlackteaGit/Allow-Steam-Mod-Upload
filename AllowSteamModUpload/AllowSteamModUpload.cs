using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using CoOpSpRpG;
using WTFModLoader.Manager;
using WTFModLoader;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Threading;
using System.IO;
using System.Windows.Forms;

namespace AllowSteamModUpload
{
   
    public class ModifyCheck : IWTFMod
    {
        public ModLoadPriority Priority => ModLoadPriority.Normal;
        public void Initialize()
        {
            Harmony harmony = new Harmony("blacktea.AllowSteamModUpload");
            harmony.PatchAll();
        }

    }

	public class MyModUploadScreen
	{
	
		public static string dialogueSelectionPathOut = "";
		public static void MyfolderSelectDialogueBox()
		{
			
			using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
			{
				folderBrowserDialog.SelectedPath = Directory.GetCurrentDirectory() + "\\Dumps\\";
				if (folderBrowserDialog.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(folderBrowserDialog.SelectedPath))
				{
					MyModUploadScreen.dialogueSelectionPathOut = folderBrowserDialog.SelectedPath;
				}
				else
				{
					MyModUploadScreen.dialogueSelectionPathOut = null;
				}
			}
		}

	}

	[HarmonyPatch(typeof(ModUploadScreen), "selectFolder")]
        public class ModUploadScreen_selectFolder
        {
            [HarmonyPrefix]
            private static Boolean Prefix(ref string ___dialogueSelectionPathOut, ref string ___dataDir, ref bool ___validDir, ref TextInput ___pathEditor, ref string ___foundFiles)
            {
	
			Thread thread = new Thread(new ThreadStart(MyModUploadScreen.MyfolderSelectDialogueBox));
			thread.SetApartmentState(ApartmentState.STA);
			thread.Start();
			while (thread.IsAlive)
			{
				Thread.Sleep(100);
			}

			___dialogueSelectionPathOut = MyModUploadScreen.dialogueSelectionPathOut;
			bool flag = ___dialogueSelectionPathOut != null;
				if (flag)
				{
					bool flag2 = ___dialogueSelectionPathOut == ___dataDir;
					if (flag2)
					{
						SCREEN_MANAGER.alerts.Enqueue("Please don't upload the data directory");
					___validDir = false;
					}
					else
					{
					___pathEditor.text = ___dialogueSelectionPathOut;
						string[] files = Directory.GetFiles(___dialogueSelectionPathOut);
						___validDir = (files.Length != 0);
					___foundFiles = "Files in selected directory:\n";
						foreach (string path in files)
						{
							string fileName = Path.GetFileName(path);
						___foundFiles += fileName;
							bool flag3 = !fileName.EndsWith(".flt") && !fileName.EndsWith(".bdm") && !fileName.EndsWith(".tdb") && !fileName.EndsWith(".dll") && !fileName.EndsWith(".txt") && !fileName.EndsWith(".json") && !fileName.EndsWith(".xnb");
							if (flag3)
							{
							___foundFiles += " (invalid file type)";
								___validDir = false;
							}
						___foundFiles += "\n";
						}
					}
				}
				else
				{
					___validDir = false;
				}
				return false;
			}
        }
}
