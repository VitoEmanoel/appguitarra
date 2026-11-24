using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ArchetypeGojiraX
{
    internal sealed class AudioSelection
    {
        public AudioEndpoint Input, Output;
        public int Channel;
    }

    internal sealed class AudioSettingsDialog : Form
    {
        private readonly GuitarAudio audio;
        private readonly AudioSelection selection;
        private readonly Func<GuitarSettings> getParameters;
        private readonly ComboBox input = new ComboBox(), output = new ComboBox(), channel = new ComboBox();
        private readonly Button start = new Button(), refresh = new Button();
        private readonly Label status = new Label();
        private readonly ComboBox impulses = new ComboBox();
        private bool fillingImpulses;
        private readonly Timer timer = new Timer();
        private string deviceError;

        public AudioSettingsDialog(GuitarAudio audio, AudioSelection selection, Func<GuitarSettings> getParameters)
        {
            this.audio = audio;
            this.selection = selection;
            this.getParameters = getParameters;

            Text = "Áudio da guitarra";
            Font = new Font("Segoe UI", 9);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(492, 388);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = MinimizeBox = ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;

            AddLabel("Entrada de áudio", 18);
            AddList(input, 39);
            input.AccessibleName = "Entrada de áudio";
            AddLabel("Canal da guitarra", 75);
            AddList(channel, 96);
            channel.AccessibleName = "Canal da guitarra";
            channel.Items.AddRange(new object[] { "Entrada 1 (canal esquerdo)", "Entrada 2 (canal direito)" });
            channel.SelectedIndex = selection.Channel;
            AddLabel("Saída de áudio", 132);
            AddList(output, 153);
            output.AccessibleName = "Saída de áudio";

            AddLabel("Caixa (IR)", 191);
            AddList(impulses, 212);
            impulses.AccessibleName = "IR da caixa";
            impulses.SelectedIndexChanged += ImpulseChanged;
            new ToolTip().SetToolTip(impulses, "IRs incluídas: Adventure Kid (adventurekid.se), licença CC BY 4.0.\n"
                + "Detalhes em CREDITOS-IRS.md no repositório.");
            ConfigureButton(new Button(), "Carregar de arquivo...", 18, 150, 245, LoadImpulse);
            ConfigureButton(new Button(), "Abrir pasta Minhas IRs", 176, 160, 245, OpenUserFolder);
            ConfigureButton(new Button(), "Atualizar IRs", 344, 130, 245, (sender, args) => FillImpulses());
            FillImpulses();

            status.Bounds = new Rectangle(18, 283, 456, 50);
            status.AccessibleName = "Status da monitoração";
            Controls.Add(status);

            ConfigureButton(refresh, "Atualizar lista", 18, 112, 341, (sender, args) => RefreshDevices());
            ConfigureButton(new Button(), "Inspecionar DSP", 136, 114, 341, (sender, args) => { DialogResult = DialogResult.Yes; });
            ConfigureButton(start, "Ouvir guitarra", 265, 118, 341, ToggleMonitoring);
            var close = new Button { DialogResult = DialogResult.Cancel };
            ConfigureButton(close, "Fechar", 391, 83, 341, null);
            CancelButton = close;

            input.SelectedIndexChanged += SelectionChanged;
            output.SelectedIndexChanged += SelectionChanged;
            RefreshDevices();
            timer.Interval = 100;
            timer.Tick += (sender, args) => UpdateStatus();
            timer.Start();
        }

        private void AddLabel(string text, int y)
        {
            Controls.Add(new Label { Text = text, AutoSize = true, Location = new Point(18, y) });
        }

        private void AddList(ComboBox list, int y)
        {
            list.Bounds = new Rectangle(18, y, 456, 27);
            list.DropDownStyle = ComboBoxStyle.DropDownList;
            Controls.Add(list);
        }

        private void ConfigureButton(Button button, string text, int x, int width, int y, EventHandler click)
        {
            button.Text = text;
            button.Bounds = new Rectangle(x, y, width, 30);
            if (click != null) button.Click += click;
            Controls.Add(button);
        }

        private void FillImpulses()
        {
            fillingImpulses = true;
            try
            {
                IrChoice[] choices = CabinetIrLibrary.ListChoices();
                impulses.Items.Clear();
                impulses.Items.AddRange(choices);
                string current = CabinetIrLibrary.Selected.Key;
                impulses.SelectedIndex = Math.Max(0, Array.FindIndex(choices, choice => choice.Key == current));
            }
            finally { fillingImpulses = false; }
        }

        private void ImpulseChanged(object sender, EventArgs args)
        {
            var choice = impulses.SelectedItem as IrChoice;
            if (fillingImpulses || choice == null || choice.Key == CabinetIrLibrary.Selected.Key) return;
            UseImpulse(choice);
        }

        private void LoadImpulse(object sender, EventArgs args)
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Title = "Escolha a IR da caixa";
                dialog.Filter = "Resposta de impulso (*.wav)|*.wav";
                if (dialog.ShowDialog(this) != DialogResult.OK) return;
                UseImpulse(CabinetIrLibrary.ChoiceForFile(dialog.FileName));
            }
        }

        private void UseImpulse(IrChoice choice)
        {
            try
            {
                CabinetIrLibrary.Select(choice);
            }
            catch (Exception ex)
            {
                if (!(ex is System.IO.IOException || ex is System.IO.InvalidDataException || ex is UnauthorizedAccessException
                    || ex is ArgumentException || ex is OverflowException || ex is OutOfMemoryException)) throw;
                MessageBox.Show(this, "Não foi possível usar esta IR: " + ex.Message, Text,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            if (audio.Running || audio.Starting) audio.SetParameters(getParameters());
            FillImpulses();
        }

        // Pasta pessoal: o que estiver aqui aparece na lista, mas fica só neste computador.
        private void OpenUserFolder(object sender, EventArgs args)
        {
            try
            {
                System.IO.Directory.CreateDirectory(CabinetIrLibrary.UserFolder);
                System.Diagnostics.Process.Start("explorer.exe", "\"" + CabinetIrLibrary.UserFolder + "\"");
            }
            catch (Exception ex)
            {
                if (!(ex is System.IO.IOException || ex is UnauthorizedAccessException || ex is System.ComponentModel.Win32Exception)) throw;
                MessageBox.Show(this, "Não foi possível abrir a pasta: " + ex.Message, Text,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void RefreshDevices()
        {
            deviceError = null;
            try
            {
                AudioEndpoint[] inputs = WindowsAudio.GetEndpoints(WindowsAudio.Capture);
                AudioEndpoint[] outputs = WindowsAudio.GetEndpoints(WindowsAudio.Render);
                Populate(input, inputs, input.SelectedItem as AudioEndpoint ?? selection.Input);
                Populate(output, outputs, output.SelectedItem as AudioEndpoint ?? selection.Output);
                if (inputs.Length == 0 || outputs.Length == 0)
                    deviceError = "Conecte uma entrada e uma saída de áudio e atualize a lista.";
            }
            catch (COMException ex)
            {
                deviceError = "Falha ao listar os dispositivos de áudio (0x" + ex.ErrorCode.ToString("X8") + ").";
            }
            UpdateStatus();
        }

        private static void Populate(ComboBox list, AudioEndpoint[] devices, AudioEndpoint previous)
        {
            list.Items.Clear();
            list.Items.AddRange(devices);
            int index = previous == null ? -1 : Array.FindIndex(devices, device => device.Id == previous.Id);
            list.SelectedIndex = previous == null && devices.Length == 1 ? 0 : index;
        }

        private void SelectionChanged(object sender, EventArgs args) { UpdateStatus(); }

        private void ToggleMonitoring(object sender, EventArgs args)
        {
            if (audio.Starting || audio.Running)
            {
                audio.Stop();
            }
            else
            {
                selection.Input = input.SelectedItem as AudioEndpoint;
                selection.Output = output.SelectedItem as AudioEndpoint;
                selection.Channel = channel.SelectedIndex;
                if (selection.Input == null || selection.Output == null) return;
                audio.Start(selection.Input, selection.Output, selection.Channel, getParameters());
            }
            UpdateStatus();
        }

        private void UpdateStatus()
        {
            bool running = audio.Running, starting = audio.Starting;
            input.Enabled = output.Enabled = channel.Enabled = refresh.Enabled = !running && !starting;
            start.Enabled = running || starting || (deviceError == null && input.SelectedIndex >= 0 && output.SelectedIndex >= 0);
            start.Text = running ? "Parar áudio" : starting ? "Cancelar" : "Ouvir guitarra";
            status.Text = running ? "Monitoração ativa. Você pode fechar esta janela e ajustar o timbre."
                : starting ? "Abrindo os dispositivos de áudio..."
                : deviceError ?? audio.Error ?? "Selecione a entrada e a saída da interface. Comece com o volume do fone baixo.";
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) timer.Dispose();
            base.Dispose(disposing);
        }
    }
}
