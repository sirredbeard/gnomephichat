using System;
using Gtk;
using Microsoft.ML.OnnxRuntimeGenAI;

class Program
{
    static void Main(string[] args)
    {
        Application.Init();
        var win = new Window("gnomephichat");
        win.SetDefaultSize(800, 600);
        win.DeleteEvent += (o, e) => Application.Quit();

        var vbox = new VBox();
        var headerBox = new HBox();
        var textView = new TextView();
        textView.Editable = false;
        textView.WrapMode = WrapMode.Word; // Enable text wrapping
        var textEntry = new Entry();

        // Create a button with a trash can icon
        var clearButton = new Button();
        var trashIcon = new Image(Stock.Delete, IconSize.Button);
        clearButton.Image = trashIcon;

        clearButton.Clicked += (sender, e) => textView.Buffer.Text = string.Empty;

        var comboBox = new ComboBox(new string[] { "Phi-3-mini" });
        comboBox.Active = 0; // Default to the first option

        headerBox.PackStart(comboBox, false, false, 0);
        headerBox.PackEnd(clearButton, false, false, 0);

        vbox.PackStart(headerBox, false, false, 0);
        vbox.PackStart(new ScrolledWindow { textView }, true, true, 0);

        var entryBox = new HBox();
        entryBox.PackStart(textEntry, true, true, 0);

        // Create a button with a forward icon
        var sendButton = new Button();
        var mailIcon = new Image(Stock.GoForward, IconSize.Button);
        sendButton.Image = mailIcon;

        // Create a spinner for loading indication
        var spinner = new Spinner();
        spinner.Visible = false;

        entryBox.PackEnd(spinner, false, false, 0);
        entryBox.PackEnd(sendButton, false, false, 0);
        vbox.PackStart(entryBox, false, false, 0);

        win.Add(vbox);

        string modelPath = GetModelPath(comboBox.Active);
        Model model = null;
        Tokenizer tokenizer = null;

        try
        {
            model = new Model(modelPath);
            tokenizer = new Tokenizer(model);
        }
        catch (Exception ex)
        {
            textView.Buffer.Text += $"Error loading model: {ex.Message}\n";
        }

        var systemPrompt = "You are an AI assistant that helps people find information. Answer questions using a direct style. Do not share more information that the requested by the users.";

        comboBox.Changed += (sender, e) =>
        {
            modelPath = GetModelPath(comboBox.Active);
            try
            {
                model = new Model(modelPath);
                tokenizer = new Tokenizer(model);
                textView.Buffer.Text += $"Model switched to: CPU-powered Phi-3-mini\n";
            }
            catch (Exception ex)
            {
                textView.Buffer.Text += $"Error loading model: {ex.Message}\n";
            }
        };

        EventHandler submitEntry = (sender, e) =>
        {
            var userQ = textEntry.Text;
            if (string.IsNullOrEmpty(userQ))
            {
                Application.Quit();
                return;
            }

            textView.Buffer.Text += $"Q: {userQ}\n";
            textEntry.Text = string.Empty;

            if (tokenizer == null)
            {
                textView.Buffer.Text += "Error: Model not loaded.\n";
                return;
            }

            // Show spinner
            spinner.Visible = true;
            spinner.Start();

            var fullPrompt = $"<|system|>{systemPrompt}<|end|><|user|>{userQ}<|end|><|assistant|>";
            var tokens = tokenizer.Encode(fullPrompt);

            var generatorParams = new GeneratorParams(model);
            generatorParams.SetSearchOption("max_length", 2048);
            generatorParams.SetSearchOption("past_present_share_buffer", true);
            generatorParams.SetInputSequences(tokens);

            var generator = new Generator(model, generatorParams);
            var outputTokens = new List<int>();

            while (!generator.IsDone())
            {
                generator.ComputeLogits();
                generator.GenerateNextToken();
                var newToken = generator.GetSequence(0).Slice(generator.GetSequence(0).Length - 1, 1);
                outputTokens.AddRange(newToken);
            }

            var output = tokenizer.Decode(outputTokens.ToArray());
            textView.Buffer.Text += $"Phi3: {output}\n";

            // Hide spinner
            spinner.Stop();
            spinner.Visible = false;
        };

        textEntry.Activated += submitEntry;
        sendButton.Clicked += submitEntry;

        win.ShowAll();
        Application.Run();
    }

    static string GetModelPath(int selectedIndex)
    {
        return Path.Combine(AppContext.BaseDirectory, @"Phi-3-mini-4k-instruct-onnx/cpu-int4-rtn-block-32-acc-level-4");
    }
}