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
        private readonly Label status = new Label(), irName = new Label();
        private readonly Button clearIr = new Button();
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
            ClientSize = new Size(492, 356);
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
            irName.Bounds = new Rectangle(18, 216, 240, 22);
            irName.AutoEllipsis = true;
            irName.AccessibleName = "IR da caixa em uso";
            Controls.Add(irName);
            ConfigureButton(new Button(), "Carregar IR...", 265, 118, 210, LoadImpulse);
            ConfigureButton(clearIr, "Usar interna", 391, 83, 210, (sender, args) =>
            {
                CabinetIrLibrary.ClearCustom();
                ApplyImpulse();
            });
            UpdateImpulseName();

            status.Bounds = new Rectangle(18, 249, 456, 52);
            status.AccessibleName = "Status da monitoração";
            Controls.Add(status);

            ConfigureButton(refresh, "Atualizar lista", 18, 112, 309, (sender, args) => RefreshDevices());
            ConfigureButton(new Button(), "Inspecionar DSP", 136, 114, 309, (sender, args) => { DialogResult = DialogResult.Yes; });
            ConfigureButton(start, "Ouvir guitarra", 265, 118, 309, ToggleMonitoring);
            var close = new Button { DialogResult = DialogResult.Cancel };
            ConfigureButton(close, "Fechar", 391, 83, 309, null);
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

        private void LoadImpulse(object sender, EventArgs args)
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Title = "Escolha a IR da caixa";
                dialog.Filter = "Resposta de impulso (*.wav)|*.wav";
                if (dialog.ShowDialog(this) != DialogResult.OK) return;
                try
                {
                    CabinetIrLibrary.LoadCustom(dialog.FileName);
                }
                catch (Exception ex)
                {
                    if (!(ex is System.IO.IOException || ex is System.IO.InvalidDataException || ex is UnauthorizedAccessException
                        || ex is ArgumentException || ex is OverflowException || ex is OutOfMemoryException)) throw;
                    MessageBox.Show(this, "Não foi possível usar esta IR: " + ex.Message, Text,
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            ApplyImpulse();
        }

        private void ApplyImpulse()
        {
            if (audio.Running || audio.Starting) audio.SetParameters(getParameters());
            UpdateImpulseName();
        }

        private void UpdateImpulseName()
        {
            string custom = CabinetIrLibrary.CustomName;
            irName.Text = custom ?? "Interna (segue a caixa e os microfones do preset)";
            clearIr.Enabled = custom != null;
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
