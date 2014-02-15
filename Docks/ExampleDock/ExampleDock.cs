using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Script.Scripts.ExampleDock
{
    using SmokeLounge.AOtomation.Messaging.Messages;
    using SmokeLounge.AOtomation.Messaging.Messages.N3Messages;

    using WeifenLuo.WinFormsUI.Docking;

    public partial class ExampleDock : DockContent, IAOToolerScript
    {
        public ExampleDock()
        {
            InitializeComponent();
        }

        public void Initialize(string[] args)
        {
            textBox1.Text = "";
        }

        public List<N3MessageType> GetPacketWatcherList()
        {
            List<N3MessageType> types = new List<N3MessageType>(){N3MessageType.CharDCMove};
            return types;
        }

        public void PushPacket(N3MessageType type, N3Message message)
        {
            switch (type)
            {
                case N3MessageType.CharDCMove:
                    CharDCMoveMessage moveMessage = (CharDCMoveMessage)message;
                    textBox1.Text = "X: " + moveMessage.Coordinates.X.ToString("0.0") + Environment.NewLine + "Y: "
                                    + moveMessage.Coordinates.Y.ToString("0.0") + "Z: "
                                    + moveMessage.Coordinates.Z.ToString("0.0");
                    break;
            }
        }
    }
}
