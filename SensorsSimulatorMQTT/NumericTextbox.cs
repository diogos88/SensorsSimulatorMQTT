using System;
using System.Windows.Forms;

namespace diogos88.MQTT.SensorsSimulator
{
   public partial class NumericTextbox : TextBox
   {
      public bool EnableDecimal { get; set; }

      public NumericTextbox()
      {
         InitializeComponent();

         this.KeyPress += TextBox_KeyPress;
         this.TextChanged += TextBox_TextChanged;
      }

      private void TextBox_TextChanged(object sender, EventArgs e)
      {
         if (Text.Equals(""))
         {
            Text = "0";
            SelectionStart = Text.Length;
            SelectionLength = 0;
         }
         else if (Text.StartsWith("0") && Text.Length > 1)
         {
            Text = Text.Substring(1);
            SelectionStart = Text.Length;
            SelectionLength = 0;
         }
      }

      private void TextBox_KeyPress(object sender, KeyPressEventArgs e)
      {
         var isDigit = char.IsDigit(e.KeyChar);
         var isControl = char.IsControl(e.KeyChar);

         if (EnableDecimal)
         {
            var isPeriod = (e.KeyChar == 46); // .
            var existsInText = Text.Contains(e.KeyChar.ToString());

            e.Handled = !(isPeriod && !existsInText) && !isDigit && !isControl;
         }
         else
            e.Handled = !isDigit && !isControl;
      }
   }
}
