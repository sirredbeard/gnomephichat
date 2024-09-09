using System;
using Gtk;
using Microsoft.ML.OnnxRuntimeGenAI;

class Program
{
    static void Main(string[] args)
    {
        Application.Init();
        var win = new Window("AI Assistant");
        win.SetDefaultSize(800, 600);
        win.DeleteEvent += (o, e) => Application.Quit();

        var vbox = new VBox();
        var textView = new TextView();
        textView.Editable = false;
        var textEntry = new Entry();

        vbox.PackStart(new ScrolledWindow { textView }, true, true, 0);
        vbox.PackStart(textEntry, false, false, 0);
        win.Add(vbox);

        var modelPath = Path.Combine(AppContext.BaseDirectory, @"Phi-3-mini-4k-instruct-onnx\cpu-int4-rtn-block-32-acc-level-4");
        var model = new Model(modelPath);
        var tokenizer = new Tokenizer(model);

        var systemPrompt = "You are an AI assistant that helps people find information. Answer questions using a direct style. Do not share more information that the requested by the users.";

        textEntry.Activated += (sender, e) =>
        {
            var userQ = textEntry.Text;
            if (string.IsNullOrEmpty(userQ))
            {
                Application.Quit();
                return;
            }

            textView.Buffer.Text += $"Q: {userQ}\n";
            textEntry.Text = string.Empty;

            var fullPrompt = $"<|system|>{systemPrompt}<|end|><|user|>{userQ}<|end|><|assistant|>";
            var tokens = tokenizer.Encode(fullPrompt);

            var generatorParams = new GeneratorParams(model);
            generatorParams.SetSearchOption("max_length", 2048);
            generatorParams.SetSearchOption("past_present_share_buffer", true);
            generatorParams.SetInputSequences(tokens);

            var generator = new Generator(model, generatorParams);
            while (!generator.IsDone())
            {
                generator.ComputeLogits();
                generator.GenerateNextToken();
                var outputTokens = generator.GetSequence(0);
                var newToken = outputTokens.Slice(outputTokens.Length - 1, 1);
                var output = tokenizer.Decode(newToken);
                textView.Buffer.Text += $"Phi3: {output}";
            }
            textView.Buffer.Text += "\n";
        };

        win.ShowAll();
        Application.Run();
    }
}