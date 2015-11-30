using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hearthstone_Deck_Tracker.Plugins;
using System.Windows.Controls;


namespace RenoTime
{
    public class RenoPlugin : IPlugin
    {
        public string Author
        {
            get { return "Justin Donohoo"; }
        }

        public string ButtonText
        {
            get { return "Setttings"; }
        }

        public string Description
        {
            get { return "Reno Alert"; }
        }

        public MenuItem MenuItem
        {
            get { return null; }
        }

        public string Name
        {
            get { return "RenoTime"; }
        }

        public void OnButtonPress()
        {
        }

        public void OnLoad()
        {
            RenoCode.Load();
        }

        public void OnUnload()
        {
        }

        public void OnUpdate()
        {
        }

        public Version Version
        {
            get { return new Version(0, 0, 0, 1); }
        }

    }
}
